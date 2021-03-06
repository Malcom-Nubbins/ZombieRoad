﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour {

    GameObject weaponPrefab;

    // Use this for initialization
    void Start ()
    {
        if (GameObject.Find("RoadTileManager") == null)
        {
            // Don't spawn a weapon if there is no roadtilemanager (mainly to stop weapons spawning in the shop screen)
            Destroy(this);
            return;
        }
        if (Random.value < 0.5)
        {
            Destroy(gameObject);
            return;
        }
        if (UnlockManager.instance == null)
        {
            Destroy(this);
        }


        GameObject[] spawnableWeapons = UnlockManager.instance.GetUnlockedItems(UnlockableType.WEAPON);
        weaponPrefab = spawnableWeapons[Random.Range(0, spawnableWeapons.Length)];
        //Debug.Log(weaponPrefab);
        GameObject weapon = Instantiate(weaponPrefab);
        weapon.transform.position = transform.position;
        Destroy(gameObject);

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
