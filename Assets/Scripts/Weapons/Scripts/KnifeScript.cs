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
		1.0f, //delay
		20);  //durability/ammo
	}
}
