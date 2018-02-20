using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : WeaponScript
{
    public AudioClip clip;
	void Start()
	{
		this.weapon = new Chain(
		"Chain",
		5.0f, // range
		0.7f, //delay
		10,  //durability/ammo
        false,
        4,
        clip);
	}
}
