using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassPopulationManager : MonoBehaviour
{
	public static GrassPopulationManager instance;

	public static GameObject[] Fences;
	public static GameObject Tree;

	[Range(0, 100)]
	public int ChanceFences;

	[Range(0, 15)]
	public int MaximumTreeDensity;

	public GameObject _Tree;

	void Awake()
	{
		instance = this;

		Fences = Resources.LoadAll<GameObject>("Prefabs/Destructable Scenery/Fences/Hedgerows");

		Tree = _Tree;
	}
}
