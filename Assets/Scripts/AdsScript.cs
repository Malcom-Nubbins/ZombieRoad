using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class AdsScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Button startButton = GetComponent<Button>();
        if(startButton)
        {
            startButton.onClick.AddListener(OnClick);
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnClick()
    {
       // Debug.Log("CLICKED! Init: " + Advertisement.isInitialized + " TestMode?: " + Advertisement.testMode);
       // Debug.Log(Advertisement.IsReady("rewardedVideo"));
        if (Advertisement.IsReady("video"))
        {
           // Debug.Log("AD IS READY TO SHOw");
            var options = new ShowOptions { resultCallback = HandleShopAd };
            Advertisement.Show(options);
        }
        
    }

    void HandleDeathAd(ShowResult result)
    {
        switch(result)
        {
            case ShowResult.Finished:
                int randomNumber = (int)Random.Range(0.0f, 10.0f);

                if(randomNumber == 0)
                {
                    if (UnlockManager.instance.GetLockedItemCount() > 0)
                    {
                        Scenes.instance.LoadScene(Scenes.Scene.UNLOCK);
                    }
                    else
                    {
                        Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
                    }
                }
                else
                {
                    Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
                }

                break;

            case ShowResult.Skipped:
                Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
                break;

            case ShowResult.Failed:
                Scenes.instance.LoadScene(Scenes.Scene.GAME_OVER);
                break;
        }
    }

    void HandleShopAd(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //Debug.Log(" AD SHOWN SUCCESSFULLY");
                // give reward, coins used shop maybe???

                Currency.AddCurrency(5);
                Scenes.instance.LoadScene(Scenes.Scene.SHOP);
                break;

            case ShowResult.Skipped:
                //Debug.Log("USER SKIPPED ADVERT NEED TO RESET THEIR PROGRESS !!!!!!!");
                Scenes.instance.LoadScene(Scenes.Scene.SHOP);
                break;

            case ShowResult.Failed:
                // Debug.Log("AD FAILED TO DISPLAY");
                Scenes.instance.LoadScene(Scenes.Scene.SHOP);
                break;
        }


    }

    public void PlayAdOnDeath()
    {
       // Debug.Log("Playing skippable ad on death Init: " + Advertisement.isInitialized + " TestMode?: " + Advertisement.testMode);
       // Debug.Log("is skippable ad avaiable to show?? :" +Advertisement.IsReady("video"));

        if (Advertisement.IsReady("video"))
        {
           // Debug.Log("SKIPPABLE ADD IS SHOWING");
            var options = new ShowOptions { resultCallback = HandleDeathAd };
            Advertisement.Show(options);
        }
    }
}
