using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Scenes.Scene scene;

	void Start()
	{
        Button startButton = GetComponent<Button>();
        startButton.onClick.AddListener(OnClick);
	}

    void OnClick()
    {
        //print("clicked");
        Scenes.instance.LoadScene(scene);
    }
}
