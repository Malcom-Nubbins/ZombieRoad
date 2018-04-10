using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWhenHitByVehicle : DieWhenHitByVehicle
{
    public GameObject bloodEffectPrefab;

    protected override void OnHitByVehicle(BaseVehicleClass vehicle)
    {
        base.OnHitByVehicle(vehicle);
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);

        Vector3 fromVehicle = (transform.position - vehicle.transform.position).normalized;
        fromVehicle.y = 0;
        fromVehicle.Normalize();
        bloodEffect.transform.rotation = Quaternion.LookRotation(fromVehicle, Vector3.up);

        GetComponent<Health>().health = 0;
        Destroy(gameObject);
    }
}
