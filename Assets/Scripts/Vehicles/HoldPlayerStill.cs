using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPlayerStill : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Transform t in transform)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }
	}
}
