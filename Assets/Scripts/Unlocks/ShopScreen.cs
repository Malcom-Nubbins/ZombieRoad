using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour {

    UnlockManager um;

    GameObject _lockedItemDisplay;
    Unlockable[] _lockedItems;
    int _currentSelectedItem;

    Text _itemNameText;
    Text _itemCostText;
    Text _currentCoinsText;

    Button purchaseButton;

    float _currentCooldownTime = 0.0f;

	Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

	// Use this for initialization
	void Start()
	{
        um = GameObject.Find("UnlockManager").GetComponent<UnlockManager>();
        
        _itemNameText = GameObject.Find("ItemNameText").GetComponent<Text>();
        _itemCostText = GameObject.Find("ItemPriceText").GetComponent<Text>();
        _currentCoinsText = GameObject.Find("CurrentCoinsText").GetComponent<Text>();

        _itemNameText.text = "";
        _itemCostText.text = "Price: -";
        _currentCoinsText.text = "Coins: " + Currency.GetCurrency();

        purchaseButton = GameObject.Find("Purchase").GetComponent<Button>();
        purchaseButton.onClick.AddListener(onPurchaseClick);

        RefreshItems();
    }

    void RefreshItems()
    {
        _lockedItems = um.GetLockedItems();
        
        if (_currentSelectedItem >= _lockedItems.Length)
        {
            _currentSelectedItem = _lockedItems.Length - 1;
        }
        
        RefreshSelectedItem();
    }
    
    void RefreshSelectedItem()
    {
        Destroy(_lockedItemDisplay);
        if (_currentSelectedItem >= 0 && _currentSelectedItem < _lockedItems.Length)
        {
            GameObject prefab = _lockedItems[_currentSelectedItem].gameObject;
            _lockedItemDisplay = Instantiate(prefab, Vector3.zero, rotation, transform);
            Collider collider = _lockedItemDisplay.GetComponentInChildren<Collider>();
            float objectSize = 10.0f;
            if (collider) objectSize = collider.bounds.size.magnitude;
            float displaySize = 6.0f;
            float scale = (displaySize / objectSize);
            _lockedItemDisplay.transform.localScale = Vector3.one * scale;
            _lockedItemDisplay.transform.localPosition -= Vector3.up * (scale / 2.0f);

            if (_lockedItemDisplay.GetComponent<SeekPlayer>() != null)
            {
                _lockedItemDisplay.GetComponent<SeekPlayer>().enabled = false;
            }
            if (_lockedItemDisplay.GetComponent<Rigidbody>())
            {
                _lockedItemDisplay.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        _currentCoinsText.text = "Coins: " + Currency.GetCurrency();
        _itemNameText.text = _lockedItemDisplay.name.Substring(0, _lockedItemDisplay.name.IndexOf('('));
        _itemCostText.text = "Price: " + _lockedItems[_currentSelectedItem].Price + " coins";
    }

    void onPurchaseClick()
    {
        if (_lockedItems.Length == 0)
        {
            return;
        }
        Unlockable item = _lockedItems[_currentSelectedItem];
        if (Currency.GetCurrency() >= item.Price)
        {
            //UnlockManager.instance.Unlo
            Currency.RemoveCurrency(item.Price);

            um.UnlockItem(item);

            RefreshItems();
        }
    }
	
    void onNextClick()
    {
        if (_lockedItems.Length == 0)
        {
            return;
        }
        _currentCooldownTime = 0.3f;
        _currentSelectedItem++;
        if (_currentSelectedItem > _lockedItems.Length - 1)
            _currentSelectedItem = 0;
        RefreshSelectedItem();
    }

    void onPrevClick()
    {
        if (_lockedItems.Length == 0)
        {
            return;
        }
        _currentCooldownTime = 0.3f;
        _currentSelectedItem--;
        if (_currentSelectedItem < 0)
            _currentSelectedItem = _lockedItems.Length - 1;
        RefreshSelectedItem();
    }

	// Update is called once per frame
	void Update()
	{
        
        if(_currentCooldownTime <= 0.0f)
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.position.x < Screen.width / 3 && (touch.position.y < (Screen.height / 3) * 2 && touch.position.y > (Screen.height / 4)))
                    {
                        onPrevClick();
                    }
                    else if(touch.position.x > (Screen.width / 3) * 2 && (touch.position.y < (Screen.height / 3) * 2 && touch.position.y > (Screen.height / 4)))
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

        //Vector3 buildingScale = new Vector3(0.30f, 0.30f, 0.30f);
        //Vector3 otherBuildingScale = new Vector3(0.65f, 0.65f, 0.65f);

        if (_lockedItemDisplay) _lockedItemDisplay.transform.Rotate(Vector3.up, 20 * Time.deltaTime);
    }
}
