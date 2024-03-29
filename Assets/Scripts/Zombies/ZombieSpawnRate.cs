﻿using UnityEngine;
using ZR.Chkpnt;

public class ZombieSpawnRate : MonoBehaviour
{
	public float zombiesPerSecond = 1.0f;
	public BaseCheckpoint checkpoint;

	public delegate void Spawn(int numZombies);
	public event Spawn OnReadyToSpawn;

	float zombiesToSpawn = 0;

	void OnEnable()
	{
		checkpoint.OnCheckpointExtend += OnCheckpointExtend;
	}

	void OnDisable()
	{
		checkpoint.OnCheckpointExtend -= OnCheckpointExtend;
	}
	
	void Update()
	{
		zombiesToSpawn += zombiesPerSecond * Time.deltaTime;
		if (zombiesToSpawn >= 1.0f)
		{
			TrySpawn(Mathf.FloorToInt(zombiesToSpawn));
		}
	}

	void TrySpawn(int num)
	{
		if (OnReadyToSpawn != null)
		{
			OnReadyToSpawn(num);
			zombiesToSpawn -= num;
		}
	}

	void OnCheckpointExtend()
	{
		zombiesPerSecond += 0.2f;
	}
}
