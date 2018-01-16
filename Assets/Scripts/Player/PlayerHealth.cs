using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health {

    private Slider _healthSlider;

    private float _lastHitTime = 0.0f;

	// Use this for initialization
	void Start () {
        health = startingHealth;
        _healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        _healthSlider.maxValue = startingHealth;
        onDeath += PlayerHealth_onDeath;
	}

    private void PlayerHealth_onDeath()
    {
        GetComponent<PlayerDeath>().killPlayer();
    }

    private void OnEnable()
    {
       // health = startingHealth;
    }

    // Update is called once per frame
    void Update () {
        if(!gameObject.GetComponent<DisableVehicle>().followCamera)
        {
            return;
        }

        if (transform.position.y < -10)
        {
            health = 0;
           // Destroy(gameObject);
        }

        _healthSlider.value = health;
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_lastHitTime > 0.0f)
        {
            _lastHitTime -= Time.deltaTime;
            return;
        }
        if(collision.gameObject.tag != "Zombie")
        {
            return;
        }

        GameObject zombie = collision.gameObject;

        if (zombie.GetComponent<Health>().health <= 0)
        {
            return;
        }

        health -= 0.5f;
        _lastHitTime = 1.0f;
    }
}
