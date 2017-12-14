using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidFX : MonoBehaviour
{
    public GameObject skidParticleSystemPrefab;

    public Transform[] skidEffectWheels;
    ParticleSystem[] skidParticleSystems;
    bool particleSystemsEnabled;

    BaseVehicleClass vehicle;

	void Start()
    {
        vehicle = GetComponent<BaseVehicleClass>();

        List<ParticleSystem> tempParticleSystems = new List<ParticleSystem>();
        foreach (Transform wheel in skidEffectWheels)
        {
            GameObject skidEffect = Instantiate(skidParticleSystemPrefab, Vector3.zero, Quaternion.identity, wheel);
            skidEffect.transform.localPosition = Vector3.zero;
            ParticleSystem skidParticleSystem = skidEffect.GetComponent<ParticleSystem>();
            tempParticleSystems.Add(skidParticleSystem);
        }
        skidParticleSystems = tempParticleSystems.ToArray();
        SetParticleSystemsEnabled(false);
	}

    void SetParticleSystemsEnabled(bool enabled)
    {
        foreach (ParticleSystem ps in skidParticleSystems)
        {
            if (enabled)
            {
                ps.Play();
            }
            else
            {
                ps.Pause();
            }
        }
        particleSystemsEnabled = enabled;
    }
	
	void Update()
    {
        // enable if left or right pressed, but not both
        bool particleSystemsDesired = (Movement.InputLeft() ^ Movement.InputRight()) && (vehicle.GetDriver() != null);
        if (particleSystemsDesired != particleSystemsEnabled)
        {
            SetParticleSystemsEnabled(particleSystemsDesired);
        }
	}

}
