using UnityEngine;
using UnityEngine.UI;

namespace ZR.GUI
{
	public class DisplayScore : MonoBehaviour
	{
		[SerializeField, NonNull] Text scoreText;
		
		void Update()
		{
			scoreText.text = "Score: " + Killcount.GetKills() * 10;
		}
	}
}
