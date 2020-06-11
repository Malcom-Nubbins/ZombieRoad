using System.Collections.Generic;
using UnityEngine;

namespace ZR.Road.Buildings
{
	public class BuildingGenerator : MonoBehaviour
	{
		public GameObject newBuilding;
		void Update()
		{
			bool bHasNeighbour = false;

			BuildingGenerator[] hit = new BuildingGenerator[8];

			TilePosition position = gameObject.GetComponent<WorldTile>().GetTilePosition();

			for (int i = 0; i < hit.Length; ++i)
			{
				hit[i] = WorldTileManager.instance.GetTile(position + new TilePosition(RoadGenerator.Xoffset(i), RoadGenerator.Zoffset(i)))?.GetComponent<BuildingGenerator>();
				if (hit[i] && hit[i].newBuilding) bHasNeighbour = true;
				//if (RoadTileManager.bDebugEnv) gameObject.GetComponent<RoadGenerator>().MySpecificDebug += "Checking " + (RoadGenerator.Direction)i + " of " + gameObject.transform.position + " has hit " + (hit[i] ? hit[i].gameObject.name : "nothing") + "\n";
			}

			GameObject newBuildingClass = null;
			if (!bHasNeighbour)
			{
				//if (RoadTileManager.bDebugEnv) gameObject.GetComponent<RoadGenerator>().MySpecificDebug += "Random Building\n";
				newBuildingClass = BuildingManager.RandomBuilding();
			}
			else
			{
				List<GameObject> neighbours = new List<GameObject>();
				for (int i = 0; i < hit.Length; ++i)
				{
					if (hit[i]?.newBuilding)
						neighbours.Add(hit[i].newBuilding);
				}

				int r = Random.Range(0, neighbours.Count);
				GameObject n = neighbours[r];
				//if (RoadTileManager.bDebugEnv) gameObject.GetComponent<RoadGenerator>().MySpecificDebug += n.GetComponent<Unlockable>().type.ToString()+"\n";

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
				newBuilding = Instantiate(newBuildingClass, transform.position, transform.rotation, transform);
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
}
