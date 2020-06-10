using UnityEngine;
using UnityEngine.UI;

public class DisplayKillcount : MonoBehaviour
{
	[SerializeField, NonNull] Text text;

	void Update()
	{
		text.text = "Zombies Killed: " + Killcount.GetKills();
	}
}
