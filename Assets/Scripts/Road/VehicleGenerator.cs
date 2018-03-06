using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		GameObject player = RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().target;
		RoadGenerator rg = gameObject.GetComponent<RoadGenerator>();
		if (rg.Exit.Length < 8) rg.RefreshExits();
		int i = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.South;

		if (rg.CullingExempt) return;

		int r = Random.Range(0, 10);
		if (r == 1)
		{
			r = Random.Range(0, 2);
			bool bSpinBench = false;
			if (r == 1) { i -= 4; bSpinBench = true; } // place bench east or north instead of west or south

			GameObject benchTemplate = Resources.Load<GameObject>("Prefabs/Destructable Scenery/bench");
			GameObject bench = Instantiate(benchTemplate, gameObject.transform.position + new Vector3(RoadGenerator.Xoffset(i) / 2.25f + RoadGenerator.Xoffset(RoadGenerator.Wrap0to7(i-2)) / 5, 1.6f, RoadGenerator.Zoffset(i) / 2.25f + RoadGenerator.Zoffset(RoadGenerator.Wrap0to7(i-2)) / 5), gameObject.transform.rotation);
			if (bSpinBench) bench.transform.Rotate(0, 180, 0);
		}

		r = Random.Range(0, 100);

		bool bInVehicle = player.GetComponent<BaseVehicleClass>();
        
		if (r <= 40 && UnlockManager.instance)
		{
			GameObject[] UnlockedCars = UnlockManager.instance.GetUnlockedItems(UnlockableType.VEHICLE);
			r = Random.Range(0, UnlockedCars.Length);
			i = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.South;
			GameObject car = Instantiate(UnlockedCars[r], gameObject.transform.position+new Vector3(RoadGenerator.Xoffset(i)/4,3,RoadGenerator.Zoffset(i)/4), gameObject.transform.rotation);
			//rg.MySpecificDebug += "generated vehicle @ " + car.transform.position + ", will not cull\n";
			//rg.CullingExempt = true;
			if (RoadTileManager.bMainMenu) car.GetComponent<BaseVehicleClass>().health = 0;
		}

		if ((RoadTileManager.bMainMenu || bInVehicle) && r > 100 - RoadTileManager.instance.ChanceBarrier)
		{
			GameObject[] RoadBlocks = Resources.LoadAll<GameObject>("Prefabs/Destructable Scenery/Fences/Road Barriers");
			r = Random.Range(0, RoadBlocks.Length);
			Instantiate(RoadBlocks[r], transform.position, transform.rotation, transform);
		}
		enabled = false;
	}
}