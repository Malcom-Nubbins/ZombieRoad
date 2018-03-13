using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
	void Update()
	{
		bool bHasNeighbour = false;

		WorldTile[] hit = new WorldTile[8];

		for (int i = 0; i < hit.Length; i++)
		{
            hit[i] = WorldTileManager.instance.GetTile(new TilePosition(RoadGenerator.Xoffset(i), RoadGenerator.Zoffset(i)));
            if (hit[i].gameObject.layer != LayerMask.NameToLayer("Building")) hit[i] = null;
            if (hit[i]) bHasNeighbour = true;
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
			foreach (WorldTile h in hit)
				if (h)
					neighbours.Add(h.gameObject);

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
