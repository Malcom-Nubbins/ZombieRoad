using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraVehicleSpawner : MonoBehaviour
{
    public float spawnChance = 0.5f;

	void Start()
    {
        if (Random.value < spawnChance)
        {
            TilePosition pos = new TilePosition(transform.position);
            //for each neighbor tile
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    WorldTile tile = WorldTileManager.instance.GetTile(pos + new TilePosition(x, z));
                    if (tile)
                    {
                        //if it has a vehicle generator, then it can have vehicles spawned on it
                        VehicleGenerator vehicleGenerator = tile.GetComponent<VehicleGenerator>();
                        if (vehicleGenerator)
                        {
                            vehicleGenerator.TrySpawn();
                        }
                    }
                }
            }
        }
	}
}
