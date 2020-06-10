using UnityEngine;

public class Currency : MonoBehaviour
{
	const string PP_CURRENCY = "currency";
	static int currency;
	
	private void OnEnable()
	{
		Load();
	}

	private void OnDisable()
	{
		Save();
	}

	static void Reset()
	{
		currency = 0;
	}

	void Load()
	{
		currency = PlayerPrefs.GetInt(PP_CURRENCY);
	}

	static void Save()
	{
		PlayerPrefs.SetInt(PP_CURRENCY, currency);
		PlayerPrefs.Save();
	}

	public static int GetCurrency()
	{
		return currency;
	}

	public static void AddCurrency(int numOfCoins = 1)
	{
		currency += numOfCoins;
		Save();
	}

	public static void RemoveCurrency(int amount)
	{
		currency -= amount;
		Save();
	}
}
