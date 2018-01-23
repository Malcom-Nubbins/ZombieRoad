using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSmokeFX : MonoBehaviour {

    public GameObject darkSmokeParticleSystemPrefab;
    public GameObject lightSmokeParticleSystemPrefab;
    public GameObject fireParticleSystemPrefab;

    public Transform smokeEffectEngine;

    private ParticleSystem lightSmokeParticleSystem;
    private ParticleSystem darkSmokeParticleSystem;
    private ParticleSystem fireParticleSystem;

    BaseVehicleClass vehicle;
	// Use this for initialization
	void Start () {
        vehicle = GetComponent<BaseVehicleClass>();

        GameObject darkSmokeEffect = Instantiate(darkSmokeParticleSystemPrefab, Vector3.zero, Quaternion.identity, smokeEffectEngine);
        darkSmokeEffect.transform.localPosition = Vector3.zero;

        GameObject lightSmokeEffect = Instantiate(lightSmokeParticleSystemPrefab, Vector3.zero, Quaternion.identity, smokeEffectEngine);
        lightSmokeEffect.transform.localPosition = Vector3.zero;

        GameObject fireEffect = Instantiate(fireParticleSystemPrefab, Vector3.zero, Quaternion.identity, smokeEffectEngine);
        fireEffect.transform.localPosition = Vector3.zero;

        darkSmokeParticleSystem = darkSmokeEffect.GetComponent<ParticleSystem>();
        lightSmokeParticleSystem = lightSmokeEffect.GetComponent<ParticleSystem>();
        fireParticleSystem = fireEffect.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {

        if(vehicle.health > vehicle.GetMaxHealth() / 2)
        {
            if (lightSmokeParticleSystem.isPlaying)
                lightSmokeParticleSystem.Stop();

            if (darkSmokeParticleSystem.isPlaying)
                darkSmokeParticleSystem.Stop();

            if (fireParticleSystem.isPlaying)
                fireParticleSystem.Stop();
        }
        else
        {
            if (vehicle.health <= vehicle.GetMaxHealth() / 2 && vehicle.health > vehicle.GetMaxHealth() / 3)
            {
                if (!lightSmokeParticleSystem.isPlaying)
                    lightSmokeParticleSystem.Play();
            }
            else if (vehicle.health <= vehicle.GetMaxHealth() / 3 && vehicle.health > 0.5f)
            {
                if (lightSmokeParticleSystem.isPlaying)
                    lightSmokeParticleSystem.Stop();

                if (!darkSmokeParticleSystem.isPlaying)
                    darkSmokeParticleSystem.Play();
            }
            else
            {
                if (darkSmokeParticleSystem.isPlaying)
                    darkSmokeParticleSystem.Stop();

                if (!lightSmokeParticleSystem.isPlaying)
                    lightSmokeParticleSystem.Play();

                if (!fireParticleSystem.isPlaying)
                    fireParticleSystem.Play();
            }
        }
	}
}
