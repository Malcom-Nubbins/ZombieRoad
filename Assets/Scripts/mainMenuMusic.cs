using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuMusic : MonoBehaviour
{

    AudioSource musicSource;
    GameObject musicBox;
    // Use this for initialization
    void OnEnable()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MusicBox");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        musicSource = this.gameObject.GetComponent<AudioSource>();
       // musicSource.Play();
        musicBox = this.gameObject;
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");

        DontDestroyOnLoad(musicBox);

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(SceneManager.GetActiveScene().name =="Sbuburbs")
        {
            musicSource.Stop();
        }
        else
        {
            if(!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
}
