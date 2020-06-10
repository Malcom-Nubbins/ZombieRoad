using UnityEngine;

public class LoadGameScene : MonoBehaviour
{
	bool loadingScene;
	float loadWait;
	
	void Start()
	{
		loadWait = 1.0f;
		loadingScene = false;
	}

	void Update()
	{
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
