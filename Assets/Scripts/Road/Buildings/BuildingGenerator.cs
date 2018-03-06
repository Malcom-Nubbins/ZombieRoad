using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
	void Update()
	{
		bool bHasNeighbour = false;

		RaycastHit[] hit = new RaycastHit[8];

		for (int i = 0; i < hit.Length; i++)
		{
			Physics.Raycast(transform.position + new Vector3(RoadGenerator.Xoffset(i), 500, RoadGenerator.Zoffset(i)), new Vector3(0, -1), out hit[i], Mathf.Infinity, LayerMask.GetMask("Building"));
			if (hit[i].collider) bHasNeighbour = true;
			//gameObject.GetComponent<RoadGenerator>().MySpecificDebug += "ray checking " + (RoadGenerator.Direction)i + " of " + gameObject.transform.position + " has hit " + (hit[i].collider ? hit[i].collider.gameObject.name : "nothing") + "\n";
		}

		GameObject newBuildingClass = null;
		if (!bHasNeighbour)
		{
			newBuildingClass = BuildingManager.RandomBuilding();
		}
		else
		{
			List<GameObject> neighbours = new List<GameObject>();
			foreach (RaycastHit h in hit)
				if (h.collider)
					neighbours.Add(h.collider.gameObject);

			int r = Random.Range(0, neighbours.Count);
			GameObject n = neighbours[r];

			if (BuildingManager.IsSkyscraper(n))
			{
				newBuildingClass = BuildingManager.RandomSkyscraper();
			}
			else if (BuildingManager.IsHouse(n))
			{
				newBuildingClass = BuildingManager.RandomHouse();
			}
			else if (BuildingManager.IsShop(n))
			{
				newBuildingClass = BuildingManager.RandomShopOrHouse();
			}
		}

		if (newBuildingClass)
		{
			/*GameObject newBuilding =*/ Instantiate(newBuildingClass, transform.position, transform.rotation, transform);
		}
		else
		{
			GameObject[] RoadBlocks = Resources.LoadAll<GameObject>("Prefabs/Destructable Scenery/Fences/Pavement Barriers");
			int r = Random.Range(0, RoadBlocks.Length);
			Instantiate(RoadBlocks[r], transform.position, transform.rotation, transform);
		}

		enabled = false;
	}
}
