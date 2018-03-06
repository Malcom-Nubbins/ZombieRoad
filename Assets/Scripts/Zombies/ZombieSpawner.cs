using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[HelpURL("https://cdn.discordapp.com/attachments/368486715779710987/420185975432544257/unknown.png")]
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
    float timeBetweenSpawnAttempts = 1.0f;//dont try to spawn every frame
    float spawnAttemptTimer = 0;

    public bool debugSpawnEveryFrame;
	
	void Start()
	{
        int level = Checkpoint.GetLevel();
        float chanceOfSpawning = Mathf.Lerp(0.05f, 0.3f, Mathf.Clamp01(level / 20.0f));
		if (!(Random.value < chanceOfSpawning) && !debugSpawnEveryFrame)
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

        if (debugSpawnEveryFrame)
        {
            numberToSpawn = 10000;
            timeBetweenSpawns = 1.0f / 30.0f;
        }
	}
	
	void Update()
	{
		if (Vector3.Distance(transform.position, followCamera.target.transform.position) < activeRadius)
		{
			if (timer <= 0)
			{
                TrySpawn();
			}
            else
            {
			    timer -= Time.deltaTime;
            }
		}
	}

    void TrySpawn()
    {
        if (spawnAttemptTimer <= 0)
        {
            spawnAttemptTimer += timeBetweenSpawnAttempts;
            Ray ray = new Ray(transform.position - Vector3.up * 0.5f, Vector3.up);
            float rayLength = 5;
            bool areaClear = !Physics.Raycast(ray, rayLength);
            if (debugSpawnEveryFrame)
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayLength,
                    areaClear ? Color.green : Color.red, 0.5f);
            }
            if (areaClear)
            {
                Spawn();
            }
        }
        else
        {
            spawnAttemptTimer -= Time.deltaTime;
        }
    }

	void Spawn()
	{
		GameObject zombie = Instantiate(zombiePrefab);
		Vector2 xzOffset = Random.insideUnitCircle * radius;
		zombie.transform.position = transform.position + new Vector3(xzOffset.x, 0, xzOffset.y);
        zombie.transform.localRotation = Quaternion.identity;

        timer += timeBetweenSpawns;
        numberToSpawn--;
        if (numberToSpawn <= 0)
        {
            Destroy(gameObject);
        }
    }
}
