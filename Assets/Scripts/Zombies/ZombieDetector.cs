using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZombieDetector : MonoBehaviour
{
    List<GameObject> nearbyZombies = new List<GameObject>();
    List<GameObject> sortedZombies = new List<GameObject>();
    GameObject nearestZombie = null;
    

	void Start()
    {
		
	}
	
	void Update()
    {
        int i = 0;
        nearbyZombies.RemoveAll((GameObject zombie) => !zombie || zombie.GetComponent<Health>().health <= 0);//remove zombies that have been destroyed without calling OnTriggerExit, also removes zombies with 0 HP
            //Find nearest zombie
        nearestZombie = null;
        float nearestDistance = float.PositiveInfinity;
        //Debug.Log(nearbyZombies.Count);
        //Debug.Log("Contains: "+nearbyZombies[0].name);
        sortedZombies = nearbyZombies.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();


		if (sortedZombies.Count > 0)
		{
			if (sortedZombies[0].GetComponent<Health>().health > 0) // make sure nearest zombie is alive
			{
				nearestZombie = sortedZombies[0];
				nearestDistance = Vector3.Distance(transform.position, sortedZombies[0].transform.position);
				//Debug.Log(nearestDistance);
			}
			else
			{
				sortedZombies.RemoveAt(0);
				Debug.Log("REMOVING DEAD ZOMBIE");
			}
		}


        /*
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
        */

	}

    public GameObject GetNearestZombie()
    {
        return nearestZombie;
    }

    public GameObject[] GetSortedZombies(int size, float range)
    {
        sortedZombies = nearbyZombies.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
       // Debug.Log("SIZE OF ARRAY BEFORE REMOVING ZOMBIES OUT OF RANGE" + sortedZombies.Count());
        sortedZombies.RemoveAll(x => Vector2.Distance(transform.position, x.transform.position) > range);
        //Debug.Log("SIZE OF ARRAY AFTETR REMOVING ZOMBIES OUT OF RANGE" + sortedZombies.Count());
//        for(int x = 0; x< sortedZombies.Count(); x++)
//       {
//            Debug.Log("zombieE: " + x + "distance: " + Vector2.Distance(transform.position, sortedZombies[x].transform.position));
//        }
        if (sortedZombies.Count >= size)
        {
            sortedZombies.GetRange(0, size);
            List<GameObject> tmpZombies = sortedZombies.GetRange(0, size);
            return tmpZombies.ToArray();
        }
        else
        {
            return sortedZombies.ToArray();
        }

    }

    public GameObject[] GetNearbyZombies()
    {
        return nearbyZombies.ToArray();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Zombie"))
        { 
            if (collider.GetComponentInParent<Health>() != null)
            {
                nearbyZombies.Add(collider.GetComponentInParent<Health>().gameObject);
            }

        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Zombie"))
        {
            if (collider.GetComponentInParent<Health>() != null)
            {
                nearbyZombies.Remove(collider.GetComponentInParent<Health>().gameObject);
            }
        }
    }
}
