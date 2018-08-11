using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyVehicle : MonoBehaviour
{
    private float _occupyCooldown;

	private bool _occupiedVehicle;
	// Use this for initialization
	void Start()
	{
        _occupyCooldown = 0.0f;
		_occupiedVehicle = false;
	}

	// Update is called once per frame
	void Update()
	{
		if(_occupyCooldown > 0.0f)
        {
            _occupyCooldown -= Time.deltaTime;
        }
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.GetComponent<BaseVehicleClass>())
		{
            BaseVehicleClass vehicle = collision.collider.GetComponent<BaseVehicleClass>();
			TryEnterVehicle(vehicle);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		OnCollisionEnter(collision);
	}

    public void SetOccupyCooldown(float cooldown)
    {
        _occupyCooldown = cooldown;
    }

	private void TryEnterVehicle(BaseVehicleClass vehicle)
	{
        if(_occupyCooldown <= 0.0f && !_occupiedVehicle)
        {
            if (!Movement.InputLeft() && !Movement.InputRight())
            {
                if (vehicle.health <= 0 || vehicle.GetFuelPercentage() <= 0)
                {
                    return;
                }

                GameObject followCamera = gameObject.GetComponent<DisableVehicle>().followCamera;
                //if (followCamera == null) Debug.Log("null followcamera");
                //if (followCamera.GetComponent<FollowCamera>() == null) Debug.Log("null followcamera component");
                //if (followCamera.GetComponent<FollowCamera>().target == null) Debug.Log("null followcamera component target");
                //if (vehicle == null) Debug.Log("null vehicle");
				if(followCamera != null)
					followCamera.GetComponent<FollowCamera>().target = vehicle.gameObject;

				gameObject.GetComponent<DisableVehicle>().followCamera = null;
				vehicle.GetComponent<DisableVehicle>().followCamera = followCamera;
				vehicle.SetDriver(gameObject);

				EnterVehicle(vehicle);

                Camera.main.GetComponent<TransparentifyObject>().player = vehicle.transform;

	            _occupiedVehicle = true;
            }
        }
	}

    protected virtual void EnterVehicle(BaseVehicleClass vehicle)
    {
		Transform driverTransform = vehicle.transform.Find("DriverTransform");
		if (driverTransform == null)
		{
			gameObject.transform.Translate(0, 100, 0);
		}
		else
		{
            //place player on bike
            BikeLean bike = vehicle.GetComponent<BikeLean>();
            bike.enabled = true;
			gameObject.transform.parent = driverTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		GetComponent<Collider>().enabled = false;
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		gameObject.GetComponentInChildren<ZombieDetector>().enabled = false; // disable players zombie detector to prevent player using weapons while in vehicle
	}

    public void ExitVehicle(BaseVehicleClass vehicle, Vector3 position)
    {
        gameObject.GetComponentInChildren<ZombieDetector>().enabled = true;
        transform.parent = null;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        transform.RotateAround(vehicle.transform.localPosition, Vector3.up, vehicle.transform.eulerAngles.y);
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Collider>().enabled = true;

        SetOccupyCooldown(1.0f);
	    _occupiedVehicle = false;
    }
}
