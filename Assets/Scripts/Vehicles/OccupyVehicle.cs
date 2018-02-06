using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyVehicle : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.GetComponent<BaseVehicleClass>())
		{
			TryEnterVehicle(collision.collider.gameObject);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		OnCollisionEnter(collision);
	}

	private void TryEnterVehicle(GameObject vehicle)
	{
		if (!Movement.InputLeft() && !Movement.InputRight())
		{
            if(vehicle.GetComponent<BaseVehicleClass>().health <= 0 || vehicle.GetComponent<BaseVehicleClass>().GetFuelPercentage() <= 0)
            {
                return;
            }

			GameObject followCamera = gameObject.GetComponent<DisableVehicle>().followCamera;
			gameObject.GetComponent<DisableVehicle>().followCamera = null;
			vehicle.GetComponent<DisableVehicle>().followCamera = followCamera;
			vehicle.GetComponent<BaseVehicleClass>().SetDriver(gameObject);
			followCamera.GetComponent<FollowCamera>().target = vehicle;
			gameObject.transform.Translate(0, 100, 0);
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            Camera.main.GetComponent<TransparentifyObject>().player = vehicle.transform;
		}
	}
}
