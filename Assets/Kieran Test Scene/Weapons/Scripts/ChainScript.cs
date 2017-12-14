using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : WeaponScript
{

	void Start()
	{
		this.weapon = new Chain(
		"Chain",
		4.0f, // range
		1.5f, //delay
		30);  //durability/ammo
	}
}
