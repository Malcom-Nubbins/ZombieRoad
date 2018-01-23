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
			if (!hit[i].collider)
			{
				bool bProbablyHole = true;
				for (int j = 0; j < hitPlus.Length; j += 2)
				{
					hitPlus[j] = RaycastHitNull;
					int k = 1;
					Vector3 RayLoc = transform.position + new Vector3(k * Xoffset(j) + Xoffset(i), 500, k * Zoffset(j) + Zoffset(i));
					Vector3 RayLocNoY = new Vector3(RayLoc.x, 0, RayLoc.z);
					while (!hitPlus[j].collider && (Vector3.Distance(CachedPlayerPosition, RayLocNoY) < (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100)))
					{
						Physics.Raycast(RayLoc, new Vector3(0, -1), out hitPlus[j], Mathf.Infinity, LayerMask.GetMask("Road"));
						k++;
						RayLoc = transform.position + new Vector3(k * Xoffset(j) + Xoffset(i), 500, k * Zoffset(j) + Zoffset(i));
						RayLocNoY.x = RayLoc.x;
						RayLocNoY.z = RayLoc.z;
					}
					if (hitPlus[j].collider && hitPlus[j].collider.GetComponent<RoadGenerator>().Exit.Length < 8) hitPlus[j].collider.GetComponent<RoadGenerator>().RefreshExits();

					MySpecificDebug += (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) + "\n";

					//if (!((RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) < 20 && (Vector3.Distance(CachedPlayerPosition, RayLocNoY) > (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100))))
					//{
					//	MySpecificDebug += "Nearby boundary hit while hole scanning\n";
					//	bProbablyHole = true;
					//}
					if ((!((RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance) - Vector3.Distance(CachedPlayerPosition, transform.position) < 20 && (Vector3.Distance(CachedPlayerPosition, RayLocNoY) > (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100)))) && (!hitPlus[j].collider || !hitPlus[j].collider.GetComponent<DisabledRoadGenerator>()))
					{
						bProbablyHole = false;
					}
				}
				if (bProbablyHole)
				{
					GameObject newTileClass = RoadTileManager.Grass;
					GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i), newTileClass.GetComponent<RoadGenerator>().YOffset-transform.position.y, Zoffset(i)), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot.transform);
					MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because of probable hole\n";
				}
			}
		}
	}
}
