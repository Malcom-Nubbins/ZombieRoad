using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenHitByVehicle : MonoBehaviour
{
    Health health;

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
        if (health)
        {
            health.health = 0;
        }
    }
}
