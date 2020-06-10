using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour
{
	MapSelection maps;
	int currentSelectedItem;

	[SerializeField, NonNull] private TextMeshProUGUI mapName;
	[SerializeField, NonNull] private RawImage mapPreview;
	[SerializeField, NonNull] private Button selectMapButton;

	float currentCooldownTime = 0.0f;

	// Use this for initialization
	void Start()
	{
		maps = GameObject.Find("UnlockManager").GetComponent<MapSelection>();
		currentSelectedItem = 0;

		mapName.text = maps.AvailableMapNames[currentSelectedItem];

		mapPreview.texture = maps.AvailableMapImages[currentSelectedItem];
	}

	public void OnPurchaseClick()
	{
		bool succ = maps.SetSelectedMap(maps.AvailableMapNames[currentSelectedItem]);

		if (succ)
		{
			Scenes.instance.LoadScene(Scenes.Scene.LOADING);
		}
		else
		{
			Debug.LogError("BAD MAP - " + maps.AvailableMapNames[currentSelectedItem]);
		}
	}

	void OnNextClick()
	{
		currentCooldownTime = 0.3f;

		currentSelectedItem++;
		if (currentSelectedItem > maps.AvailableMapNames.Length - 1)
			currentSelectedItem = 0;

		mapPreview.texture = maps.AvailableMapImages[currentSelectedItem];
		mapName.text = maps.AvailableMapNames[currentSelectedItem];
	}

	void OnPrevClick()
	{
		currentCooldownTime = 0.3f;

		currentSelectedItem--;
		if (currentSelectedItem < 0)
			currentSelectedItem = maps.AvailableMapNames.Length - 1;

		mapPreview.texture = maps.AvailableMapImages[currentSelectedItem];
		mapName.text = maps.AvailableMapNames[currentSelectedItem];
	}

	void Update()
	{
		if (currentCooldownTime <= 0.0f)
		{
			if (Input.touchCount > 0)
			{
				for (int i = 0; i < Input.touches.Length; ++i)
				{
					var touch = Input.touches[i];
					if (touch.position.x < Screen.width / 3 && touch.position.y < Screen.height / 3 * 2 && touch.position.y > Screen.height / 4)
					{
						OnPrevClick();
					}
					else if (touch.position.x > Screen.width / 3 * 2 && touch.position.y < Screen.height / 3 * 2 && touch.position.y > Screen.height / 4)
					{
						OnNextClick();
					}
				}
			}

			if (Input.GetKey(KeyCode.LeftArrow))
				OnPrevClick();

			if (Input.GetKey(KeyCode.RightArrow))
				OnNextClick();
		}
		else
		{
			currentCooldownTime -= Time.deltaTime;
		}
	}
}
