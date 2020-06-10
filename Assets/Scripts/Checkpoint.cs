using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
	//checkpoint cylinder radius is always 2 * checkpointRadius
	private Vector3 checkpointPosition;

	// persistent reference to the camera, from which we can retreive the active playercharacter or vehicle
	[NonNull] public FollowCamera FollowCamera;
	[NonNull] public Transform RoadMapRoot;

	[SerializeField] bool debugDisableTimer;
	[SerializeField] bool TextSizeFlag = false;

	[SerializeField] float checkpointRadius;
	float timeRemaining;
	float nextTimeRemaining;

	//private float checkpointRotationSpeed;
	[SerializeField] private TextMeshProUGUI m_CheckpointTimerText;

	[SerializeField] Text checkpointDistance;

	static protected int level = 0;

	[SerializeField, NonNull] MeshFilter mesh;
	[SerializeField] Mesh[] lods;

	public event Action OnCheckpointExtend;

	float checkpointDisplayRadius;//goes from 0 to checkpointRadius to animate next checkpoint

	public virtual void Start()
	{
		checkpointPosition = FollowCamera.target.transform.position;
		checkpointPosition.y = -0.1f;

		checkpointRadius = gameObject.transform.localScale.x / 2.0f;
		gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

		level = 0;
		timeRemaining = 10;
		nextTimeRemaining = 2;//start small so it does not scale too fast

		//checkpointRotationSpeed = 50.0f;

		m_CheckpointTimerText.text = "Time Left: " + timeRemaining;

		for (int i = 0; i < lods.Length; ++i)
			lods[i] = Instantiate(lods[i]);

		if (!mesh.mesh) mesh.mesh = lods[0];
	}

	public virtual void Update()
	{
		//Transform checkpointTransform = GetComponent<Transform>();
		//checkpointTransform.Rotate((Vector3.up * checkpointRotationSpeed) * Time.deltaTime);

		Vector3 playerPosition = FollowCamera.target.transform.position;
		Vector3 distance = playerPosition - checkpointPosition;
		distance.y = 0;

		if (checkpointDisplayRadius < checkpointRadius)
		{
			checkpointDisplayRadius += checkpointRadius * 1.0f * Time.deltaTime;
			if (checkpointDisplayRadius > checkpointRadius) checkpointDisplayRadius = checkpointRadius;
			gameObject.transform.localScale = new Vector3(checkpointDisplayRadius * 2, 8.0f, checkpointDisplayRadius * 2);
		}

		if (distance.magnitude >= checkpointRadius)
		{
			UpdateCheckpoint();
		}

		if (timeRemaining > 0 && !debugDisableTimer)
			timeRemaining -= Time.deltaTime;

		if (timeRemaining < 0)
		{
			//Debug.Log("Player ran out of time!");
			//end/restart game?
		   // CancelInvoke("decreaseTimeRemaining");
			timeRemaining = 0;

			PlayerDeath player = GameObject.Find("PlayerCharacter").GetComponent<PlayerDeath>();
			player.killPlayer();
		}

        if (timeRemaining < 10 && timeRemaining > 0)
        {
			m_CheckpointTimerText.color = new Color(100, 0, 0);

			if(m_CheckpointTimerText.fontSize < 47 && TextSizeFlag)
			{
				m_CheckpointTimerText.fontSize += 30.0f * Time.deltaTime;
				if (m_CheckpointTimerText.fontSize >= 47)
				{
					m_CheckpointTimerText.fontSize = 47;
					TextSizeFlag = false;
				}
			}
			else
			{
				m_CheckpointTimerText.fontSize -= 30.0f * Time.deltaTime;
				if (m_CheckpointTimerText.fontSize <= 36)
				{
					TextSizeFlag = true;
					m_CheckpointTimerText.fontSize = 36;
				}
			}
		}
		else if(timeRemaining <= 0)
		{
			m_CheckpointTimerText.color = new Color(100, 0, 0);
			m_CheckpointTimerText.fontSize = 47;
		}
        else
        {
			m_CheckpointTimerText.color = new Color(0, 0, 0);
			m_CheckpointTimerText.fontSize = 47;
        }
		m_CheckpointTimerText.text = "Time Left: " + timeRemaining.ToString("0.0");

		float distFromCentre = distance.magnitude;
		float distFromEdge = checkpointRadius - distFromCentre;
		checkpointDistance.text = distFromEdge.ToString("0") + "m";
	}

	public virtual void UpdateCheckpoint()
	{
		float nextChackpointSizeMultiplier = 1.2f;

		checkpointRadius *= nextChackpointSizeMultiplier;

		checkpointDisplayRadius = 0;
		gameObject.transform.localScale = Vector3.zero;

		if (checkpointRadius > 50.0f && checkpointRadius < 256.0f && lods.Length > 1)
			mesh.mesh = lods[1];
		else if (checkpointRadius > 256.0f && checkpointRadius < 1200.0f && lods.Length > 2)
			mesh.mesh = lods[2];
		else if (checkpointRadius > 1200.0f /*&& checkpointRadius < 1200.0f*/ && lods.Length > 3)
			mesh.mesh = lods[3];

		checkpointPosition = FollowCamera.target.transform.position;
		checkpointPosition.y = -0.1f;
		gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

		level++;
		nextTimeRemaining *= nextChackpointSizeMultiplier;//scale time the same as radius
		timeRemaining += nextTimeRemaining;
		//Debug.Log("Checkpoint radius: " + checkpointRadius + ", Time added to get there: " + nextTimeRemaining);

		OnCheckpointExtend?.Invoke();
	}

	public float GetRadius()
	{
		return checkpointRadius;
	}

	public static int GetLevel()
	{
		return level;
	}
}
