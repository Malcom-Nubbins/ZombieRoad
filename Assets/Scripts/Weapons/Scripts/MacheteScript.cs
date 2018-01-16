using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacheteScript : WeaponScript
{

	// Use this for initialization
	void Start()
	{
		this.weapon = new Machete(
		"Machete",
		4.0f, // range
		1.5f, //delay
		2);  //durability/ammo
	}


}

