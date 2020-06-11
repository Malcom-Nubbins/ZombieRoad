using UnityEngine;
using UnityEngine.UI;

namespace ZR.GUI
{
	public class DisplayCurrency : MonoBehaviour
	{
		[SerializeField, NonNull] Text currency;

		void Update()
		{
			currency.text = "Coins: " + Currency.GetCurrency();
		}
	}
}
