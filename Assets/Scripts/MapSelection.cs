using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
	[SerializeField] private string _selectedMap;
	public string GetSelectedMap() { return _selectedMap; }
	public bool SetSelectedMap(string NewSelection) { if (SceneManager.GetSceneByName(NewSelection) != null) { _selectedMap = NewSelection; return true; } return false; }

	public string[] AvailableMapNames;
	public Texture2D[] AvailableMapImages;
}
