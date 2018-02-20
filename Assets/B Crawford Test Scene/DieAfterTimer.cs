using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterTimer : MonoBehaviour {
    public float timer = 5.0f;
	// Use this for initialization
	void Start () {
        GetComponent<Health>().onDeath += () =>
        {
            Destroy(this);
        };
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 100;
            GetComponent<Health>().health = 0;
        }
	}
}
