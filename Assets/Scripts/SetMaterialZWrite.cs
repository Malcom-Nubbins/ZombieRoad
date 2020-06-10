using UnityEngine;

public class SetMaterialZWrite : MonoBehaviour
{
	Object[] buildingMaterials;
	// Use this for initialization
	void Start()
	{
		buildingMaterials = Resources.LoadAll("Models/Buildings/Materials/", typeof(Material));

		for (int i = 0; i < buildingMaterials.Length; ++i)
		{
			Material mat = (Material)buildingMaterials[i];
			mat.SetInt("_ZWrite", 1);
		}
	}
}
