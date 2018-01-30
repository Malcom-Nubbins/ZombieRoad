using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCurrency : MonoBehaviour {

    Text currency;
	// Use this for initialization
	void Start () {
        currency = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        currency.text = "Coins: " + Currency.GetCurrency();
	}
}
