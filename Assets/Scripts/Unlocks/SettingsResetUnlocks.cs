using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsResetUnlocks : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button reset = GetComponent<Button>();
        reset.onClick.AddListener(onClick);
	}
	
    void onClick()
    {
        UnlockManager.instance.ResetUnlocks();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
