﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZR.Road;

public class BaseVehicleClass : Movement
{
	public float health = 50;
	private float _maxHealth;
	public float speedModifier = 1.0f;
	bool deadState = false;

	public float totalFuel = 50.0f;
	private float maxFuel;
	public float fuelConsumptionRate = 1.0f;
	public float GetFuelPercentage() { return totalFuel / maxFuel; }

	public static float timeBeforeNoButtonsPressedRemovesPlayer;
	GameObject _driver = null;
	float timeWithNoButtonsPressed = 0;

	float timeNotMoving;

	public float maxSpeed = 15.0f;
	public float acceleration = 4.5f;
	float accelerationMultiplier = 1.0f;
	public float deceleration = 4.5f;

	private float decel = 6.555f;
	private float originalRotationAngle = 0.0f;

	private Slider _fuelSlider;
	private Slider _vehHealthSlider;

	private Button _vehExitButton;

	private CanvasGroup _vehicleUIGroup;
	private CanvasGroup _vehicleExitButtonGroup;

	AudioSource engineSource;
	AudioSource crashSource;
	public AudioClip crashSound;
	public AudioClip engineSound;
	public AudioClip engineDying;
	//bool CrashPlayed = false;
	//bool engineDeathPlayed = false;
	List<GameObject> zombiesOnRoof = new List<GameObject>();

	Rigidbody vehicleRB;

	private float _lastHitTime = 0.0f;
	private float _replayTime = 0.0f;

	// Debug
	//public bool debugDisableAutoExit = false;

	// Use this for initialization
	void Start()
	{
		//Debug.Log("Vehicle Started: " + gameObject.name);
		if (!RoadTileManager.bMainMenu && !_vehicleUIGroup) _vehicleUIGroup = GameObject.Find("VehicleUI").GetComponent<CanvasGroup>();
		//_vehicleUIGroup.alpha = 0.0f;

		vehicleRB = GetComponent<Rigidbody>();
		vehicleRB.centerOfMass = centerOfMass;
		speed *= speedModifier;
		originalRotationAngle = rotationAngle;

		maxFuel = totalFuel;
		_maxHealth = health;

		if (!RoadTileManager.bMainMenu && !_vehicleExitButtonGroup) _vehicleExitButtonGroup = GameObject.Find("ExitVehicleButton").GetComponent<CanvasGroup>();
		//_vehicleExitButtonGroup.alpha = 0.0f;

		// Debug
		//timeBeforeNoButtonsPressedRemovesPlayer = 10000.0f;
	}

	private void OnEnable()
	{
		if (_driver == null)
			return;

		if (!RoadTileManager.bMainMenu && !_vehicleUIGroup) _vehicleUIGroup = GameObject.Find("VehicleUI").GetComponent<CanvasGroup>();
		if (!RoadTileManager.bMainMenu && !_vehicleExitButtonGroup) _vehicleExitButtonGroup = GameObject.Find("ExitVehicleButton").GetComponent<CanvasGroup>();
		_fuelSlider = GameObject.Find("FuelSlider").GetComponent<Slider>();
		_vehHealthSlider = GameObject.Find("VehicleHealthSlider").GetComponent<Slider>();
		_vehExitButton = GameObject.Find("ExitVehicleButton").GetComponent<Button>();

		engineSource = transform.Find("VehicleAudioSource").GetComponent<AudioSource>();
		crashSource = transform.Find("CrashAudioSource").GetComponent<AudioSource>();

		engineSource.clip = engineSound;
		engineSource.loop = true;
		engineSource.Play();

		crashSource.clip = crashSound;
		crashSource.loop = false;

		_vehicleUIGroup.alpha = 1.0f;

		_vehicleExitButtonGroup.alpha = 1.0f;

		_fuelSlider.maxValue = maxFuel;
		_fuelSlider.value = totalFuel;

		_vehHealthSlider.maxValue = _maxHealth;
		_vehHealthSlider.value = health;

		// Button btn = _vehExitButton.GetComponent<Button>();
		_vehExitButton.onClick.AddListener(TaskOnClick);
	}

	protected void InitialiseBase()
	{
		Start();
	}

	void TaskOnClick()
	{
		//Debug.Log("Button Pressed");
		speed = 0.0f;
		TryExitVehicle();
	}

	private void TryExitVehicle()
	{
		if (_driver == null) return;

		_vehicleUIGroup.alpha = 0.0f;
		_vehicleExitButtonGroup.alpha = 0.0f;
		_vehExitButton.onClick.RemoveAllListeners();
		speed = 0.0f;

		if (engineSource.isPlaying)
		{
			engineSource.Stop();
		}

		BikeLean bike = GetComponent<BikeLean>();
		if (bike)
		{
			bike.enabled = false;
			vehicleRB.constraints = RigidbodyConstraints.None;
			vehicleRB.centerOfMass = new Vector3(0, 0.5f, 0);
		}

		GameObject followCamera = gameObject.GetComponent<DisableVehicle>().followCamera;
		gameObject.GetComponent<DisableVehicle>().followCamera = null;
		_driver.GetComponent<DisableVehicle>().followCamera = followCamera;
		followCamera.GetComponent<FollowCamera>().target = _driver;

		Vector3 playerReposition = new Vector3(
			((transform.localPosition.x - 5) + (transform.localScale.x)),
			((transform.localPosition.y + 2) + (transform.localScale.y)),
			((transform.localPosition.z) + (transform.localScale.z)));

		_driver.GetComponent<OccupyVehicle>().ExitVehicle(this, playerReposition);

		GameObject[] zombies = zombiesOnRoof.ToArray();//copy because zombies will be removed while looping
		foreach (GameObject zombie in zombies)
		{
			zombie.GetComponent<ClimbOnVehicle>().DetachFromVehicle();
			zombie.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up * 0.5f).normalized * 500 * (speed / maxSpeed), ForceMode.Acceleration);
		}

		Camera.main.GetComponent<TransparentifyObject>().player = _driver.transform;

		_driver = null;

		timeNotMoving = 0.0f;
	}

	private void Update()
	{
		base.Update();

		

		if (measuredSpeed > maxSpeed)
		{
			//enabled = false;
			//return;
		}

		//if (!debugDisableAutoExit)
		{
			if (measuredSpeed < 2.0f)
			{
				timeNotMoving += Time.deltaTime;
			}
			else
			{
				timeNotMoving = 0;
			}
			if (timeNotMoving > 1.5f)
			{
				TryExitVehicle();
				speed = 0;
				return;
			}
		}

		if (speed <= 0.0f && (totalFuel <= 0.0f || deadState))
		{
			TryExitVehicle();
			return;
		}

		if (InputLeft() || InputRight() || InputUp())
		{
			timeWithNoButtonsPressed = 0;
		}
		else
		{
			timeWithNoButtonsPressed += Time.deltaTime;
		}

		//if (!debugDisableAutoExit)
		{

			if ((totalFuel <= 0.0f || deadState) && speed <= 0.1f
				&& timeWithNoButtonsPressed > timeBeforeNoButtonsPressedRemovesPlayer)
			{
				rotationAngle = 0.0f;
				speed = 0.0f;
				TryExitVehicle();
			}

			if (speed <= 0.1f && timeWithNoButtonsPressed > timeBeforeNoButtonsPressedRemovesPlayer)
			{
				rotationAngle = 0.0f;
				speed = 0.0f;
				TryExitVehicle();
			}
		}

		if (gameObject.transform.localPosition.y < -30.0f)
		{
			TryExitVehicle();
		}
	}

	// Update is called once per frame
	protected new void FixedUpdate()
	{
		if (_driver == null)
			return;

		// Implement damage from zombies
		if (health <= 0)
		{
			deadState = true;
		}

		// Slow the vehicle gradually until it stops if the vehicle 'dies'
		if (deadState)
		{
			if (speed <= 0)
			{
				TryExitVehicle();
				return;
			}

			speed -= deceleration * Time.deltaTime;

			if (engineSource.pitch >= 0.5)
			{
				engineSource.pitch -= 0.01f;
			}
		}
		else
		{
			if (InputLeft() || InputRight())
			{
				if (totalFuel > 0)
				{
					totalFuel -= fuelConsumptionRate * Time.deltaTime;
					totalFuel = Mathf.Max(totalFuel, 0);
					_fuelSlider.value = totalFuel;

					if (speed <= maxSpeed)
					{
						speed += (acceleration * accelerationMultiplier * Time.deltaTime);
						if (engineSource.pitch <= 1.3)
						{
							engineSource.pitch += 0.01f;
						}

					}
				}
				else
				{
					if (speed <= 0)
					{
						TryExitVehicle();
						return;
					}
					// Slow the vehicle gradually until it stops if the vehicle runs out of fuel
					speed -= 2.5f * Time.deltaTime;

					if (engineSource.pitch >= 0.5)
					{
						engineSource.pitch -= 0.01f;
					}
					else
					{
						engineSource.Stop();
					}
				}
			}
			else
			{
				if (totalFuel > 0.0f)
				{
					totalFuel -= (fuelConsumptionRate / 8) * Time.deltaTime;
					_fuelSlider.value = totalFuel;
				}

				speed -= (decel * Time.deltaTime);
				if (engineSource.pitch >= 0.5)
				{
					engineSource.pitch -= 0.01f;
				}
			}
		}

		speed = Mathf.Clamp(speed, 0, maxSpeed);
		rotationAngle = originalRotationAngle * (speed / maxSpeed);
		Vector3 velocity = (transform.forward * speed) * Time.deltaTime;
		transform.position = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z);
		if ((InputLeft() && InputRight()))
		{ }
		else if (InputLeft())
			transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), -rotationAngle * Time.deltaTime);
		else if (InputRight())
			transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), rotationAngle * Time.deltaTime);

		// Once we get touch controls working, change this to something else
		if (InputUp())
		{
			TaskOnClick();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_driver == null)
			return;

		if (_lastHitTime > 0.0f)
		{
			_lastHitTime -= Time.deltaTime;
			return;
		}

		//if ((collision.gameObject.layer & LayerMask.healthNameToLayer("Road")) != 0)
		//{
		//	return;
		//}

		GameObject zombie = collision.gameObject;

		if (!zombie || zombie.layer!=LayerMask.NameToLayer("Zombie")) return;

		if (zombie && zombie.GetComponent<Health>() && zombie.GetComponent<Health>().health <= 0)
		{
			return;
		}

        BikeLean bike = GetComponent<BikeLean>();
        PlayerHealth player = null;
        if (bike)
        {
            player = GetComponentInChildren<PlayerHealth>();
        }
        if (bike && player)
        {
            //zombie damages player
            player.GetHitByZombie(zombie);
        }
        else
        {
            //zombie damages vehicle
            if (health > 0)
            {
                //Debug.Log(gameObject.name + " docking health due to collision with " + collision.gameObject.name);
                health -= 0.5f;
                _vehHealthSlider.value = health;
                _lastHitTime = 0.5f;
            }
            else
            {
                health = 0.0f;
            }
        }
	}

	void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.CompareTag("Building"))
		{
			if (speed > 5)
			{
				if (Vector3.Angle(transform.forward, -collision.contacts[0].normal) < 30)//driving towards the building
				{
					Crash();
				}
			}
		}

		if (collision.gameObject.CompareTag("Zombie") && collision.gameObject.GetComponent<Health>())
		{
			OnCollisionEnter(collision);
		}
	}

	void Crash()
	{
		engineSource.Stop();
		engineSource.pitch = 1.0f;
		engineSource.loop = false;
		crashSource.Play();

		float damageFromCrash = _maxHealth / 3;
		health -= damageFromCrash;

		_vehHealthSlider.value = health;
		speed = 0;
		TryExitVehicle();
	}

	public GameObject GetDriver()
	{
		return _driver;
	}

	public void SetDriver(GameObject driver)
	{
		_driver = driver;
		timeWithNoButtonsPressed = -1.0f;//extra time to press buttons when entering vehicle
	}

	float zombieOnRoofSpeedChange = -1.0f;
	float zombieOnRoofAccelerationMultiplierChange = -0.1f;
	float zombieOnRoofMassChange = 10.0f;

	void ZombieOnRoofChanges(int multiplier)
	{
		if (maxSpeed < 0.0f)
		{
			maxSpeed = 0.0f;
		}

		if (accelerationMultiplier < 0.0f)
		{
			accelerationMultiplier = 0.0f;
		}
		maxSpeed += zombieOnRoofSpeedChange * multiplier;
		accelerationMultiplier += zombieOnRoofAccelerationMultiplierChange * multiplier;
		vehicleRB.mass += zombieOnRoofMassChange * multiplier;
	}

	public void AddZombieOnRoof(GameObject zombie)
	{
		zombiesOnRoof.Add(zombie);
		ZombieOnRoofChanges(1);
	}

	public void RemoveZombieFromRoof(GameObject zombie)
	{
		zombiesOnRoof.Remove(zombie);
		ZombieOnRoofChanges(-1);
	}

	public int GetNumZombiesOnRoof()
	{
		return zombiesOnRoof.Count;
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}
}
