using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
	public bool spawnAllAtOnce;
	float activeRadius = 150;
	static FollowCamera followCamera;
	GameObject zombiePrefab;
	int numberToSpawn = 5;
	float timeBetweenSpawns = 5;
	float radius;
	float timer = 0;
	
	void Start()
	{
        int level = Checkpoint.GetLevel();
        float chanceOfSpawning = Mathf.Lerp(0.05f, 0.3f, Mathf.Clamp01(level / 20.0f));
		if (!(Random.value < chanceOfSpawning))
		{
			Destroy(gameObject);
			return;
		}
		
		if (followCamera==null) followCamera = FindObjectOfType<FollowCamera>();

		SphereCollider sphere = GetComponent<SphereCollider>();
		if (sphere)
		{
			radius = sphere.radius;
			Destroy(sphere);
		}

        if (UnlockManager.instance == null)
        {
            Destroy(this);
        }
		
		GameObject[] zombies = UnlockManager.instance.GetUnlockedItems(UnlockableType.ZOMBIE);
		zombiePrefab = zombies[Random.Range(0, zombies.Length)];
        //Debug.Log(zombiePrefab);

        int minSpawn = Mathf.RoundToInt(Mathf.Lerp(1, 10, Mathf.Clamp01(level / 20.0f)));
        int maxSpawn = Mathf.RoundToInt(Mathf.Lerp(2, 15, Mathf.Clamp01(level / 20.0f)));
        numberToSpawn = Random.Range(minSpawn, maxSpawn);
        
        if (spawnAllAtOnce)
        {
            timeBetweenSpawns = 0;
        }
        else
        {
            float minTimeBetweenSpawns = Mathf.Lerp(5.0f, 1.5f, Mathf.Clamp01(level / 20.0f));
            float maxTimeBetweenSpawns = 5.0f;
            timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
        }
	}
	
	void Update()
	{
		if (Vector3.Distance(transform.position, RoadTileManager.bMainMenu ? followCamera.MainMenuZombieTarget.transform.position : followCamera.target.transform.position) < activeRadius)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				timer += timeBetweenSpawns;
				Spawn();
				numberToSpawn--;
				if (numberToSpawn <= 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}

	void Spawn()
	{
		GameObject zombie = Instantiate(zombiePrefab);
		Vector2 xzOffset = Random.insideUnitCircle * radius;
		zombie.transform.position = transform.position + new Vector3(xzOffset.x, 0, xzOffset.y);
	}
}
