﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTileManager : MonoBehaviour
{
	public static RoadTileManager instance;

	public static GameObject Grass;
	public static GameObject Pavement;
	public static GameObject Straight;
	public static GameObject Corner;
	public static GameObject T;

	public static GameObject DeadEnd;
	public static GameObject FourWay;

	public static Checkpoint checkpoint;

	public GameObject _Grass;
	public GameObject _Pavement;
	public GameObject _Straight;
	public GameObject _Corner;
	public GameObject _T;

	public GameObject _DeadEnd;
	public GameObject _FourWay;

	public Checkpoint _Checkpoint;

	[Range(0, 100)]
	public int ChanceStraight;
	[Range(0, 100)]
	public int ChanceCorner;
	[Range(0, 100)]
	public int ChanceT;

	[Range(0, 100)]
	public int ChancePavement;
	[Range(0, 100)]
	public int ChanceGrass;

	[Range(0, 100)]
	public int ChanceBarrier;

	public bool _Cull = true;
	public static bool bCull;

	public static bool bMainMenu;

	//int MaxTimeUntilFieldCheck = 450;
	//int TimeUntilFieldCheck;

	void Awake()
	{
		instance = this;
		Grass = _Grass;
		Pavement = _Pavement;
		Straight = _Straight;
		Corner = _Corner;
		T = _T;

		DeadEnd = _DeadEnd;
		FourWay = _FourWay;

		checkpoint = _Checkpoint;

		bCull = _Cull;

		//TimeUntilFieldCheck = MaxTimeUntilFieldCheck;

		bMainMenu = GetComponent<MainMenuDummyCheckpoint>() != null;
	}

	GameObject EmergencyFieldRemover;

	float RoundDownToGrid(float a)
	{
		return WorldTileManager.TILE_SIZE * (int)(a / WorldTileManager.TILE_SIZE);
	}

	Vector3 RoundDownToGrid(Vector3 a)
	{
		return new Vector3(RoundDownToGrid(a.x), RoundDownToGrid(a.y), RoundDownToGrid(a.z));
	}

    void Update()
    {
        foreach (WorldTile tile in WorldTileManager.instance.GetAllTiles())
        {
            if (tile is RoadGenerator)
            {
                ((RoadGenerator)tile).Extend(false);
            }
        }
    }

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Keypad0))
			bCull = !bCull;

		Transform playerTransform = checkpoint.FollowCamera.GetComponent<FollowCamera>().target.transform;

		//Debug.Log(checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>().Length + " 35:" + checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[35].gameObject.name);

		if (checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>().Length > 100 /*usually around 140, prevent checking until the map is up to size*/ && EmergencyFieldRemover == null && checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[35] as DisabledRoadGenerator != null /*35 is around a quarter, meaning grass at this position indicates a map with three quarters grass*/)
		{
			int i = RoadGenerator.Wrap0to7(Mathf.RoundToInt(playerTransform.rotation.eulerAngles.y / 45.0f));
			TilePosition hitTest = new TilePosition(RoundDownToGrid(playerTransform.position)) + new TilePosition(RoadGenerator.Xoffset(i) * 8, RoadGenerator.Zoffset(i) * 8);
			//Debug.Log("Testing for a hit at grid "+hitTest.x+","+hitTest.z+"; World location "+hitTest.GetWorldPosition());
			WorldTile Hit = WorldTileManager.instance.GetTile(hitTest);

			if (!Hit)
			{
				//Debug.Log("FUCKIN FIELD, heading " + playerTransform.rotation.eulerAngles.y + "≈" + (RoadGenerator.Direction)i);

				EmergencyFieldRemover = Instantiate(FourWay, hitTest.GetWorldPosition() + new Vector3(0, FourWay.GetComponent<RoadGenerator>().YOffset, 0), Quaternion.identity, checkpoint.RoadMapRoot.transform);
				//Debug.Log("EFR: "+EmergencyFieldRemover.gameObject.name + " at " + (hitTest.GetWorldPosition() + new Vector3(0, FourWay.GetComponent<RoadGenerator>().YOffset, 0)));
				WorldTileManager.instance.AddTile(EmergencyFieldRemover.GetComponent<WorldTile>());
				EmergencyFieldRemover.transform.SetAsFirstSibling();
				RoadGenerator newRG = EmergencyFieldRemover.GetComponent<RoadGenerator>();
				newRG.CullingExempt = true;
				newRG.RefreshExits();
				//newRG.Extend(true);
				//checkpoint.RoadMapRoot.BroadcastMessage("Extend", true);
			}
		}
		else if // Emergency Field Remover has come within the invisible boundary lines (σ回ω・)σ
			((EmergencyFieldRemover != null && Vector3.Distance(playerTransform.position, EmergencyFieldRemover.transform.position) < checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance)
			|| // Emergency Field Remover is no longer in front of the player
			(EmergencyFieldRemover != null && Vector3.Dot(playerTransform.position - EmergencyFieldRemover.transform.position, playerTransform.forward) > 90))
		{
			EmergencyFieldRemover.GetComponent<RoadGenerator>().CullingExempt = false;
			EmergencyFieldRemover = null;
		}
	}

	public static GameObject RandomRoadTile(bool bAllowQuad)
	{
		int r = Random.Range(0, 100);

		if (r < instance.ChanceCorner)
		{
			return Corner;
		}
		else if (r < instance.ChanceCorner + instance.ChanceT)
		{
			return bAllowQuad ? FourWay : T;
		}

		return Straight;
	}

	public static GameObject RandCornerT(bool bAllowQuad)
	{
		int r = Random.Range(0, instance.ChanceCorner + instance.ChanceT);

		if (r < instance.ChanceCorner)
		{
			return bAllowQuad ? T : Corner;
		}

		return bAllowQuad ? FourWay : T;
	}

	public static GameObject RandStraightT()
	{
		int r = Random.Range(0, instance.ChanceStraight + instance.ChanceT);

		if (r < instance.ChanceStraight)
		{
			return Straight;
		}

		return T;
	}

	public static GameObject RandQuadT()
	{
		//int r = Random.Range(0, instance.ChanceStraight + instance.ChanceT);

		//if (r < instance.ChanceStraight)
		//{
		//	return FourWay;
		//}

		return FourWay;
	}

	public static GameObject RandCornerDead()
	{
		int r = Random.Range(0, instance.ChanceCorner + instance.ChanceT);

		if (r < instance.ChanceT)
		{
			return Corner;
		}

		return DeadEnd;
	}

	public static GameObject RandPavementGrass()
	{
		int r = Random.Range(0, 100);

		if (r < instance.ChanceGrass)
		{
			return Grass;
		}

		return Pavement;
	}

	public void OnValidate()
	{
		if (ChanceStraight + ChanceCorner + ChanceT != 100)
			Debug.LogError("ChanceStraight + ChanceCorner + ChanceT ≠ 100");
		else
			Debug.Log("Road chances are fine");

		if (ChanceGrass + ChancePavement != 100)
			Debug.LogError("ChanceGrass + ChancePavement ≠ 100");
		else
			Debug.Log("Pavement vs Grass chances are fine");
	}
}
