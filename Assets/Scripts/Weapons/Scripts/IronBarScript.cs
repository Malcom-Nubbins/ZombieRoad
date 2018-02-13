using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBarScript : WeaponScript
{

	void Start()
	{
		this.weapon = new IronBar(
		"IronBar",
		5.0f, // range
		0.7f, //delay
		50,  //durability/ammo
        false,
        1);
    }
}
