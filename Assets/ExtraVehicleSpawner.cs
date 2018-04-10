using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraVehicleSpawner : MonoBehaviour
{
    public float spawnChance = 0.5f;

	void Start()
    {
        TilePosition pos = new TilePosition(transform.position);
        foreach (TilePosition neighborOffset in new TilePosition[] { new TilePosition(-1, 0), new TilePosition(1, 0), new TilePosition(0, -1), new TilePosition(0, 1) })
        {
            if (Random.value < spawnChance)
            {
                WorldTile tile = WorldTileManager.instance.GetTile(pos + neighborOffset);
                if (tile)
                {
                    for (int i = 0; i < 10; i++)//eventually will spawn a vehicle
                    {
                        if (!tile.GetComponent<VehicleGenerator>())
                        {
                            tile.gameObject.AddComponent<VehicleGenerator>();
                        }
                    }
                }
            }
        }
	}
}
