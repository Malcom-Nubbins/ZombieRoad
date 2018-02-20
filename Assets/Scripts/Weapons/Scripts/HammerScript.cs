using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : WeaponScript {

    public AudioClip clip;
	void Start()
	{
        this.weapon = new Hammer(
        "Hammer",
        3.5f, // range
        0.4f, //delay
        20,  //durability/ammo
        false,
        1,
        clip);
	}
}
