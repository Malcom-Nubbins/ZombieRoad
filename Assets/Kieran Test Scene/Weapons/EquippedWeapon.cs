using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : MonoBehaviour
{

	public HandWeapon equippedWeapon;
    public GameObject Bullet;
    GameObject SpawnedBullet;
    public ZombieDetector zombieDetector;
	GameObject zombie;
	public GameObject followCamera;
    float AttackCooldown;

	public bool attacking = false;
	int zombieMask;
	// Use this for initialization
	void Start()
	{
		zombieMask = 1 << 8;
	}
	
	// Update is called once per frame
	void Update()
	{
        
		//Debug.Log(attacking);
		zombie = zombieDetector.GetNearestZombie();
		if (equippedWeapon != null)
		{
			//if zombie is dead stop attacking
			if (!zombie || zombie.GetComponent<Health>().health <= 0)
			{
                CancelInvoke("attack");
				attacking = false;
				return;
			}

			Vector3 zombiePosition = zombie.transform.position;
			Vector3 playerPosition = followCamera.GetComponent<FollowCamera>().target.transform.position;
            //distance from zombie to player

			if (Vector3.Distance(zombiePosition, playerPosition) <= equippedWeapon.getAttackingDistance())
			{
			   // Debug.Log(Vector3.Distance(zombiePosition, playerPosition));
			   // Debug.Log(attacking);
				//only want to call this once so use the attacking flag and check if facing
				if (!attacking)
				{
					//Debug.DrawLine(playerPosition, (playerPosition + followCamera.GetComponent<FollowCamera>().target.transform.forward * 20), Color.green, 2, false);
				   
					//Debug.Log("StartAttack");
                   
					InvokeRepeating("attack", 0.0f, equippedWeapon.getAttackingDelay());
					attacking = true;
					
				}
			}
			else
			{
				//Debug.Log("ZombiePos: " + zombiePosition + "  PlayerPos: " + playerPosition + "   DISTANCE: " + Vector3.Distance(zombiePosition, playerPosition));
				//Debug.Log(zombie.name);
				//if player is not in distance, stop attacking
				CancelInvoke("attack");
				attacking = false;
			}
		}

	}

	public void attack()
	{
		//if no weapon is equipped, don't attack
		if (equippedWeapon == null || AttackCooldown > 0)
		{
            //Debug.Log(AttackCooldown);
            AttackCooldown = AttackCooldown - (Time.deltaTime * 50);
			return;
            
		}

        if(equippedWeapon.getWeaponName() == "Handgun" || equippedWeapon.getWeaponName() == "Shotgun")
        {
            SpawnedBullet = Instantiate(Bullet, transform.position, Quaternion.identity);
            //Destroy(SpawnedBullet, 1.5f);
            SpawnedBullet.GetComponent<BulletScript>().ShootBullet(zombie);
            
        }
		//attack - pass in the zombie the player is attacking - only 1 for now
		//until they are tagged

		equippedWeapon.attack(zombie);
        equippedWeapon.reduceDurability();
		//Debug.Log(equippedWeapon.getWeaponName() + " Durability: " + equippedWeapon.getDurability());
        AttackCooldown = equippedWeapon.getAttackingDelay();
	   

	}
}
