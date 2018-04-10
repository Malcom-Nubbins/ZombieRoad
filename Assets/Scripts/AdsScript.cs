using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using Assets.Scripts.GUI;

public class AdsScript : MonoBehaviour
{
	string toastString = "Could not load ad. Please check internet connection.";
	string AndroidGameID = "1741339";
	string iOSGameID = "1741340";

	// Use this for initialization
	void Start ()
	{
		Button startButton = GetComponent<Button>();
		if(startButton)
		{
			startButton.onClick.AddListener(OnClick);
		}

	}

	public void OnClick()
	{
		// Debug.Log("CLICKED! Init: " + Advertisement.isInitialized + " TestMode?: " + Advertisement.testMode);
		// Debug.Log(Advertisement.IsReady("rewardedVideo"));
		if (Advertisement.isInitialized)
		{
			if (Advertisement.IsReady("rewardedVideo"))
			{
				// Debug.Log("AD IS READY TO SHOw");
				var options = new ShowOptions { resultCallback = HandleShopAd };
				Advertisement.Show(options);
			}
			else
			{
				DisplayToast.ShowToast(toastString);
			}
		}
	}

	void HandleDeathAd(ShowResult result)
	{
		switch(result)
		{
			case ShowResult.Finished:
			case ShowResult.Failed:
				if (UnlockManager.instance.GetLockedItemCount() > 0)
				{
					Scenes.instance.LoadScene(Scenes.Scene.UNLOCK);
				}
				else
				{
					Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
				}
				break;
			case ShowResult.Skipped:
			default:
				Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
				break;
		}
	}

	void HandleShopAd(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Currency.AddCurrency(5);
				Scenes.instance.LoadScene(Scenes.Scene.SHOP);
				break;

			case ShowResult.Skipped:
			case ShowResult.Failed:
			default:
				Scenes.instance.LoadScene(Scenes.Scene.SHOP);
				break;
		}
	}

	public void PlayAdOnDeath()
	{
		// Debug.Log("Playing skippable ad on death Init: " + Advertisement.isInitialized + " TestMode?: " + Advertisement.testMode);
		// Debug.Log("is skippable ad avaiable to show?? :" +Advertisement.IsReady("video"));
		if (Advertisement.isInitialized)
		{
			if (Advertisement.IsReady("video"))
			{
				// Debug.Log("SKIPPABLE AD IS SHOWING");
				var options = new ShowOptions { resultCallback = HandleDeathAd };
				Advertisement.Show(options);
			}
			else
			{
				// If ads can't load, go straight to the Unlock screen.
				if (UnlockManager.instance.GetLockedItemCount() > 0)
				{
					Scenes.instance.LoadScene(Scenes.Scene.UNLOCK);
				}
				else
				{
					Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
				}
			}
		}
		else
		{
			// if unity ads are not initialized ( happens when app is launched offline )
			// try to initlialize again and move to game over scene
			InitAds();
			if (UnlockManager.instance.GetLockedItemCount() > 0)
			{
				Scenes.instance.LoadScene(Scenes.Scene.UNLOCK);
			}
			else
			{
				Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
			}
		}
	}

	public void InitAds()
	{
		WWW testWebsite = new WWW("http://google.com");
		if (testWebsite.error == null)
		{
#if UNITY_ANDROID
				Advertisement.Initialize(AndroidGameID);
#elif UNITY_IOS
				Advertisment.Initialize(iOSGameID);
#endif
		}
		else
		{
			DisplayToast.ShowToast(toastString);
		}
	}
}
