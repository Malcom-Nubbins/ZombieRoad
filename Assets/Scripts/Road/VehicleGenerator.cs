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

		//GameObject streetlightTemplate = Resources.Load<GameObject>("Prefabs/Destructable Scenery/street_light");
		int r /*= Random.Range(0, 2)*/;
		//r = r * 2 - 1;
		//r *= 5;
		int i/* = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.North*/;
		//GameObject streetlight = Instantiate(streetlightTemplate, gameObject.transform.position + new Vector3(RoadGenerator.Xoffset(i) / 4 *1.1f, 0.6f, RoadGenerator.Zoffset(i) / 4 * 1.1f), gameObject.transform.rotation);
		//streetlight.transform.Rotate(0, (i==(int)RoadGenerator.Direction.West?1:-1) * 90, 0);

		if (!player.GetComponent<BaseVehicleClass>() || player.GetComponent<BaseVehicleClass>().GetFuelPercentage() < 0.1f)
		{
			r = Random.Range(0, 100);
			if (r <= 10 && UnlockManager.instance)
			{
				GameObject[] UnlockedCars = UnlockManager.instance.GetUnlockedItems(UnlockableType.VEHICLE);
				r = Random.Range(0, UnlockedCars.Length-1);
				i = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.South;
				GameObject car = Instantiate(UnlockedCars[r], gameObject.transform.position+new Vector3(RoadGenerator.Xoffset(i)/4,3,RoadGenerator.Zoffset(i)/4), gameObject.transform.rotation);
				//rg.MySpecificDebug += "generated vehicle @ " + car.transform.position + ", will not cull\n";
				//rg.CullingExempt = true;
			}
		}
		enabled = false;
	}
}