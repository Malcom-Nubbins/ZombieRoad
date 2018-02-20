using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour
{

	GameObject _lockedItemDisplay;
	MapSelection _maps;
	int _currentSelectedItem;

	Text _itemNameText;

	Button purchaseButton;

	float _currentCooldownTime = 0.0f;

	Sprite mapPreview;
	SpriteRenderer mpRenderer;

	Vector2 oo;

	// Use this for initialization
	void Start()
	{
		_maps = GameObject.Find("UnlockManager").GetComponent<MapSelection>();
		_currentSelectedItem = 0;

		_itemNameText = GameObject.Find("ItemNameText").GetComponent<Text>();
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];

		purchaseButton = GameObject.Find("Purchase").GetComponent<Button>();
		purchaseButton.onClick.AddListener(onPurchaseClick);

		oo = new Vector2(0, 0);
		mapPreview = Sprite.Create(_maps.AvailableMapImages[_currentSelectedItem], new Rect(0.0f, 0.0f, _maps.AvailableMapImages[_currentSelectedItem].width, _maps.AvailableMapImages[_currentSelectedItem].height), oo);

		mpRenderer = GameObject.Find("MapPreview").GetComponent<SpriteRenderer>();
		mpRenderer.sprite = mapPreview;
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
			Debug.Log("BAD MAP - " + _maps.AvailableMapNames[_currentSelectedItem]);
		}
	}

	void onNextClick()
	{
		_currentCooldownTime = 0.3f;
		Destroy(_lockedItemDisplay);
		_currentSelectedItem++;
		if (_currentSelectedItem > _maps.AvailableMapNames.Length - 1)
			_currentSelectedItem = 0;

		mapPreview = null;
		mapPreview = Sprite.Create(_maps.AvailableMapImages[_currentSelectedItem], new Rect(0.0f, 0.0f, _maps.AvailableMapImages[_currentSelectedItem].width, _maps.AvailableMapImages[_currentSelectedItem].height), oo);
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];
		mpRenderer.sprite = mapPreview;
	}

	void onPrevClick()
	{
		_currentCooldownTime = 0.3f;
		Destroy(_lockedItemDisplay);
		_currentSelectedItem--;
		if (_currentSelectedItem < 0)
			_currentSelectedItem = _maps.AvailableMapNames.Length - 1;

		mapPreview = null;
		mapPreview = Sprite.Create(_maps.AvailableMapImages[_currentSelectedItem], new Rect(0.0f, 0.0f, _maps.AvailableMapImages[_currentSelectedItem].width, _maps.AvailableMapImages[_currentSelectedItem].height), oo);
		_itemNameText.text = _maps.AvailableMapNames[_currentSelectedItem];
		mpRenderer.sprite = mapPreview;
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
