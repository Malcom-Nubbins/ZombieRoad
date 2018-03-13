using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeLean : MonoBehaviour
{
    public float maxLeanAngle;
    float currentLeanAmount;//[-1,1]
    BaseVehicleClass vehicle;

	void Start()
    {
        vehicle = GetComponent<BaseVehicleClass>();
	}
	
	void Update()
    {
        float targetLeanAmount = (Movement.InputLeft() ? -1 : 0) + (Movement.InputRight() ? 1 : 0);
        currentLeanAmount += (targetLeanAmount - currentLeanAmount) * 5.0f * Time.deltaTime;
        float speedPercentage = vehicle.speed / vehicle.maxSpeed;
        float leanAngle = currentLeanAmount * (maxLeanAngle * speedPercentage);
        Vector3 localEuler = transform.localRotation.eulerAngles;
        localEuler.z = -1 * leanAngle;
        transform.localRotation = Quaternion.Euler(localEuler);
	}
}
