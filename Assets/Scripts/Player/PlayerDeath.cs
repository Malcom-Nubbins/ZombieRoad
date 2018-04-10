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
	AudioSource PlayerSource;
	public AudioClip playerDeathSound;

	void Start()
	{
		GetComponent<Health>().onDeath += OnDeath;
		PlayerSource = GetComponent<AudioSource>();
	}
	
	void Update()
	{
		if (dead)
		{

			gameOverCountdown -= Time.deltaTime;
			if (gameOverCountdown <= 0)
			{
				if(Advertisement.isInitialized)
				{
					if(!showingAd)
					{
						int nextAdCountdown = PlayerPrefs.GetInt("adCountdown");

						if(nextAdCountdown == 0)
						{
							nextAdCountdown = 3;
							PlayerPrefs.SetInt("adCountdown", nextAdCountdown);
							PlayerPrefs.Save();

							GetComponent<AdsScript>().PlayAdOnDeath();
							showingAd = true;
						}
						else
						{
							nextAdCountdown--;
							PlayerPrefs.SetInt("adCountdown", nextAdCountdown);
							PlayerPrefs.Save();

							showingAd = true; // prevent update from running again before unlock scene loads
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
				else
				{
					GetComponent<AdsScript>().InitAds();
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
		PlayerSource.PlayOneShot(playerDeathSound);
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
		if (!zombie.GetComponent<Health>() || zombie.GetComponent<Health>().health <= 0)
		{
			return;
		}

		//killPlayer();
	}
}
