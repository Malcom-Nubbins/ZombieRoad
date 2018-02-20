using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : WeaponScript
{
    public AudioClip clip;
    // Use this for initialization
    void Start()
	{
		this.weapon = new Shotgun(
		"Shotgun",
		20.0f, // range
		0.5f, //delay
		10,  //durability/ammo
        true,
        6,
        clip);
    }


}
