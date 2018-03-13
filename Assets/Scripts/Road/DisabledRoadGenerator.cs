using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledRoadGenerator : RoadGenerator
{
	public enum Type : int
	{
		Pavement,
		Grass,
		xInvalid
	}

	static RaycastHit RaycastHitNull = new RaycastHit();

	public Type TileClassification;

	override public void Extend(bool bForceOOBExtension = false)
	{
		bHaveExpanded = false;

		if (!ShouldExtend()) return;

		for (int i = 0; i < hit.Length; i += 2)
		{
			if (!hit[i])
			{
				bool bProbablyHole = true;
				for (int j = 0; j < hitPlus.Length; j += 2)
				{
					hitPlus[j] = null;
					int k = 1;
					TilePosition RayLoc = GetTilePosition() + new TilePosition(k * Xoffset(j) + Xoffset(i), k * Zoffset(j) + Zoffset(i));
					while (!hitPlus[j] && (Vector3.Distance(CachedPlayerPosition, new Vector3(RayLoc.x, 0, RayLoc.z)) < (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100)))
					{
                        hitPlus[j] = WorldTileManager.instance.GetTile(RayLoc);
						k++;
                        RayLoc = GetTilePosition() + new TilePosition(k * Xoffset(j) + Xoffset(i), k * Zoffset(j) + Zoffset(i));
                    }
					if (hitPlus[j] && hitPlus[j].GetComponent<RoadGenerator>().Exit.Length < 8) hitPlus[j].GetComponent<RoadGenerator>().RefreshExits();

					MySpecificDebug += (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) + "\n";

					//if (!((RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) < 20 && (Vector3.Distance(CachedPlayerPosition, RayLocNoY) > (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100))))
					//{
					//	MySpecificDebug += "Nearby boundary hit while hole scanning\n";
					//	bProbablyHole = true;
					//}
					if ((!((RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) < 20 && (Vector3.Distance(CachedPlayerPosition, new Vector3(RayLoc.x, 0, RayLoc.z)) > (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100)))) && (!hitPlus[j] || !hitPlus[j].GetComponent<DisabledRoadGenerator>()))
					{
						bProbablyHole = false;
					}
				}
				if (bProbablyHole)
				{
					GameObject newTileClass = RoadTileManager.Grass;
					GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset-transform.position.y, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot.transform);
                    WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
                    MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because of probable hole\n";
				}
			}
		}
	}
}
