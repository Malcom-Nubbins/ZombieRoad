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
        if (scene == Scenes.Scene.SHOP)
        {
            if (UnlockManager.instance.GetLockedItemCount() < 1)
            {
                startButton.interactable = false;
            }
        }
    }

    void OnClick()
    {
        if(scene == Scenes.Scene.SHOP)
        {
            if(UnlockManager.instance.GetLockedItemCount() < 1)
            {
                return;
            }
        }

        if(scene == Scenes.Scene.GAME)
        {
            Scenes.instance.LoadGameScene();
            return;
        }
        //print("clicked");
        Scenes.instance.LoadScene(scene);
    }
}
