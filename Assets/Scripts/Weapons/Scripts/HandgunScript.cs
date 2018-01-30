using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunScript : WeaponScript
{
	// Use this for initialization
	void Start ()
	{
		this.weapon = new Handgun(
		"Handgun",
		25.0f, // range
		0.5f, //delay
		50,  //durability/ammo
        true,
        3);
    }
	

}

