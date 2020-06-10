using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
	static MainMenuMusic inst;

	[SerializeField, NonNull] AudioSource musicSource;
	
	void Start()
	{
		if (inst != null && inst != this)
		{
			Destroy(gameObject);
			return;
		}
		inst = this;

		// musicSource.Play();
		AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");

		DontDestroyOnLoad(gameObject);
	}
	
	void Update()
	{
		if (SceneManager.GetActiveScene().name == "Sbuburbs" || SceneManager.GetActiveScene().name == "Somerset")
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
