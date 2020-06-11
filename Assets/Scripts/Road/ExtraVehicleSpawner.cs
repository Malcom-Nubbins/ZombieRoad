using UnityEngine;

namespace ZR.Road
{
	public class ExtraVehicleSpawner : MonoBehaviour
	{
		void Start()
		{
			if (Random.value < gameObject.GetComponent<Unlockable>().Price / 100.0f)
			{
				TilePosition pos = new TilePosition(transform.position);
				//for each neighbour tile
				for (int x = -1; x <= 1; ++x)
				{
					for (int z = -1; z <= 1; ++z)
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
}
