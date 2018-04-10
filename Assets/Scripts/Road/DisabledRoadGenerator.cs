using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledRoadGenerator : RoadGenerator
{
	public enum Type : int
	{
		Pavement,
		Grass,
		Water,
		xInvalid
	}

	public Type TileClassification;

	bool bProbablyHoleLastFrame = false;

	override public void Extend(bool bForceOOBExtension = false)
	{
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
					while (!hitPlus[j])
					{
						hitPlus[j] = WorldTileManager.instance.GetTile(RayLoc);
						k++;
						RayLoc = GetTilePosition() + new TilePosition(k * Xoffset(j) + Xoffset(i), k * Zoffset(j) + Zoffset(i));
						if (Vector3.Distance(RayLoc.GetWorldPosition(), gameObject.transform.position) > RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100) break; 
					}
					if (hitPlus[j] && hitPlus[j].GetComponent<RoadGenerator>().Exit.Length < 8) hitPlus[j].GetComponent<RoadGenerator>().RefreshExits();

					if (RoadTileManager.bDebugEnv) MySpecificDebug += Time.fixedTime + " hitPlus " + (Direction)j + " concluded with " + (hitPlus[j]?hitPlus[j].gameObject.name + " (" + hitPlus[j].gameObject.transform.position + ")" : "boundary")+"\n";

					if (hitPlus[j] && !hitPlus[j].GetComponent<DisabledRoadGenerator>())
					{
						bProbablyHole = false;
					}
				}
				if (bProbablyHole && bProbablyHoleLastFrame)
				{
					GameObject newTileClass = RoadTileManager.Grass;
					GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset-transform.position.y, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot.transform);
					WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
					if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because of probable hole\n";

				}
				bProbablyHoleLastFrame = bProbablyHole;
			}
			else
				if (RoadTileManager.bDebugEnv) MySpecificDebug += Time.fixedTime + " hit     " + (Direction)i + " concluded immediately with " + hit[i].gameObject.name + " (" + hit[i].gameObject.transform.position + ")\n";
		}
	}
}
