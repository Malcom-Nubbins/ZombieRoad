using UnityEngine;

namespace ZR.Unlocks
{
	public class SettingsResetUnlocks : MonoBehaviour
	{
		public void OnClick()
		{
			UnlockManager.instance.ResetUnlocks();
		}
	}
}