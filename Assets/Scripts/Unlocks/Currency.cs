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
        currency = PlayerPrefs.GetInt(PP_CURRENCY, 0);
    }

    void Save()
    {
        PlayerPrefs.SetInt(PP_CURRENCY, currency);
    }

    public static void AddCurrency()
    {
        currency++;
    }

    public static int GetCurrency()
    {
        return currency;
    }
}
