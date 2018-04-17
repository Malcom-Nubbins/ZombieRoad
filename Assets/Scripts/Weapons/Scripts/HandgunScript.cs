using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunScript : WeaponScript
{
    public AudioClip clip;
	// Use this for initialization
	void Start ()
	{
		this.weapon = new Handgun(
		"Handgun",
		25.0f, // range
		0.5f, //delay
		31,  //durability/ammo
        true,
        1,
        clip);
    }
	

}

