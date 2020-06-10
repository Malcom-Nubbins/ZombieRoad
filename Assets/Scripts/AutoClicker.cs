using UnityEngine;
using UnityEngine.UI;

public class AutoClicker : MonoBehaviour
{
	[SerializeField, NonNull] Button button;

	void Update()
	{
		button.onClick.Invoke();
	}
}
