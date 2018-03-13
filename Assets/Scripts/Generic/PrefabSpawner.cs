using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    
	void Awake()
    {
        GameObject obj = Instantiate(prefab, transform);
        //obj.transform.parent = transform;
        //obj.transform.localPosition = Vector3.zero;
        //obj.transform.localRotation = Quaternion.identity;
	}
}
