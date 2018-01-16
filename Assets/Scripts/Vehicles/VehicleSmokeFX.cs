using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSmokeFX : MonoBehaviour {

    public GameObject smokeParticleSystemPrefab;

    public Transform smokeEffectEngine;
    private ParticleSystem smokeParticleSystem;

    BaseVehicleClass vehicle;
	// Use this for initialization
	void Start () {
        vehicle = GetComponent<BaseVehicleClass>();
        GameObject smokeEffect = Instantiate(smokeParticleSystemPrefab, Vector3.zero, Quaternion.identity, smokeEffectEngine);
        smokeEffect.transform.localPosition = Vector3.zero;

        smokeParticleSystem = smokeEffect.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (vehicle.health <= vehicle.GetMaxHealth() / 3)
        { 

            if(!smokeParticleSystem.isPlaying)
                smokeParticleSystem.Play();
        }
        else
        {
            if (smokeParticleSystem.isPlaying)
                smokeParticleSystem.Stop();
        }
	}
}
