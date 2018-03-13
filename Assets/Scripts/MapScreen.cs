using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour
{
	MapSelection _maps;
	int _currentSelectedItem;

	Text _itemNameText;

	Button purchaseButton;

	float _currentCooldownTime = 0.0f;

	RawImage mapPreview;

	// Use this for initialization
	void Start()
	{
		_maps = GameObject.Find("UnlockManager").GetComponent<MapSelection>();
		_currentSelectedItem = 0;

		_itemNameText = GameObject.Find("ItemNameText").GetComponent<Text>();
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];

		purchaseButton = GameObject.Find("Purchase").GetComponent<Button>();
		purchaseButton.onClick.AddListener(onPurchaseClick);

		mapPreview = GameObject.Find("MapPreview").GetComponent<RawImage>();
		mapPreview.texture = _maps.AvailableMapImages[_currentSelectedItem];
	}

	void onPurchaseClick()
	{
		bool succ = _maps.SetSelectedMap(_maps.AvailableMapNames[_currentSelectedItem]);

		if (succ)
		{
			Scenes.instance.LoadScene(Scenes.Scene.LOADING);
		}
		else
		{
			Debug.LogError("BAD MAP - " + _maps.AvailableMapNames[_currentSelectedItem]);
		}
	}

	void onNextClick()
	{
		_currentCooldownTime = 0.3f;

		_currentSelectedItem++;
		if (_currentSelectedItem > _maps.AvailableMapNames.Length - 1)
			_currentSelectedItem = 0;

		mapPreview.texture = _maps.AvailableMapImages[_currentSelectedItem];
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];
	}

	void onPrevClick()
	{
		_currentCooldownTime = 0.3f;

		_currentSelectedItem--;
		if (_currentSelectedItem < 0)
			_currentSelectedItem = _maps.AvailableMapNames.Length - 1;

		mapPreview.texture = _maps.AvailableMapImages[_currentSelectedItem];
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];
	}

	// Update is called once per frame
	void Update()
	{

		if (_currentCooldownTime <= 0.0f)
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					if (touch.position.x < Screen.width / 3)
					{
						onPrevClick();
					}
					else if (touch.position.x > (Screen.width / 3) * 2)
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
	}
}
