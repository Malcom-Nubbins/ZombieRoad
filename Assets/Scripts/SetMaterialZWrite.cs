using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialZWrite : MonoBehaviour {

    public Material[] skyScraperMaterials;
    public Material[] houseMaterials;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < skyScraperMaterials.Length; i++)
        {
            skyScraperMaterials[i].SetInt("_ZWrite", 1);
        }

        for (int i = 0; i < houseMaterials.Length; i++)
        {
            houseMaterials[i].SetInt("_ZWrite", 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
