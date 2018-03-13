using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : HandWeapon
{
    static GameObject Bullet;
    GameObject Zombie;
    Vector3 MoveVector;

    //BulletScript BulletScript = new BulletScript();
//	ParticleSystem ParticleEmitterPS;
//    ParticleSystem.Particle[] particles;

	public Handgun(string weaponName, float attackingDistance, float attackingDelay, int durability, bool ranged, int weaponTargetCount, AudioClip weaponSound) : base(weaponName, attackingDistance, attackingDelay, durability, ranged, weaponTargetCount, weaponSound)
    {
//		if (ParticleEmitter) SetParticleEmitter(ParticleEmitter);
        //Bullet = GameObject.Find("Bullet");
	}

	public void SetParticleEmitter(GameObject ParticleEmitter_)
	{

//		ParticleEmitter = ParticleEmitter_;
//		ParticleEmitterPS = ParticleEmitter.GetComponent<ParticleSystem>();



    }
/*
    private void updateParticle(GameObject target)
    {
        ParticleEmitterPS.Emit(1);
        particles = new ParticleSystem.Particle[ParticleEmitterPS.particleCount];
        ParticleEmitterPS.GetParticles(particles);
        Debug.Log("particles found: " + this.particles.Length);
         if(this.particles.Length != 0)
                {
                   // particles[0].position = Vector3.MoveTowards(particles[0].position, target.transform.position, Time.deltaTime / 2.0f);
                    float addedForce =(particles[0].startLifetime - particles[0].remainingLifetime) * (10 * Vector3.Distance(target.transform.position, particles[0].position));
                    particles[0].velocity =  (target.transform.position - particles[0].position).normalized * (addedForce * 5);
                    Debug.Log(addedForce);
                    //particles[0].position = Vector3.Lerp(particles[0].position, target.transform.position,Time.deltaTime / 2.0f);
                    ParticleEmitterPS.SetParticles(particles, particles.Length);
            
                }
    }
    */

	public override void attack(GameObject TargetZombie)
	{
        Zombie = TargetZombie;
        Health zombieHealth = Zombie.GetComponent<Health>();
		zombieHealth.health -= 2;
        //Debug.Log("Handgun attacking");
        //Bullet.GetComponent<BulletScript>().ShootBullet(Zombie,);

        //ShootBullet();

        //        updateParticle(gameObject);
        //        ParticleEmitterPS.Play();

    }
}
