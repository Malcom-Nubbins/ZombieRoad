using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    public ZombieSpawnRate spawnRate;
    public FollowCamera followCamera;

    int zombiesToSpawn = 0;
    float spawnRadius = 80.0f;

    void OnEnable()
    {
        spawnRate.OnReadyToSpawn += SpawnRate_OnReadyToSpawn;
    }

    void OnDisable()
    {
        spawnRate.OnReadyToSpawn -= SpawnRate_OnReadyToSpawn;
    }

    void Start()
    {

    }

    void Update()
    {
        if (zombiesToSpawn > 0)
        {
            TrySpawn();
        }
    }

    private void SpawnRate_OnReadyToSpawn(int numZombies)
    {
        zombiesToSpawn += numZombies;
    }

    void TrySpawn()
    {
        Transform playerTransform = followCamera.target.transform;
        //in front of player plus random amount
        Vector2 direction = (new Vector2(playerTransform.forward.x, playerTransform.forward.z).normalized
            + Random.insideUnitCircle.normalized * 0.5f).normalized;
        Vector3 spawnPos = playerTransform.position;
        spawnPos.y = 0.5f;
        spawnPos.x += direction.x * spawnRadius;
        spawnPos.z += direction.y * spawnRadius;

        bool canSpawn = !Physics.CheckSphere(spawnPos + Vector3.up * 0.5f, 0.5f);

        if (canSpawn)
        {
            Spawn(spawnPos);
        }
    }

    void Spawn(Vector3 position)
    {
        GameObject[] unlockedZombies = UnlockManager.instance.GetUnlockedItems(UnlockableType.ZOMBIE);
        GameObject zombiePrefab = unlockedZombies[Random.Range(0, unlockedZombies.Length)];

        GameObject zombie = Instantiate(zombiePrefab, position, Quaternion.identity);

        zombiesToSpawn--;
    }
}
