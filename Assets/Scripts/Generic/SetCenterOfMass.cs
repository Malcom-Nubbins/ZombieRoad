using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour
{
    public Rigidbody rigidBody;

	void Start()
    {
        if (!rigidBody) rigidBody = GetComponentInParent<Rigidbody>();
        rigidBody.centerOfMass = transform.position;
	}
}
