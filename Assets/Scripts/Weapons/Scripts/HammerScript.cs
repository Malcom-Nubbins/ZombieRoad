using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : WeaponScript {

	void Start()
	{
		this.weapon = new Hammer(
		"Hammer",
		3.0f, // range
		0.1f, //delay
		20,  //durability/ammo
        false,
        1);
	}
}
