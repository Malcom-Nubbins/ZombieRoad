using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockScreen : MonoBehaviour
{
    GameObject newItemDisplay;
	void Start()
    {
        UnlockManager um = GameObject.Find("UnlockManager").GetComponent<UnlockManager>();
        Unlockable newItem = um.UnlockRandom();
        if (newItem == null)
        {
            SceneManager.LoadScene("LoadingScene");
            return;
        }
        GameObject prefab = newItem.gameObject;
        newItemDisplay = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
	}
	
	void Update()
    {
        Vector3 buildingScale = new Vector3(0.35f, 0.35f, 0.35f);
        Vector3 otherBuildingScale = new Vector3(0.7f, 0.7f, 0.7f);

        transform.Rotate(Vector3.up, 20 * Time.deltaTime);
        if (newItemDisplay)
        {
            if(newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SKYSCRAPER)
            {
                newItemDisplay.transform.localScale = buildingScale;
            }
            else if(newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.HOUSE || newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SHOP)
            {
                newItemDisplay.transform.localScale = otherBuildingScale;
            }
            else
            {
                newItemDisplay.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            newItemDisplay.transform.position = new Vector3(0.0f, -5.0f, 20.0f);
        }
	}
}
