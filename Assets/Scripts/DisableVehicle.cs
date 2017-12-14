using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVehicle : MonoBehaviour
{

	public GameObject followCamera;

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!followCamera || followCamera.GetComponent<FollowCamera>().target!=gameObject)
		{
			transform.GetComponent<Movement>().enabled = false;
		}
		else if (transform.rotation.z >= 0.43f || transform.rotation.z <= -0.43f)
		{
			transform.GetComponent<Movement>().enabled = false;
			//Debug.Log("Shouldn't be moving");
		}
		else
		{
			transform.GetComponent<Movement>().enabled = true;
		}
	}
}
