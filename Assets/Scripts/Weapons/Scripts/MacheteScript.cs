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
		0.1f, //delay
		2000,  //durability/ammo
        false,
        20);
    }


}

