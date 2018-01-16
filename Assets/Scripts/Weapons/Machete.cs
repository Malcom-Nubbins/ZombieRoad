using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machete : HandWeapon
{
	public Machete(string weaponName, float attackingDistance, float attackingDelay, int durability) : base(weaponName, attackingDistance, attackingDelay, durability)
	{

	}
	public override void attack(GameObject gameObject)
	{
		Health zombieHealth = gameObject.GetComponent<Health>();
		zombieHealth.health -= 1;
		Debug.Log("Machete attacking");
	}
}