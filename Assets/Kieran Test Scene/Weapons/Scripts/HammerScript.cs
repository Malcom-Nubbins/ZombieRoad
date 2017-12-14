using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : WeaponScript {

	void Start()
	{
		this.weapon = new Hammer(
		"Hammer",
		3.0f, // range
		1.0f, //delay
		20);  //durability/ammo
	}
}
