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

	public bool _Cull = true;
	public static bool bCull;

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
	}

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Keypad0))
			bCull = !bCull;
	}

	public static GameObject RandomRoadTile()
	{
		int r = Random.Range(0, 100);

		if (r < instance.ChanceCorner)
		{
			return Corner;
		}
		else if (r < instance.ChanceCorner + instance.ChanceT)
		{
			return T;
		}

		return Straight;
	}

	public static GameObject RandCornerT()
	{
		int r = Random.Range(0, instance.ChanceCorner + instance.ChanceT);

		if (r < instance.ChanceCorner)
		{
			return Corner;
		}

		return T;
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
		int r = Random.Range(0, instance.ChanceStraight + instance.ChanceT);

		if (r < instance.ChanceStraight)
		{
			return FourWay;
		}

		return T;
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
