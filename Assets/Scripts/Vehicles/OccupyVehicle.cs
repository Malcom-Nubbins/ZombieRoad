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
		if (collision.collider.gameObject.GetComponent<BaseVehicleClass>())
		{
			TryEnterVehicle(collision.collider.gameObject);
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

	private void TryEnterVehicle(GameObject vehicle)
	{
        if(_occupyCooldown <= 0.0f)
        {
            if (!Movement.InputLeft() && !Movement.InputRight())
            {
                if (vehicle.GetComponent<BaseVehicleClass>().health <= 0 || vehicle.GetComponent<BaseVehicleClass>().GetFuelPercentage() <= 0)
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
}
