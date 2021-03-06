﻿using System.Collections.Generic;
using UnityEngine;

namespace ZR.Road.Buildings
{
	public class GrassPopulator : MonoBehaviour
	{
		int FenceDir = -1;

		// awake happens too soon like a bitch
		void Update()
		{
			RoadGenerator rg = gameObject.GetComponent<RoadGenerator>();
			int r = Random.Range(0, 100);

			if (r < GrassPopulationManager.instance.ChanceFences)
			{
				int f = Random.Range(0, GrassPopulationManager.Fences.Length);
				GameObject FenceTemplate = GrassPopulationManager.Fences[f];
				int d = 2 * Random.Range(0, 4);

				rg.DoHits();
				WorldTile tiled = rg.GetNeighbours()[d];
				if (tiled && tiled.GetComponent<GrassPopulator>() && tiled.GetComponent<GrassPopulator>().FenceDir == RoadGenerator.Wrap0to7(d - 4))
				{
					//Debug.Log("fence collision prevented");
				}
				else
				{
					GameObject fence = Instantiate(FenceTemplate, transform.position + new Vector3((RoadGenerator.Xoffset(d) * WorldTileManager.TILE_SIZE) / 2, 0, (RoadGenerator.Zoffset(d) * WorldTileManager.TILE_SIZE) / 2), Quaternion.identity, transform);
					fence.transform.Rotate(0, d * 45 - 90, 0);
					FenceDir = d;
				}
			}

			int t = Random.Range(0, GrassPopulationManager.instance.MaximumTreeDensity);
			List<Vector3> trees = new List<Vector3>();
			trees.Clear();
			float x, y;
			x = y = 0.0f;
			bool bCollides = true;

			for (int i = 0; i < t; ++i)
			{
				bCollides = true;
				while (bCollides)
				{
					x = Random.Range(-9.0f, 9.0f);
					y = Random.Range(-9.0f, 9.0f);
					bCollides = false;
					foreach (Vector3 treepos in trees)
						if ((treepos - new Vector3(x, 0, y)).magnitude < 4.0f)
							bCollides = true;
				}
				trees.Add(new Vector3(x, 0, y));
				Instantiate(GrassPopulationManager.Tree, transform.position + new Vector3(x, 0, y), Quaternion.identity, transform);
			}

			enabled = false;
		}
	}
}
