using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
	//checkpoint cylinder radius is always 2 * checkpointRadius
	private Vector3 checkpointPosition;

	// persistent reference to the camera, from which we can retreive the active playercharacter or vehicle
	public GameObject FollowCamera;
	public GameObject RoadMapRoot;

	public bool DebugDisableTimer;
    public bool TextSizeFlag = false;

	public float checkpointRadius;
	private float timeRemaining;

	//private float checkpointRotationSpeed;
	private Text _checkpointTimer;

    Text checkpointDistance;

    static int level = 0;
	
	public Mesh[] lods;

	void Start()
	{
		checkpointPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;
		checkpointPosition.y = -0.1f;

		checkpointRadius = gameObject.transform.localScale.x / 2.0f;
		gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

        level = 0;
		timeRemaining = 9;

		//checkpointRotationSpeed = 50.0f;

		_checkpointTimer = GameObject.Find("CheckpointTimerText").GetComponent<Text>();
		_checkpointTimer.text = "Time Left: " + timeRemaining;

        checkpointDistance = GameObject.Find("CheckpointDistanceText").GetComponent<Text>();

		for (int i = 0; i < lods.Length; i++)
			lods[i] = Instantiate(lods[i]);

		if (!gameObject.GetComponent<MeshFilter>().mesh) gameObject.GetComponent<MeshFilter>().mesh = lods[0];
	}

	void Update()
	{
		//Transform checkpointTransform = GetComponent<Transform>();
		//checkpointTransform.Rotate((Vector3.up * checkpointRotationSpeed) * Time.deltaTime);

		Vector3 playerPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;
		Vector3 distance = playerPosition - checkpointPosition;
        distance.y = 0;

		if (distance.magnitude >= checkpointRadius)
		{
			UpdateCheckpoint();
		}

		if (timeRemaining > 0 && !DebugDisableTimer)
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

        if (timeRemaining < 10)
        {
            _checkpointTimer.color = new Color(100, 0, 0);
            if (_checkpointTimer.fontSize < 65 && TextSizeFlag == true)
            {
                _checkpointTimer.fontSize += 1;
                if(_checkpointTimer.fontSize == 65)
                {
                    TextSizeFlag = false;
                }
            }
            else if (timeRemaining > 0)
            {
                 _checkpointTimer.fontSize -= 1;
                if(_checkpointTimer.fontSize == 45)
                {
                    TextSizeFlag = true;
                }
            }

        }

        else
            _checkpointTimer.color = new Color(0, 0, 0);

		_checkpointTimer.text = "Time Left: " + timeRemaining.ToString("0.0");

        float distFromCentre = distance.magnitude;
        float distFromEdge = checkpointRadius - distFromCentre;
        checkpointDistance.text = distFromEdge.ToString("0") + "m";

		RoadMapRoot.BroadcastMessage("Extend", false);
	}

	void UpdateCheckpoint()
	{
		checkpointRadius *= 1.5f;
		gameObject.transform.localScale = new Vector3(checkpointRadius * 2, 8.0f, checkpointRadius * 2);
		if (checkpointRadius > 50.0f && checkpointRadius < 256.0f && lods.Length > 1)
			gameObject.GetComponent<MeshFilter>().mesh = lods[1];
		else if (checkpointRadius > 256.0f && checkpointRadius < 1200.0f && lods.Length > 2)
			gameObject.GetComponent<MeshFilter>().mesh = lods[2];
		else if (checkpointRadius > 1200.0f /*&& checkpointRadius < 1200.0f*/ && lods.Length > 3)
			gameObject.GetComponent<MeshFilter>().mesh = lods[3];

		checkpointPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;
		checkpointPosition.y = -0.1f;
		gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

        level++;
        float playerSpeed = 10;
        float fastestVehicleSpeed = 20;
        float lerp = Mathf.Clamp((float)level / 20.0f, 0, 1);
        float speedRequired = Mathf.Lerp(playerSpeed * 0.5f, fastestVehicleSpeed, lerp);
        float extraTime = checkpointRadius / speedRequired;
		timeRemaining += extraTime;
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
