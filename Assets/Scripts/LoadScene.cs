using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
	[SerializeField, NonNull] Button button;
	[SerializeField, FormerlySerializedAs("_scene")] Scenes.Scene scene;

	void Start()
	{
		button.onClick.AddListener(OnClick);
		if (scene == Scenes.Scene.SHOP)
		{
			if (UnlockManager.instance.GetLockedItemCount() < 1)
			{
				button.interactable = false;
			}
		}
	}

	void OnClick()
	{
		if (scene == Scenes.Scene.SHOP)
		{
			if (UnlockManager.instance.GetLockedItemCount() < 1)
			{
				return;
			}
		}

		if (scene == Scenes.Scene.GAME)
		{
			Scenes.instance.LoadGameScene();
			return;
		}
		//print("clicked");
		Scenes.instance.LoadScene(scene);
	}
}
