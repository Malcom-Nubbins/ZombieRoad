using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDetector : MonoBehaviour
{
    List<GameObject> nearbyZombies = new List<GameObject>();
    GameObject nearestZombie = null;

	void Start()
    {
		
	}
	
	void Update()
    {
        nearbyZombies.RemoveAll((GameObject zombie) => !zombie || zombie.GetComponent<Health>().health == 0);//remove zombies that have been destroyed without calling OnTriggerExit, also removes zombies with 0 HP
        //Find nearest zombie
        nearestZombie = null;
        float nearestDistance = float.PositiveInfinity;
        //Debug.Log(nearbyZombies.Count);
        //Debug.Log("Contains: "+nearbyZombies[0].name);
        foreach (GameObject zombie in nearbyZombies)
        {
            if (zombie.GetComponent<Health>().health > 0) // skips zombies with 0hp, should not be needed as zombies with 0 hp should be removed at this point.
            {
                float dist = Vector3.Distance(transform.position, zombie.transform.position);
                if (dist < nearestDistance)
                {
                    nearestZombie = zombie;
                    nearestDistance = dist;
                }
            }
        }
	}

    public GameObject GetNearestZombie()
    {
        return nearestZombie;
    }

    public GameObject[] GetNearbyZombies()
    {
        return nearbyZombies.ToArray();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Zombie"))
        {
            nearbyZombies.Add(collider.GetComponentInParent<Health>().gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Zombie"))
        {
            nearbyZombies.Remove(collider.GetComponentInParent<Health>().gameObject);
        }
    }
}
