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

    private float checkpointRadius;
    private float timeRemaining;

    private float checkpointRotationSpeed;
    private Text _checkpointTimer;

    void Start()
    {
        checkpointPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;

        checkpointRadius = gameObject.transform.localScale.x / 2.0f;
        gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

        timeRemaining = 10;

        checkpointRotationSpeed = 50.0f;

        _checkpointTimer = GameObject.Find("CheckpointTimerText").GetComponent<Text>();
        _checkpointTimer.text = "Time Left: " + timeRemaining;
    }

    void Update()
    {
   
        //rotate the checkpoint
        Transform checkpointTransform = GetComponent<Transform>();
        checkpointTransform.Rotate((Vector3.up * checkpointRotationSpeed) * Time.deltaTime);

        Vector3 playerPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;
        Vector3 distance = playerPosition - checkpointPosition;

        if (distance.magnitude >= checkpointRadius)
        {
            createCheckpoint();
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            Debug.Log("Player ran out of time!");
            _checkpointTimer.text = "Out of time!";
            //end/restart game?
            // CancelInvoke("decreaseTimeRemaining");
            timeRemaining = 0;

            PlayerDeath player = GameObject.Find("PlayerCharacter").GetComponent<PlayerDeath>();
            player.killPlayer();
            return;
        }

        _checkpointTimer.text = "Time Left: " + timeRemaining.ToString("0.0");

        RoadMapRoot.BroadcastMessage("Extend", this);
    }

    void createCheckpoint()
    {
        checkpointRadius *= 1.5f;
        gameObject.transform.localScale = new Vector3(checkpointRadius * 2, 5.0f, checkpointRadius * 2);

        checkpointPosition = FollowCamera.GetComponent<FollowCamera>().target.transform.position;
        gameObject.transform.SetPositionAndRotation(checkpointPosition, gameObject.transform.rotation);

        timeRemaining += 10;


    }


}
