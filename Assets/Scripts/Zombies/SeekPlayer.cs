﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZR.Road;

public class SeekPlayer : MonoBehaviour
{
	// persistent reference to the camera, from which we can retreive the active playercharacter or vehicle
	public static FollowCamera followCamera;
	public float speed = 3.0f;
    public float wanderForce = 1.0f;
	public float wanderCircleRadius = 2.0f;
	public float wanderCircleDistanceInFront = 3.0f;
	public float wanderCircleRotationSpeed = 90.0f;
	float wanderCircleCurrentAngle = 0.0f;
    public float swarmAlignmentForce = 1.0f;
    public float swarmCohesionForce = 1.0f;
    public float swarmSeparationForce = 1.0f;

	Rigidbody rb;
	public Vector3 MainMenuDest;
	Vector3 seekDirection;
    AudioSource zombieSource;
    public AudioClip zombieAttack;

	private bool _seeking;

	void Start()
	{
		if (followCamera == null)
		{
			followCamera = FindObjectOfType<FollowCamera>();
		}
		GetComponent<Health>().onDeath += () => {
			enabled = false;
		};
		MainMenuDest = new Vector3(Random.value * followCamera.CullDistance - followCamera.CullDistance/2.0f, 0, Random.value * followCamera.CullDistance - followCamera.CullDistance / 2.0f) + RoadTileManager.checkpoint.RoadMapRoot.position;
        zombieSource = GetComponent<AudioSource>();

		StartCoroutine(SeekPlayerRoutine());
    }

	private IEnumerator CheckCollisionsRoutine(List<Transform> nearZombies, Vector3 averageForwardDirection, Vector3 averagePosition, Vector3 separationDirection)
	{
		Collider[] near = Physics.OverlapSphere(transform.position, 10);

		foreach (Collider c in near)
		{
			if (c != this && c.CompareTag("Zombie") && c.gameObject.GetComponent<Health>())
			{
				nearZombies.Add(c.transform);
			}
		}

		foreach (Transform zomb in nearZombies)
		{
			averageForwardDirection += zomb.forward;
			averagePosition += zomb.position;
			separationDirection += (transform.position - zomb.position);
		}

		yield return null;
	}

	private IEnumerator SeekPlayerRoutine()
	{
		while(true)
		{
			var health = gameObject.GetComponent<Health>();

			if (health.health <= 0)
				break;

			Vector3 toPlayer = (RoadTileManager.bMainMenu ? MainMenuDest : followCamera.target.transform.position) - transform.position;
			toPlayer.y = 0;//dont move up or down, physics will handle that

			if (RoadTileManager.bMainMenu || toPlayer.magnitude <= 200)
			{
				seekDirection = toPlayer.normalized;
				float seekAngle = Mathf.Atan2(seekDirection.z, seekDirection.x);
				//Debug.DrawRay(transform.position + transform.up, seekDirection * 5, Color.green);


				//wander
				wanderCircleCurrentAngle += Random.Range(-1.0f, 1.0f) * wanderCircleRotationSpeed * Time.deltaTime;
				if (wanderCircleCurrentAngle < 0) wanderCircleCurrentAngle += 360.0f;
				if (wanderCircleCurrentAngle >= 360.0f) wanderCircleCurrentAngle -= 360.0f;
				//add seekAngle so if wanderCircleCurrentAngle is 0 it will move in the seek direction instead of wandering towards positive x
				Vector3 wanderCircleDirection = new Vector3(
					Mathf.Cos(wanderCircleCurrentAngle * Mathf.Deg2Rad + seekAngle),
					0,
					Mathf.Sin(wanderCircleCurrentAngle * Mathf.Deg2Rad + seekAngle)
				);
				Vector3 pointOnWanderCirclePosition = (seekDirection * wanderCircleDistanceInFront) + wanderCircleDirection * wanderCircleRadius;
				Vector3 pointOnWanderCircleDirection = pointOnWanderCirclePosition.normalized;
				//Debug.DrawRay(transform.position + transform.up, pointOnWanderCircleDirection * 5, Color.red);

				//swarm

				List<Transform> nearZombies = new List<Transform>();

				//alignment
				Vector3 averageForwardDirection = transform.forward;
				//cohesion
				Vector3 averagePosition = transform.position;
				//separation
				Vector3 separationDirection = Vector3.zero;

				//yield return CheckCollisionsRoutine(nearZombies, averageForwardDirection, averagePosition, separationDirection);

				averageForwardDirection.y = 0;
				averageForwardDirection.Normalize();
				if (nearZombies.Count > 0) averagePosition /= (nearZombies.Count + 1);//+1 to include this zombies position
				separationDirection.y = 0;
				separationDirection.Normalize();
				Vector3 averagePositionOffset = (averagePosition - transform.position);
				averagePositionOffset.y = 0;
				averagePositionOffset.Normalize();


				Vector3 moveDirection = Vector3.zero;
				moveDirection += pointOnWanderCircleDirection * wanderForce;
				moveDirection += averageForwardDirection * swarmAlignmentForce;
				moveDirection += averagePositionOffset * swarmCohesionForce;
				moveDirection += separationDirection * swarmSeparationForce;
				moveDirection.Normalize();

				Quaternion rotation = transform.rotation;
				rotation.SetLookRotation(moveDirection, transform.up);
				transform.rotation = rotation;

				transform.position = transform.position + transform.forward * speed * Time.deltaTime;
			}

			if (RoadTileManager.bMainMenu && toPlayer.magnitude <= 20)
				MainMenuDest = new Vector3(Random.value * followCamera.CullDistance - followCamera.CullDistance / 2.0f, 0, Random.value * followCamera.CullDistance - followCamera.CullDistance / 2.0f) + RoadTileManager.checkpoint.RoadMapRoot.position;

			yield return null;
		}

		yield return null;
	}
	
	void Update()
	{
		
	}

	private void OnCollisionStay(Collision collision)
	{
		if (RoadTileManager.bMainMenu) OnCollisionEnter(collision);
	}

	void OnCollisionEnter(Collision collision)
	{
        if (RoadTileManager.bMainMenu)
		{
			if (collision.gameObject.tag != "Zombie") return;

			GameObject zombie = collision.gameObject;

			if (zombie.GetComponent<Health>() == null) return; // shouldn't be happening

			if (zombie.GetComponent<Health>().health <= 0) return;

			gameObject.GetComponent<Health>().health -= 0.5f;

		}
	}
}
