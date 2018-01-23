using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesPileIn : MonoBehaviour
{
    ZombieDetector zombieDetector;
    
	void Start()
    {
        zombieDetector = GetComponent<ZombieDetector>();
        GetComponentInParent<Health>().onDeath += () =>
        {
            enabled = true;
            GetComponentInParent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePosition;
        };
        enabled = false;
	}
	
	void Update()
    {
		foreach (GameObject zombie in zombieDetector.GetNearbyZombies(1))
        {
            if (!zombie.GetComponent<PileIn>())
            {
                PileIn pileIn = zombie.AddComponent<PileIn>();
                pileIn.target = transform.position;
            }
        }
	}
}
