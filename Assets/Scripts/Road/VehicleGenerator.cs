using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{
    public float spawnChance = 0.4f;
    public bool hasSpawned = false;

    RoadGenerator rg;
    
    void Start()
	{
        rg = gameObject.GetComponent<RoadGenerator>();
        if (rg.Exit.Length < 8) rg.RefreshExits();
        SpawnNonVehicles();
        if (Random.value < spawnChance)
        {
            TrySpawn();
        }
	}

    public bool TrySpawn()
    {
        if (hasSpawned) return false;
        Spawn();
        return true;
    }

    void Spawn()
    {
        if (UnlockManager.instance)
        {
            GameObject[] UnlockedCars = UnlockManager.instance.GetUnlockedItems(UnlockableType.VEHICLE);
            int index = Random.Range(0, UnlockedCars.Length);
            int dir = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.South;
            GameObject car = Instantiate(UnlockedCars[index], gameObject.transform.position + new Vector3((RoadGenerator.Xoffset(dir) * WorldTileManager.TILE_SIZE) / 4, 3, (RoadGenerator.Zoffset(dir) * WorldTileManager.TILE_SIZE) / 4), gameObject.transform.rotation);
            //if (RoadTileManager.bDebugEnv) rg.MySpecificDebug += "generated vehicle @ " + car.transform.position + "\n";

            if (RoadTileManager.bMainMenu) car.GetComponent<BaseVehicleClass>().health = 0;
        }
        hasSpawned = true;
    }

    void SpawnNonVehicles()
    {
        GameObject player = RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>().target;
        
        int i = rg.Exit[(int)RoadGenerator.Direction.North] ? (int)RoadGenerator.Direction.West : (int)RoadGenerator.Direction.South;

        if (rg.CullingExempt) return;

        int r = Random.Range(0, 10);
        if (r == 1)
        {
            r = Random.Range(0, 2);
            bool bSpinBench = false;
            if (r == 1) { i -= 4; bSpinBench = true; } // place bench east or north instead of west or south

            GameObject benchTemplate = Resources.Load<GameObject>("Prefabs/Destructable Scenery/bench");
            GameObject bench = Instantiate(benchTemplate, gameObject.transform.position + new Vector3((RoadGenerator.Xoffset(i) * WorldTileManager.TILE_SIZE) / 2.25f + (RoadGenerator.Xoffset(RoadGenerator.Wrap0to7(i - 2) * (int)WorldTileManager.TILE_SIZE)) / 5, 1.6f, (RoadGenerator.Zoffset(i) * WorldTileManager.TILE_SIZE) / 2.25f + (RoadGenerator.Zoffset(RoadGenerator.Wrap0to7(i - 2)) * WorldTileManager.TILE_SIZE) / 5), gameObject.transform.rotation);
            if (bSpinBench) bench.transform.Rotate(0, 180, 0);
            hasSpawned = true;
        }

        r = Random.Range(0, 100);

        bool bInVehicle = player.GetComponent<BaseVehicleClass>();

        if ((RoadTileManager.bMainMenu || bInVehicle) && r > 100 - RoadTileManager.instance.ChanceBarrier)
        {
            GameObject[] RoadBlocks = Resources.LoadAll<GameObject>("Prefabs/Destructable Scenery/Fences/Road Barriers");
            r = Random.Range(0, RoadBlocks.Length);
            Instantiate(RoadBlocks[r], transform.position, transform.rotation, transform);hasSpawned = true;
        }
        
    }
}