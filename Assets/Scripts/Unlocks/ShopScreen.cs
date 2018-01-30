using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour {

    GameObject _lockedItemDisplay;
    Unlockable[] _lockedItems;
    int _currentSelectedItem;

    Text _itemNameText;

    float _currentCooldownTime = 0.0f;

	// Use this for initialization
	void Start () {
        UnlockManager um = GameObject.Find("UnlockManager").GetComponent<UnlockManager>();
        _lockedItems = um.GetLockedItems();
        _currentSelectedItem = 0;

        GameObject prefab = _lockedItems[_currentSelectedItem].gameObject;
        _lockedItemDisplay = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);

        _itemNameText = GameObject.Find("ItemNameText").GetComponent<Text>();
        _itemNameText.text = _lockedItemDisplay.name.Substring(0, _lockedItemDisplay.name.IndexOf('('));
    }
	
    void onNextClick()
    {
        _currentCooldownTime = 0.3f;
        Destroy(_lockedItemDisplay);
        _currentSelectedItem++;
        if (_currentSelectedItem > _lockedItems.Length - 1)
            _currentSelectedItem = 0;

        GameObject prefab = _lockedItems[_currentSelectedItem].gameObject;
        Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        _lockedItemDisplay = Instantiate(prefab, Vector3.zero, transform.rotation, transform);
        _itemNameText.text = _lockedItemDisplay.name.Substring(0, _lockedItemDisplay.name.IndexOf('('));
    }

    void onPrevClick()
    {
        _currentCooldownTime = 0.3f;
        Destroy(_lockedItemDisplay);
        _currentSelectedItem--;
        if (_currentSelectedItem < 0)
            _currentSelectedItem = _lockedItems.Length - 1;

        GameObject prefab = _lockedItems[_currentSelectedItem].gameObject;
        Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        _lockedItemDisplay = Instantiate(prefab, Vector3.zero, transform.rotation, transform);
        _itemNameText.text = _lockedItemDisplay.name.Substring(0, _lockedItemDisplay.name.IndexOf('('));
    }

	// Update is called once per frame
	void Update () {
        
        if(_currentCooldownTime <= 0.0f)
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.position.x < Screen.width / 2)
                    {
                        onPrevClick();
                    }
                    else
                    {
                        onNextClick();
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
                onPrevClick();

            if (Input.GetKey(KeyCode.RightArrow))
                onNextClick();
        }
        else
        {
            _currentCooldownTime -= Time.deltaTime;
        }

        Vector3 buildingScale = new Vector3(0.35f, 0.35f, 0.35f);
        Vector3 otherBuildingScale = new Vector3(0.7f, 0.7f, 0.7f);

        transform.Rotate(Vector3.up, 20 * Time.deltaTime);
        if (_lockedItemDisplay)
        {
            if (_lockedItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SKYSCRAPER)
            {
                _lockedItemDisplay.transform.localScale = buildingScale;
            }
            else if (_lockedItemDisplay.GetComponent<Unlockable>().type == UnlockableType.HOUSE || _lockedItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SHOP)
            {
                _lockedItemDisplay.transform.localScale = otherBuildingScale;
            }
            else
            {
                _lockedItemDisplay.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            _lockedItemDisplay.transform.position = new Vector3(0.0f, -8.0f, 20.0f);
        }
    }
}
