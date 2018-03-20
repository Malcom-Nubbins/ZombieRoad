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
        startButton.onClick.AddListener(OnClick);

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnClick()
    {
       // Debug.Log("CLICKED! Init: " + Advertisement.isInitialized + " TestMode?: " + Advertisement.testMode);
       // Debug.Log(Advertisement.IsReady("rewardedVideo"));
        if (Advertisement.IsReady("rewardedVideo"))
        {
           // Debug.Log("AD IS READY TO SHOw");
            var options = new ShowOptions { resultCallback = HandleShowingAd };
            Advertisement.Show(options);
        }
        
    }

    void HandleShowingAd(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //Debug.Log(" AD SHOWN SUCCESSFULLY");
                // give reward, coins used shop maybe???

                Currency.AddCurrency(5);
                break;

            case ShowResult.Skipped:
                //Debug.Log("USER SKIPPED ADVERT NEED TO RESET THEIR PROGRESS !!!!!!!");
                break;

            case ShowResult.Failed:
               // Debug.Log("AD FAILED TO DISPLAY");
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
            var options = new ShowOptions { resultCallback = HandleShowingAd };
            Advertisement.Show(options);
        }
    }
}
