using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	public static BuildingManager instance;

	public static GameObject[] Skyscrapers;
	public static GameObject[] Houses;
	public static GameObject[] Shops;

	public GameObject[] _Skyscrapers;
	public GameObject[] _Houses;
	public GameObject[] _Shops;

	[Range(0, 100)]
	public int ChanceSkyscraper;
	[Range(0, 100)]
	public int ChanceHouses;
	[Range(0, 100)]
	public int ChanceShops;
	[Range(0, 100)]
	public int ChanceNothing;

	void Awake()
	{
		instance = this;
        
		Skyscrapers = _Skyscrapers;
		Houses = _Houses;
		Shops = _Shops;
	}
	public void OnValidate()
	{
		if (ChanceSkyscraper + ChanceHouses + ChanceShops + ChanceNothing != 100)
			Debug.LogError("ChanceSkyscraper + ChanceHouses + ChanceShops + ChanceNothing ≠ 100");
		else
			Debug.Log("Building chances are fine");
	}

	public static GameObject RandomBuilding()
	{
		int r = Random.Range(0, 100);

		if (r < instance.ChanceSkyscraper)
		{
			return RandomSkyscraper();
		}
		else if (r < instance.ChanceSkyscraper + instance.ChanceHouses)
		{
			return RandomHouse();
		}
		else if (r < instance.ChanceSkyscraper + instance.ChanceHouses + instance.ChanceShops)
		{
			return RandomShop();
		}

		return null;
	}

	public static GameObject RandomShopOrHouse()
	{
		int r = Random.Range(0, instance.ChanceHouses+instance.ChanceShops);

		if (r < instance.ChanceHouses)
		{
			return RandomHouse();
		}

		return RandomShop();
	}

	public static GameObject RandomSkyscraper()
	{
        GameObject[] unlockedSkyscrapers = UnlockManager.instance.GetUnlockedItems(UnlockableType.SKYSCRAPER);
		int r = Random.Range(0, unlockedSkyscrapers.Length);
		return !unlockedSkyscrapers[r]?Skyscrapers[0]:unlockedSkyscrapers[r];
	}
	public static GameObject RandomHouse()
    {
        GameObject[] unlockedHouses = UnlockManager.instance.GetUnlockedItems(UnlockableType.HOUSE);
        int r = Random.Range(0, unlockedHouses.Length);
		return !unlockedHouses[r] ? Houses[0] : unlockedHouses[r];
	}
	public static GameObject RandomShop()
    {
        GameObject[] unlockedShops = UnlockManager.instance.GetUnlockedItems(UnlockableType.SHOP);
        int r = Random.Range(0, unlockedShops.Length);
		return !unlockedShops[r] ? Shops[0] : unlockedShops[r];
	}

	public static bool IsSkyscraper(GameObject o)
	{
		if (o.name.Contains("sky")) return true;
		
		return false;
	}
	public static bool IsHouse(GameObject o)
	{
		if (o.name.Contains("hous")) return true;

		return false;
	}
	public static bool IsShop(GameObject o)
	{
		if (o.name.Contains("shop")) return true;

		return false;
	}
}
