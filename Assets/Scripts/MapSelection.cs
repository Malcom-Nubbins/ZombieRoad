using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
	[SerializeField, NonEmpty] string selectedMap;
	public string GetSelectedMap() { return selectedMap; }
	public bool SetSelectedMap(string NewSelection) { if (Application.CanStreamedLevelBeLoaded(NewSelection)) { selectedMap = NewSelection; return true; } return false; }

	public string[] AvailableMapNames;
	public Texture2D[] AvailableMapImages;
}
