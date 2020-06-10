using UnityEngine;

public class Killcount : MonoBehaviour
{
	const string PP_KILLS = "kills";

	static int kills;

	void OnEnable()
	{
		Load();
	}

	void OnDisable()
	{
		Save();
	}

	void Load()
	{
		kills = PlayerPrefs.GetInt(PP_KILLS, 0);
	}

	void Save()
	{
		PlayerPrefs.SetInt(PP_KILLS, kills);
	}

	public static void Reset()
	{
		kills = 0;
	}

	public static void AddKill()
	{
		kills++;
	}

	public static int GetKills()
	{
		return kills;
	}
}
