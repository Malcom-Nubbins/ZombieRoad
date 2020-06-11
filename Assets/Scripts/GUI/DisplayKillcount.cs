using UnityEngine;
using UnityEngine.UI;

namespace ZR.GUI
{
	public class DisplayKillcount : MonoBehaviour
	{
		[SerializeField, NonNull] Text text;

		void Update()
		{
			text.text = "Zombies Killed: " + Killcount.GetKills();
		}
	}
}
