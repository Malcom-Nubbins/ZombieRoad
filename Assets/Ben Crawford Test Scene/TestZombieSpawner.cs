using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombieSpawner : MonoBehaviour {

    public GameObject prefab;
	public GameObject FollowCamera;
    public float timeBetweenSpawns = 2.0f;
    float timer = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<SeekPlayer>().FollowCamera = FollowCamera;
            timer += timeBetweenSpawns;
        }
	}
}
