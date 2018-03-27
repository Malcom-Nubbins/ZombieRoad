using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	public static BuildingManager instance;

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
		return unlockedSkyscrapers[r];
	}
	public static GameObject RandomHouse()
    {
        GameObject[] unlockedHouses = UnlockManager.instance.GetUnlockedItems(UnlockableType.HOUSE);
        int r = Random.Range(0, unlockedHouses.Length);
		return unlockedHouses[r];
	}
	public static GameObject RandomShop()
    {
        GameObject[] unlockedShops = UnlockManager.instance.GetUnlockedItems(UnlockableType.SHOP);
        //if (unlockedShops.Length == 0) return Shops[0];
        int r = Random.Range(0, unlockedShops.Length);
		return unlockedShops[r];
	}

	public static bool IsSkyscraper(GameObject o)
	{
		if (o.GetComponent<Unlockable>().type==UnlockableType.SKYSCRAPER) return true;
		
		return false;
	}
	public static bool IsHouse(GameObject o)
	{
		if (o.GetComponent<Unlockable>().type == UnlockableType.HOUSE) return true;

		return false;
	}
	public static bool IsShop(GameObject o)
	{
		if (o.GetComponent<Unlockable>().type == UnlockableType.SHOP) return true;

		return false;
	}
}
