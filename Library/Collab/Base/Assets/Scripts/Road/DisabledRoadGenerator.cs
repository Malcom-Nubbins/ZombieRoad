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

	public Type TileClassification;

	override public void Extend(/*Checkpoint NewCheckpoint*/)/* { }
	public void Holes()// extend into holes*/
	{
		bHaveExpanded = false;
		if (Vector3.Distance(RoadTileManager.checkpoint.transform.position, transform.position) > (RoadTileManager.checkpoint.gameObject.transform.localScale.x / 2) + 20) return;

		bool bCanExtend = false;

		// if the Raycasts from last time have any no-hits, we might be able to expand, otherwise we should not need new Raycasts and can bail
		foreach (RaycastHit h in hit)
			if (!h.collider)
				bCanExtend = true;

		if (!bCanExtend) return;

		for (int i = 0; i < hit.Length; i++)
		{
			Physics.Raycast(transform.position + new Vector3(Xoffset(i), 500, Zoffset(i)), new Vector3(0, -1), out hit[i], Mathf.Infinity, 1 << 9);
			if (hit[i].collider && hit[i].collider.gameObject.GetComponent<RoadGenerator>().Exit.Length < 8) hit[i].collider.gameObject.GetComponent<RoadGenerator>().RefreshExits();
			if (!hit[i].collider) bCanExtend = true;
		}

		if (!bCanExtend) return; // if none of the new Raycasts didn't hit anything then we can't expand and can bail


		for (int i = 0; i < hit.Length; i += 2)
		{
			if (!hit[i].collider)
			{
				bool bProbablyHole = true;
				for (int j = 0; j < hitPlus.Length; j += 2)
				{
					int k = 1;
					Vector3 RayLoc = transform.position + new Vector3(k * Xoffset(j) + Xoffset(i), 500, k * Zoffset(j) + Zoffset(i));
					Vector3 RayLocNoY = new Vector3(RayLoc.x, 0, RayLoc.z);
					while (!hitPlus[j].collider && (Vector3.Distance(RoadTileManager.checkpoint.transform.position, RayLocNoY) < (RoadTileManager.checkpoint.gameObject.transform.localScale.x / 2) - 20))
					{
						Physics.Raycast(RayLoc, new Vector3(0, -1), out hitPlus[j], Mathf.Infinity, 1 << 9);
						k++;
						RayLoc = transform.position + new Vector3(k * Xoffset(j) + Xoffset(i), 500, k * Zoffset(j) + Zoffset(i));
						RayLocNoY.x = RayLoc.x;
						RayLocNoY.z = RayLoc.z;
					}
					if (hitPlus[j].collider && hitPlus[j].collider.GetComponent<RoadGenerator>().Exit.Length < 8) hitPlus[j].collider.GetComponent<RoadGenerator>().RefreshExits();
					if (!hitPlus[j].collider ||/* hitPlus[j].collider &&*/ !hitPlus[j].collider.GetComponent<DisabledRoadGenerator>()) bProbablyHole = false;
				}
				if (bProbablyHole)
				{
					GameObject newTileClass = RoadTileManager.Grass;
					GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i), newTileClass.GetComponent<RoadGenerator>().YOffset-transform.position.y, Zoffset(i)), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot.transform);
				}
			}
		}
	}
}
