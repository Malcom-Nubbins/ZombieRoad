using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class HandWeapon {


	private float attackingDistance;
	private float attackingDelay;
	private string weaponName;
	private int durability;

	public HandWeapon(string weaponName, float attackingDistance, float attackingDelay, int durability)
	{
		this.weaponName = weaponName;
		this.attackingDistance = attackingDistance;
		this.attackingDelay = attackingDelay;
		this.durability = durability;
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


	public abstract void attack(GameObject gameObject);
}
