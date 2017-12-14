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

	override public void Extend()
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
					while (!hitPlus[j].collider && (Vector3.Distance(CachedPlayerPosition, RayLocNoY)/*Vector3.Distance(RoadTileManager.checkpoint.transform.position, RayLocNoY)*/ < /*(RoadTileManager.checkpoint.gameObject.transform.localScale.x / 2) + 20*/(RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 100)))
					{
						//Debug.Log(gameObject.name + gameObject.transform.position + " \n Distance checkpoint->ray: " + Vector3.Distance(RoadTileManager.checkpoint.transform.position, RayLocNoY) + " < " + ((RoadTileManager.checkpoint.gameObject.transform.localScale.x / 2) + 20) + " \n Distance player->ray: " + Vector3.Distance(CachedPlayerPosition, RayLocNoY) + " < " + (RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().CullDistance + 20));
						Physics.Raycast(RayLoc, new Vector3(0, -1), out hitPlus[j], Mathf.Infinity, LayerMask.GetMask("Road")/*1 << 9*/);
						//MySpecificDebug += "Considering " + (Direction)i + ", scanning " + (Direction)j + " hit " + (hitPlus[j].collider ?  hitPlus[j].collider.name + hitPlus[j].collider.transform.position : "nothing "+RayLocNoY) + "\n";
						k++;
						RayLoc = transform.position + new Vector3(k * Xoffset(j) + Xoffset(i), 500, k * Zoffset(j) + Zoffset(i));
						RayLocNoY.x = RayLoc.x;
						RayLocNoY.z = RayLoc.z;
					}
					if (hitPlus[j].collider && hitPlus[j].collider.GetComponent<RoadGenerator>().Exit.Length < 8) hitPlus[j].collider.GetComponent<RoadGenerator>().RefreshExits();
					if (!hitPlus[j].collider ||/* hitPlus[j].collider &&*/ !hitPlus[j].collider.GetComponent<DisabledRoadGenerator>())
					{
						//MySpecificDebug += "Considering " + (Direction)i + ", scanning " + (Direction)j + (hitPlus[j].collider ? " hit " + hitPlus[j].collider.name + hitPlus[j].collider.transform.position : " reached oob") + ", PROBABLY NOT HOLE" + "\n";
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
