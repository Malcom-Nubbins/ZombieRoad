using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour {

    const string PP_CURRENCY = "currency";
    static int currency;
    // Use this for initialization
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

    public static void AddCurrency()
    {
        currency++;
        Save();
    }

    public static int GetCurrency()
    {
        return currency;
    }

    public static void AddCurrency(int numOfCoins)
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
