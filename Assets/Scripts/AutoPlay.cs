using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
	void Start()
    {
        
	}
	
	void Update()
    {
        GetComponent<Button>().onClick.Invoke();
    }
}
