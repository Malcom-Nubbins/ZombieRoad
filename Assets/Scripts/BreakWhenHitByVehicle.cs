using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWhenHitByVehicle : DieWhenHitByVehicle
{
    public GameObject breakEffectPrefab;
    Rigidbody rb;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	void Update()
    {
		
	}

    protected override void OnHitByVehicle(BaseVehicleClass vehicle)
    {
        base.OnHitByVehicle(vehicle);

        if (breakEffectPrefab)
        {
            GameObject effect = Instantiate(breakEffectPrefab, transform.position + Vector3.up * 1.5f, transform.rotation);

            Vector3 fromVehicle = (transform.position - vehicle.transform.position).normalized;
            fromVehicle.y = 0;
            fromVehicle.Normalize();
            effect.transform.rotation = Quaternion.LookRotation(fromVehicle, Vector3.up);

            Destroy(gameObject);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
            Destroy(this);//remove script, doesn't need to be called again
        }
    }
}
