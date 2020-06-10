using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour
{
	[SerializeField] private TextMeshPro m_MapName;
	[SerializeField] private RawImage m_MapPreview;
	[SerializeField] private Button m_SelectMapButton;

	private MapSelection m_Maps;
	int _currentSelectedItem;

	float _currentCooldownTime = 0.0f;

	// Use this for initialization
	void Start()
	{
		m_Maps = GameObject.Find("UnlockManager").GetComponent<MapSelection>();
		_currentSelectedItem = 0;
		m_MapName.text = m_Maps.AvailableMapNames[_currentSelectedItem];
		m_SelectMapButton.onClick.AddListener(onPurchaseClick);
		m_MapPreview.texture = m_Maps.AvailableMapImages[_currentSelectedItem];
	}

	void onPurchaseClick()
	{
		bool succ = m_Maps.SetSelectedMap(m_Maps.AvailableMapNames[_currentSelectedItem]);

		if (succ)
		{
			Scenes.instance.LoadScene(Scenes.Scene.LOADING);
		}
		else
		{
			Debug.LogError("BAD MAP - " + m_Maps.AvailableMapNames[_currentSelectedItem]);
		}
	}

	void onNextClick()
	{
		_currentCooldownTime = 0.3f;

		_currentSelectedItem++;
		if (_currentSelectedItem > m_Maps.AvailableMapNames.Length - 1)
			_currentSelectedItem = 0;

		m_MapPreview.texture = m_Maps.AvailableMapImages[_currentSelectedItem];
		m_MapName.text = m_Maps.AvailableMapNames[_currentSelectedItem];
	}

	void onPrevClick()
	{
		_currentCooldownTime = 0.3f;

		_currentSelectedItem--;
		if (_currentSelectedItem < 0)
			_currentSelectedItem = m_Maps.AvailableMapNames.Length - 1;

		m_MapPreview.texture = m_Maps.AvailableMapImages[_currentSelectedItem];
		m_MapName.text = m_Maps.AvailableMapNames[_currentSelectedItem];
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
                    if (touch.position.x < Screen.width / 3 && (touch.position.y < (Screen.height / 3) * 2 && touch.position.y > (Screen.height / 4)))
                    {
                        onPrevClick();
                    }
                    else if (touch.position.x > (Screen.width / 3) * 2 && (touch.position.y < (Screen.height / 3) * 2 && touch.position.y > (Screen.height / 4)))
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
