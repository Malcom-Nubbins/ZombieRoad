using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDummyCheckpoint : Checkpoint
{

	// Use this for initialization
	override public void Start() { level = 10; }

	// Update is called once per frame
	override public void Update() {}

	override public void UpdateCheckpoint() { }
}