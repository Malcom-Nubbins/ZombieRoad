using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
	static MainMenuMusic inst;

	[SerializeField, NonNull] AudioSource musicSource;
	
	void OnEnable()
	{
		if (inst != null && inst != this)
		{
			Destroy(this);
		}

		// musicSource.Play();
		AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");

		DontDestroyOnLoad(gameObject);
		inst = this;
	}
	
	void Update()
	{
		if (SceneManager.GetActiveScene().name == "Sbuburbs")
		{
			musicSource.Stop();
		}
		else
		{
			if (!musicSource.isPlaying)
			{
				musicSource.Play();
			}
		}
	}
}
