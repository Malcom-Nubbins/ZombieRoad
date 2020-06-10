using UnityEngine;

public class AutoMainMenu : MonoBehaviour
{
	enum State
	{
		START,
		FIRST_MAIN_MENU,
		DONE
	}
	static AutoMainMenu instance;
	State state = State.START;
	Scenes.Scene returnToScene;

	void OnEnable()
	{
		if (instance)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	void Update()
	{
		switch (state)
		{
			case State.START:
				if (Scenes.instance.GetCurrentScene() == Scenes.Scene.LOADING)
				{
					Destroy(gameObject); // starting on main menu, don't need to do anything
					return;
				}
				DontDestroyOnLoad(gameObject);
				returnToScene = Scenes.instance.GetCurrentScene();
				state = State.FIRST_MAIN_MENU;
				Scenes.instance.LoadScene(Scenes.Scene.LOADING); // load main menu for first time
				break;
			case State.FIRST_MAIN_MENU:
				state = State.DONE;
				Scenes.instance.LoadScene(returnToScene);
				break;
			case State.DONE:
				Destroy(gameObject);
				break;
		}
	}
}
