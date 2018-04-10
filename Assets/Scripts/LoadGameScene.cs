using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameScene : MonoBehaviour {

    bool loadingScene;
    float loadWait;
	// Use this for initialization
	void Start () {
        loadWait = 1.0f;
        loadingScene = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (loadWait <= 0.0f)
        {
            if (!loadingScene)
            {
                Scenes.instance.LoadGameScene();
                loadingScene = true;
            }
            return;
        }

        loadWait -= Time.deltaTime;
	}
}
