using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : WeaponScript
{
	void Start()
	{
		this.weapon = new Knife(
		"Knife",
		3.0f, // range
		0.3f, //delay
		25,  //durability/ammo
        false,
        1);
    }
}
