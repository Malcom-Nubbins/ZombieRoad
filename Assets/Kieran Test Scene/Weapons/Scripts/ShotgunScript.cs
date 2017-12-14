using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : WeaponScript
{

	// Use this for initialization
	void Start()
	{
		this.weapon = new Shotgun(
		"Shotgun",
		21.0f, // range
		1.5f, //delay
		2);  //durability/ammo
	}


}
