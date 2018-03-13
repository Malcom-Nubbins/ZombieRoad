using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenHitByVehicle : MonoBehaviour
{
    Health health;
    AudioSource zombieKillSource;
    public AudioClip zombieHit;

    void Start()
    {
        health = GetComponent<Health>();
    }
	
    void OnTriggerStay(Collider collider)
    {
        OnTriggerEnter(collider);
    }

    void OnTriggerEnter(Collider collider)
    {
		if (collider.CompareTag("Vehicle"))
        {
            BaseVehicleClass vehicle = collider.GetComponentInParent<BaseVehicleClass>();
            if (vehicle.speed > 5.0f)
            {
                OnHitByVehicle(vehicle);
            }
        }
    }

    protected virtual void OnHitByVehicle(BaseVehicleClass vehicle)
    {
        zombieKillSource = vehicle.GetComponentInChildren<AudioSource>();
        if(health.health > 0)
        {
            if(!zombieKillSource.isPlaying)
            {
                zombieKillSource.PlayOneShot(zombieHit);
            }
        }

        
        FallOnDeath fallOnDeath = GetComponent<FallOnDeath>();
        if (fallOnDeath)
        {
            Vector3 toVehicle = vehicle.transform.position - transform.position;
            fallOnDeath.SetAxisToRotateAround(Vector3.Cross(-toVehicle.normalized, Vector3.up));
        }
        if (health)
        {
            health.health = 0;
        }
    }
}
