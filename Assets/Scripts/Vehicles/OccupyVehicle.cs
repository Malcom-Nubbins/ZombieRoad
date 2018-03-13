using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyVehicle : MonoBehaviour
{
    private float _occupyCooldown;
	// Use this for initialization
	void Start()
	{
        _occupyCooldown = 0.0f;	
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
		if (collision.collider.CompareTag("Vehicle"))
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
        if(_occupyCooldown <= 0.0f)
        {
            if (!Movement.InputLeft() && !Movement.InputRight())
            {
                if (vehicle.health <= 0 || vehicle.GetFuelPercentage() <= 0)
                {
                    return;
                }

                GameObject followCamera = gameObject.GetComponent<DisableVehicle>().followCamera;
                gameObject.GetComponent<DisableVehicle>().followCamera = null;
                vehicle.GetComponent<DisableVehicle>().followCamera = followCamera;
                vehicle.SetDriver(gameObject);
                //if (followCamera == null) Debug.Log("null followcamera");
                //if (followCamera.GetComponent<FollowCamera>() == null) Debug.Log("null followcamera component");
                //if (followCamera.GetComponent<FollowCamera>().target == null) Debug.Log("null followcamera component target");
                //if (vehicle == null) Debug.Log("null vehicle");
                followCamera.GetComponent<FollowCamera>().target = vehicle.gameObject;

                EnterVehicle(vehicle);

                Camera.main.GetComponent<TransparentifyObject>().player = vehicle.transform;
            }
        }
	}

    private void EnterVehicle(BaseVehicleClass vehicle)
    {
        //place player in vehicle/on bike
        GetComponent<Collider>().enabled = false;
        Transform driverTransform = vehicle.transform.Find("DriverTransform");
        if (driverTransform == null) driverTransform = vehicle.transform;
        gameObject.transform.parent = driverTransform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
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

        SetOccupyCooldown(2.0f);
    }
}
