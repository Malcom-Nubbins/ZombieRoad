using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDummyCheckpoint : Checkpoint
{

	// Use this for initialization
	override public void Start() { }

	// Update is called once per frame
	override public void Update() { RoadMapRoot.BroadcastMessage("Extend", false); }

	override public void UpdateCheckpoint() { }
}