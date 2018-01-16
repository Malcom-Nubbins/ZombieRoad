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
		if (!followCamera && transform.position.y < -20) Destroy(gameObject);

		if (!followCamera || followCamera.GetComponent<FollowCamera>().target!=gameObject)
		{
			transform.GetComponent<Movement>().enabled = false;
		}
		else if (transform.rotation.z >= 0.66f || transform.rotation.z <= -0.66f)
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
