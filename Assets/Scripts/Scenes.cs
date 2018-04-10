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

    public Scene GetCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        for (int scene = 0; scene < sceneNames.Length; scene++)
        {
            if (sceneNames[scene].Equals(sceneName))
            {
                return (Scene)scene;
            }
        }
        Debug.Log("Current scene named " + sceneName + " does not exist in scene names list, defaulting to main menu");
        return Scene.LOADING;
    }
	
	public string GetSceneName(Scene scene)
    {
        return (scene == Scene.GAME) ? UnlockManager.instance.gameObject.GetComponent<MapSelection>().GetSelectedMap() : sceneNames[(int)scene];
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadGameSceneAsync());
    }

    IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation gameLoad = SceneManager.LoadSceneAsync(GetSceneName(Scene.GAME));

        while(!gameLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
		SceneManager.LoadSceneAsync(GetSceneName(scene), mode);
    }
}
