using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public enum Scene
    {
        LOADING = 0,
        GAME = 1,
        UNLOCK = 2,
        GAME_OVER = 3,
        SHOP = 4,
		MAP = 5,
        SETTINGS = 6
    }
    public string[] sceneNames;

    public static Scenes instance;

    private void OnEnable()
    {
        instance = this;
    }
	
	public string GetSceneName(Scene scene)
    {
        return (scene == Scene.GAME) ? UnlockManager.instance.gameObject.GetComponent<MapSelection>().GetSelectedMap() : sceneNames[(int)scene];
    }

    public void LoadScene(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
		if (scene == Scene.GAME && UnlockManager.instance.gameObject.GetComponent<MapSelection>().GetSelectedMap() == "EdSheeran") Destroy(UnlockManager.instance);
		SceneManager.LoadScene(GetSceneName(scene), mode);
    }
}
