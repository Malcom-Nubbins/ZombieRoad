using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : HandWeapon
{
	GameObject ParticleEmitter;
	ParticleSystem ParticleEmitterPS;
	public Shotgun(string weaponName, float attackingDistance, float attackingDelay, int durability) : base(weaponName, attackingDistance, attackingDelay, durability)
	{
		//ParticleEmitter = GameObject.Find("ShotgunBullet");
		//ParticleEmitterPS = ParticleEmitter.GetComponent<ParticleSystem>();
	}
	public override void attack(GameObject gameObject)
	{
		Health zombieHealth = gameObject.GetComponent<Health>();
		zombieHealth.health -= 2;
		
		Debug.Log("Shotgun attacking");
	}
}
