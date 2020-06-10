using UnityEngine;
using UnityEngine.UI;

public class SettingMasterVolume : MonoBehaviour
{
	[SerializeField, NonNull] Slider masterVolumeSlider;
	bool setup;

	void Start()
	{
		setup = true;
		masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", AudioListener.volume);
	}

	public void ChangeVolume(float volume)
	{
		AudioListener.volume = volume;
		PlayerPrefs.SetFloat("MasterVolume", volume);
		PlayerPrefs.Save();
	}
}
