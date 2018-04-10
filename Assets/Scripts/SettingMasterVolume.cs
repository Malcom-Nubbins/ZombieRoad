using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMasterVolume : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Slider masterVolumeSlider = GetComponent<Slider>();
        masterVolumeSlider.value = AudioListener.volume;
        masterVolumeSlider.onValueChanged.AddListener(changeVolume);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");

    }

    private void changeVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
