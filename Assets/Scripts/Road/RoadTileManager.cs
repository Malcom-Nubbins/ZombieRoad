using System.Collections;
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
	}

	GameObject EmergencyFieldRemover;

	float RoundDownToGrid(float a)
	{
		return 20.0f * (int)(a / 20);
	}

	Vector3 RoundDownToGrid(Vector3 a)
	{
		return new Vector3(RoundDownToGrid(a.x), RoundDownToGrid(a.y), RoundDownToGrid(a.z));
	}

	void FixedUpdate()
	{
		//Debug.Log("Tick, count=" + checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>().Length + ", 110's name "+ checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[110].gameObject.name);
		if (Input.GetKey(KeyCode.Keypad0))
			bCull = !bCull;

		//TimeUntilFieldCheck--;
		//if (TimeUntilFieldCheck==0)
		//{
		//	//110 is slightly under a quarter on average
		if (Time.fixedTime > 1 && EmergencyFieldRemover == null && checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[110] as DisabledRoadGenerator != null)
		{
			int i = RoadGenerator.Wrap0to7((int)(checkpoint.FollowCamera.GetComponent<FollowCamera>().target.transform.rotation.eulerAngles.y / 45.0f));
			RaycastHit Hit; Physics.Raycast(RoundDownToGrid(checkpoint.FollowCamera.GetComponent<FollowCamera>().target.transform.position) + new Vector3(RoadGenerator.Xoffset(i) * 12, 500, RoadGenerator.Zoffset(i) * 12), new Vector3(0, -1), out Hit, Mathf.Infinity, 1 << 9);

			if (!Hit.collider)
			{
				Debug.Log("FUCKIN FIELD");

				EmergencyFieldRemover = Instantiate(FourWay, RoundDownToGrid(checkpoint.FollowCamera.GetComponent<FollowCamera>().target.transform.position) + new Vector3(RoadGenerator.Xoffset(i) * 12, FourWay.GetComponent<RoadGenerator>().YOffset, RoadGenerator.Zoffset(i) * 12), Quaternion.identity, checkpoint.RoadMapRoot.transform);
				EmergencyFieldRemover.transform.SetAsFirstSibling();
				RoadGenerator newRG = EmergencyFieldRemover.GetComponent<RoadGenerator>();
				newRG.CullingExempt = true;
				newRG.RefreshExits();
				//newRG.Extend(true);
				//checkpoint.RoadMapRoot.BroadcastMessage("Extend", true);
			}
		}
		else if (EmergencyFieldRemover != null && Vector3.Distance(checkpoint.FollowCamera.GetComponent<FollowCamera>().target.transform.position, EmergencyFieldRemover.transform.position) < checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance)
		{
			EmergencyFieldRemover.GetComponent<RoadGenerator>().CullingExempt = false;
			EmergencyFieldRemover = null;
		}
		//}
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
