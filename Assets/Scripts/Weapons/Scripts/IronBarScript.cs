using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBarScript : WeaponScript
{

	void Start()
	{
		this.weapon = new IronBar(
		"IronBar",
		3.0f, // range
		1.0f, //delay
		30,  //durability/ammo
        false,
        1);
    }
}
