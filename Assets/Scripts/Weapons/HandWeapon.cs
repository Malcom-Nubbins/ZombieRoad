using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class HandWeapon {


	private float attackingDistance;
	private float attackingDelay;
	private string weaponName;
	private int durability;
    private bool ranged;
    private int weaponTargetCount;
    private AudioClip weaponSound;

	public HandWeapon(string weaponName, float attackingDistance, float attackingDelay, int durability, bool ranged, int weaponTargetCount, AudioClip weaponSound)
	{
		this.weaponName = weaponName;
		this.attackingDistance = attackingDistance;
		this.attackingDelay = attackingDelay;
		this.durability = durability;
        this.ranged = ranged;
        this.weaponTargetCount = weaponTargetCount;
        this.weaponSound = weaponSound;
	}
    public HandWeapon(string weaponName, float attackingDistance, float attackingDelay, int durability, bool ranged, int weaponTargetCount)
    {
        this.weaponName = weaponName;
        this.attackingDistance = attackingDistance;
        this.attackingDelay = attackingDelay;
        this.durability = durability;
        this.ranged = ranged;
        this.weaponTargetCount = weaponTargetCount;
    }

    public string getWeaponName()
	{
		return weaponName;
	}

	public float getAttackingDistance()
	{
		return attackingDistance;
	}


	public float getAttackingDelay()
	{
		return attackingDelay;
	}

	public int getDurability()
	{
		return durability;
	}

	public void reduceDurability()
	{
		durability--;
	}
    public bool getRanged()
    {
        return ranged;
    }
    public int getWeaponTargetCount()
    {
        return weaponTargetCount;
    }
    public AudioClip getWeaponSound()
    {
        return weaponSound;
    }


	public abstract void attack(GameObject gameObject);
}
