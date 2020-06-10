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

	TilePosition tp;
	Vector3 v3;
	private void Start()
	{
		tp = new TilePosition();
		v3 = new Vector3();
	}
	bool bProbablyHole;

	public override void Extend(bool bForceOOBExtension = false)
	{
		if (!ShouldExtend())
			return;

		for (int i = 0; i < hit.Length; i += 2)
		{
			if (!hit[i])
			{
				bProbablyHole = true;
				for (int j = 0; j < hitPlus.Length; j += 2)
				{
					hitPlus[j] = null;
					int k = 1;
					tp.x = k * Xoffset(j) + Xoffset(i);
					tp.z = k * Zoffset(j) + Zoffset(i);
					TilePosition RayLoc = GetTilePosition() + tp;
					while (!hitPlus[j])
					{
						hitPlus[j] = WorldTileManager.instance.GetTile(RayLoc);
						k++;
						tp.x = k * Xoffset(j) + Xoffset(i);
						tp.z = k * Zoffset(j) + Zoffset(i);
						RayLoc = GetTilePosition() + tp;
						if (Vector3.Distance(RayLoc.GetWorldPosition(), gameObject.transform.position) > RoadTileManager.checkpoint.FollowCamera.CullDistance + 100)
							break; 
					}
					if (hitPlus[j] && hitPlus[j].GetComponent<RoadGenerator>().Exit.Length < 8)
						hitPlus[j].GetComponent<RoadGenerator>().RefreshExits();

					if (RoadTileManager.bDebugEnv)
						MySpecificDebug += Time.fixedTime + " hitPlus " + (Direction)j + " concluded with " + (hitPlus[j]?hitPlus[j].gameObject.name + " (" + hitPlus[j].gameObject.transform.position + ")" : "boundary")+"\n";

					if (hitPlus[j] && !hitPlus[j].GetComponent<DisabledRoadGenerator>())
					{
						bProbablyHole = false;
					}
				}
				if (bProbablyHole && bProbablyHoleLastFrame)
				{
					GameObject newTileClass = RoadTileManager.Grass;
					v3.x = Xoffset(i) * WorldTileManager.TILE_SIZE;
					v3.y = newTileClass.GetComponent<RoadGenerator>().YOffset - transform.position.y;
					v3.z = Zoffset(i) * WorldTileManager.TILE_SIZE;
					GameObject newTile = Instantiate(newTileClass, transform.position + v3, Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot);
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
