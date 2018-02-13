using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class PlayerDeath : MonoBehaviour
{
    float gameOverCountdown = 2.5f;
    bool dead = false;
    bool showingAd = false;

	void Start()
    {
        GetComponent<Health>().onDeath += OnDeath;
	}
	
	void Update()
    {
        if (dead)
        {

            gameOverCountdown -= Time.deltaTime;
            if (gameOverCountdown <= 0)
            {
                if(!showingAd)
                {
                    Debug.Log("call this only once ok unity?");
                    GetComponent<AdsScript>().PlayAdOnDeath();
                    showingAd = true;
                }


                if (UnlockManager.instance.GetLockedItemCount() > 0)
                {
                    Scenes.instance.LoadScene(Scenes.Scene.UNLOCK);
                }
                else
                {
                    Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
                }
            }
        }
	}

    void OnDeath()
    {
        Movement move = GetComponent<Movement>();
        move.centerOfMass.Set(0.0f, 6.5f, 0.0f);
        move.speed = 0.0f;
        move.rotationAngle = 0.0f;

        GameObject cameraTarget = Camera.main.GetComponent<FollowCamera>().target;
        if (cameraTarget != gameObject)
        {
            BaseVehicleClass vehicle = cameraTarget.GetComponent<BaseVehicleClass>();
            if (vehicle)
            {
                vehicle.health = 0;
            }
        }

        dead = true;
    }

    public void killPlayer()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        playerHealth.health = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        //only collide with zombies
        if (collision.gameObject.tag != "Zombie")
        {
            return;
        }

        GameObject zombie = collision.gameObject;

        //if zombie is dead, don't kill the player
        if (zombie.GetComponent<Health>().health <= 0)
        {
            return;
        }

        //killPlayer();
    }
}
