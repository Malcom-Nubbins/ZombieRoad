﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballBatScript : WeaponScript
{
    public AudioClip clip;
	void Start()
	{
        this.weapon = new BaseballBat(
        "Baseball bat",
        4.0f, // range
        0.5f, //delay
        10,  //durability/ammo
        false, // true for ranged / false for melee
        1,
        clip); // max amount of zombies weapon can hit
	}
}
