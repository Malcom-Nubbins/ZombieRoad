using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekPlayer : MonoBehaviour
{
	// persistent reference to the camera, from which we can retreive the active playercharacter or vehicle
	public static FollowCamera followCamera;
	public float speed = 3.0f;
    public float wanderCircleRadius = 2.0f;
    public float wanderCircleDistanceInFront = 3.0f;
    public float wanderCircleRotationSpeed = 90.0f;
    float wanderCircleCurrentAngle = 0.0f;
    Rigidbody rb;
	public Vector3 MainMenuDest;
	Vector3 seekDirection;
	float AIdelay;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (followCamera == null)
        {
            followCamera = FindObjectOfType<FollowCamera>();
		}
        GetComponent<Health>().onDeath += () => {
            enabled = false;
        };
		MainMenuDest = new Vector3(Random.value * 200, 0, Random.value * -200) + RoadTileManager.checkpoint.RoadMapRoot.transform.position;
		AIdelay = Random.value;
	}
	
	void Update()
    {
		Vector3 toPlayer = (RoadTileManager.bMainMenu ? MainMenuDest : followCamera.target.transform.position) - transform.position;
        toPlayer.y = 0;//dont move up or down, physics will handle that

        if (RoadTileManager.bMainMenu || toPlayer.magnitude <= 200)
        {
			seekDirection = toPlayer.normalized;
			float seekAngle = Mathf.Atan2(seekDirection.z, seekDirection.x);
			//Debug.DrawRay(transform.position + transform.up, seekDirection * 5, Color.green);

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

			Vector3 moveDirection = pointOnWanderCircleDirection;

			Quaternion rotation = transform.rotation;
			rotation.SetLookRotation(moveDirection, transform.up);
			transform.rotation = rotation;

            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        }
		if (RoadTileManager.bMainMenu)
		{
			if (toPlayer.magnitude <= 20) MainMenuDest = new Vector3(Random.value * 200, 0, Random.value * -200) + RoadTileManager.checkpoint.RoadMapRoot.transform.position;
		}
	}
}
