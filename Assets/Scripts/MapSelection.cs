using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
	// public for the editor's sake, fix later
	public string SelectedMap;
	public string GetSelectedMap() { return SelectedMap; }
	public bool SetSelectedMap(string NewSelection) { if (SceneManager.GetSceneByName(NewSelection) != null) { SelectedMap = NewSelection; return true; } return false; }
}
