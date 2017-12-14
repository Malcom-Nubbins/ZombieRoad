using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballBatScript : WeaponScript
{

	void Start()
	{
		this.weapon = new BaseballBat(
		"Baseball bat",
		3.0f, // range
		1.5f, //delay
		10);  //durability/ammo
	}
}
