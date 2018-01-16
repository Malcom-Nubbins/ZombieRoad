using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBloodTexture : MonoBehaviour
{
    Material withoutBloodMaterial;
    public Material withBloodMaterial;
    MeshRenderer[] meshRenderers;

	void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        withoutBloodMaterial = meshRenderers[0].material;
	}
	
	void Update()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Blood"))
        {
            ApplyBlood(true);
        }
    }

    void ApplyBlood(bool blood)
    {
        Material material = blood ? withBloodMaterial : withoutBloodMaterial;
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = material;
        }
    }
}
