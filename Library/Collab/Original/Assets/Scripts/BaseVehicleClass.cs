﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	GameObject driver;
	float timeWithNoButtonsPressed = 0;

	public float maxSpeed = 15.0f;
	public float acceleration = 4.5f;
	float accelerationMultiplier = 1.0f;

	private float decel = 6.555f;
	private float originalRotationAngle = 0.0f;

	private Slider _fuelSlider;
	private Slider _vehHealthSlider;

	private Button _vehExitButton;

	private CanvasGroup _vehicleUIGroup;
	private CanvasGroup _vehicleExitButtonGroup;

    List<GameObject> zombiesOnRoof = new List<GameObject>();

	Rigidbody vehicleRB;

	private float _lastHitTime = 0.0f;

	// Use this for initialization
	void Start()
	{
		if (!_vehicleUIGroup) _vehicleUIGroup = GameObject.Find("VehicleUI").GetComponent<CanvasGroup>();
		_vehicleUIGroup.alpha = 0.0f;

		vehicleRB = GetComponent<Rigidbody>();
        vehicleRB.centerOfMass = centerOfMass;
		speed *= speedModifier;
		originalRotationAngle = rotationAngle;

		maxFuel = totalFuel;
		_maxHealth = health;

		if (!_vehicleExitButtonGroup) _vehicleExitButtonGroup = GameObject.Find("ExitVehicleButton").GetComponent<CanvasGroup>();
		_vehicleExitButtonGroup.alpha = 0.0f;

        // Debug
        //timeBeforeNoButtonsPressedRemovesPlayer = 10000.0f;
	}

	private void OnEnable()
	{
		_fuelSlider = GameObject.Find("FuelSlider").GetComponent<Slider>();
		_vehHealthSlider = GameObject.Find("VehicleHealthSlider").GetComponent<Slider>();
		_vehExitButton = GameObject.Find("ExitVehicleButton").GetComponent<Button>();
		if (!_vehicleUIGroup) _vehicleUIGroup = GameObject.Find("VehicleUI").GetComponent<CanvasGroup>();
		if (!_vehicleExitButtonGroup) _vehicleExitButtonGroup = GameObject.Find("ExitVehicleButton").GetComponent<CanvasGroup>();

		_vehicleUIGroup.alpha = 1.0f;

		_vehicleExitButtonGroup.alpha = 1.0f;
		_maxHealth = health;

		_fuelSlider.maxValue = maxFuel;
		_vehHealthSlider.maxValue = _maxHealth;

	   // Button btn = _vehExitButton.GetComponent<Button>();
		_vehExitButton.onClick.AddListener(TaskOnClick);
	}

	protected void InitialiseBase()
	{
		Start();
	}

	void TaskOnClick()
	{
		Debug.Log("Button Pressed");
		speed = 0.0f;
		TryExitVehicle();
	}

	private void TryExitVehicle()
	{
		if (driver == null) return;

		_vehicleUIGroup.alpha = 0.0f;
		_vehicleExitButtonGroup.alpha = 0.0f;
		_vehExitButton.onClick.RemoveAllListeners();
		speed = 0.0f;

		GameObject followCamera = gameObject.GetComponent<DisableVehicle>().followCamera;
		gameObject.GetComponent<DisableVehicle>().followCamera = null;
		driver.GetComponent<DisableVehicle>().followCamera = followCamera;
		followCamera.GetComponent<FollowCamera>().target = driver;

		Vector3 playerReposition = new Vector3(
			((transform.localPosition.x - 5) + (transform.localScale.x)),
			((transform.localPosition.y + 2) + (transform.localScale.y)),
			((transform.localPosition.z) + (transform.localScale.z)));


		driver.transform.SetPositionAndRotation(playerReposition, Quaternion.Euler(0.0f, 0.0f, 0.0f));
		driver.transform.RotateAround(transform.localPosition, Vector3.up, transform.eulerAngles.y);
		driver.GetComponent<Rigidbody>().useGravity = true;

        Camera.main.GetComponent<TransparentifyObject>().player = driver.transform;

        driver = null;
	}

	private void Update()
	{
        base.Update();
		if(speed <= 0.0f && (totalFuel <= 0.0f || deadState))
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
		if ((totalFuel <= 0.0f || deadState) && speed <= 0.1f
			&& timeWithNoButtonsPressed > timeBeforeNoButtonsPressedRemovesPlayer)
		{
			rotationAngle = 0.0f;
			speed = 0.0f;
			TryExitVehicle();
		}

		if(speed <= 0.1f && timeWithNoButtonsPressed > timeBeforeNoButtonsPressedRemovesPlayer)
		{
			rotationAngle = 0.0f;
			speed = 0.0f;
			TryExitVehicle();
		}

		if(gameObject.transform.localPosition.y < -30.0f)
		{
			TryExitVehicle();
		}
	}

	// Update is called once per frame
	protected new void FixedUpdate()
	{
		// Implement damage from zombies
		if (health <= 0)
			deadState = true;

		// Slow the vehicle gradually until it stops if the vehicle 'dies'
		if (deadState)
		{
			if(speed <= 0)
			{
				TryExitVehicle();
				return;
			}

			speed -= 4.5f * Time.deltaTime;
			speed = Mathf.Max(speed, 0);
			rotationAngle = originalRotationAngle * (speed / maxSpeed);
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
						speed = speed + (acceleration * accelerationMultiplier * Time.deltaTime);
						speed = Mathf.Min(speed, maxSpeed);
						rotationAngle = originalRotationAngle * (speed / maxSpeed);
					}
				}
				else
				{
					if(speed <= 0)
					{
						TryExitVehicle();
						return;
					}
					// Slow the vehicle gradually until it stops if the vehicle runs out of fuel
					speed -= 2.5f * Time.deltaTime;
					speed = Mathf.Max(0, speed);
					rotationAngle = originalRotationAngle * (speed / maxSpeed);
				}

				if ((InputLeft() && InputRight()))
				{ }
				else if (InputLeft())
					transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), -rotationAngle * Time.deltaTime);
				else if (InputRight())
					transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), rotationAngle * Time.deltaTime);

				Vector3 velocity = (transform.forward * speed) * Time.deltaTime;
				transform.position = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z);
			}
			else
			{
				if (totalFuel > 0.0f)
				{
					totalFuel -= (fuelConsumptionRate / 8) * Time.deltaTime;
					_fuelSlider.value = totalFuel;
				}

				speed = speed - (decel * Time.deltaTime);
				speed = Mathf.Max(speed, 0);
				if (speed < 0)
					speed = 0;

				rotationAngle = originalRotationAngle * (speed / maxSpeed);
				Vector3 velocity = (transform.forward * speed) * Time.deltaTime;

                if(velocity.x != 0 || velocity.y != 0 || velocity.z != 0)
				    transform.position = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z);
			}
		}

		//base.FixedUpdate();// movement/turning

		// Once we get touch controls working, change this to something else
		if(InputUp())
		{
			TaskOnClick();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_lastHitTime > 0.0f)
		{
			_lastHitTime -= Time.deltaTime;
			return;
		}

		GameObject zombie = collision.gameObject;

		if (zombie && zombie.GetComponent<Health>() && zombie.GetComponent<Health>().health <= 0)
		{
			return;
		}

		if (health > 0)
		{
			//TakeDamage(0.5f);
			health -= 0.5f;
			_vehHealthSlider.value = health;
			_lastHitTime = 1.0f;
		}
		else
		{
			health = 0.0f;
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

		if(collision.gameObject.CompareTag("Zombie"))
		{
			OnCollisionEnter(collision);
		}
	}

	void Crash()
	{
		GameObject[] zombies = zombiesOnRoof.ToArray();//copy because zombies will be removed while looping
		foreach (GameObject zombie in zombies)
		{
			zombie.GetComponent<ClimbOnVehicle>().DetachFromVehicle();
			zombie.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up * 0.5f).normalized * 500, ForceMode.Acceleration);
		}
		speed = 0;
		TryExitVehicle();
	}

	public GameObject GetDriver()
	{
		return driver;
	}

	public void SetDriver(GameObject driver)
	{
		this.driver = driver;
		timeWithNoButtonsPressed = -1.0f;//extra time to press buttons when entering vehicle
	}

	float zombieOnRoofSpeedChange = -1.0f;
	float zombieOnRoofAccelerationMultiplierChange = -0.1f;
	float zombieOnRoofMassChange = 10.0f;

	void ZombieOnRoofChanges(int multiplier)
	{
		if(maxSpeed < 0.0f)
		{
            maxSpeed = 0.0f;
		}

		if(accelerationMultiplier < 0.0f)
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
}
