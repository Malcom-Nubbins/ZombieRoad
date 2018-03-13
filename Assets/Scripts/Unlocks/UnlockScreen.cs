using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockScreen : MonoBehaviour
{
    GameObject newItemDisplay;
    Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    void Start()
    {
        UnlockManager um = GameObject.Find("UnlockManager").GetComponent<UnlockManager>();
        Unlockable newItem = um.UnlockRandom();
        if (newItem == null)
        {
            SceneManager.LoadScene("GameOverScene");
            return;
        }
        GameObject prefab = newItem.gameObject;
        newItemDisplay = Instantiate(prefab, Vector3.zero, rotation, transform);
	}
	
	void Update()
    {
        Vector3 buildingScale = new Vector3(0.30f, 0.30f, 0.30f);
        Vector3 otherBuildingScale = new Vector3(0.65f, 0.65f, 0.65f);

        transform.Rotate(Vector3.up, 20 * Time.deltaTime);
        if (newItemDisplay)
        {
            if (newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SKYSCRAPER)
            {
                newItemDisplay.transform.localScale = buildingScale;
            }
            else if (newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.HOUSE || newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SHOP)
            {
                newItemDisplay.transform.localScale = otherBuildingScale;
            }
            else
            {
                newItemDisplay.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            newItemDisplay.transform.position = new Vector3(0.0f, -8.0f, 20.0f);
        }
    }
}
