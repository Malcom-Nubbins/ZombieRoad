using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TransparentifyObject : MonoBehaviour
{
    public Transform player;

    private List<RaycastHit> hiddenBuildings;
    private List<GameObject> buildings;
    private Camera _camera;

	// Use this for initialization
	void Start () {
        hiddenBuildings = new List<RaycastHit>();
        buildings = new List<GameObject>();
        player = GetComponent<FollowCamera>().target.transform;
        _camera = GetComponent<Camera>();
	}

	private IEnumerator FadeOutRoutine(GameObject building)
	{
		foreach(Renderer meshRenderer in building.GetComponentsInChildren<Renderer>())
		{
			foreach(Material material in meshRenderer.materials)
			{
				for(float f = 1.0f; f >= 0.4f; f -= 0.05f)
				{
					Color c = material.color;
					c.a = f;
					material.color = c;
					yield return null;
				}
			}
		}
	}

	private IEnumerator FadeInRoutine(GameObject building)
	{
		foreach (Renderer mesh in building.GetComponentsInChildren<Renderer>())
		{
			foreach (Material material in mesh.materials)
			{
				material.SetInt("_ZWrite", 1);
				for(float f = material.color.a; f <= 1.0f; f += 0.05f)
				{
					Color c = material.color;
					c.a = f;
					material.color = c;
					yield return null;
				}
			}
		}
	}

	void Update ()
    {
        RaycastHit[] ObstacleHit = Physics.RaycastAll(player.position, -_camera.transform.forward, Mathf.Infinity, LayerMask.GetMask("Building"));
        
        foreach(RaycastHit hit in ObstacleHit)
        {
            if (!buildings.Contains(hit.collider.gameObject))
            {
                buildings.Add(hit.collider.gameObject);

				StartCoroutine(FadeOutRoutine(hit.collider.gameObject));
            }            
        }

        List<GameObject> hits = new List<GameObject>();
        for(int i = 0; i < ObstacleHit.Length; ++i)
        {
            hits.Add(ObstacleHit[i].collider.gameObject);
        }

        foreach(GameObject buildingHit in buildings)
        {
            if(!hits.Contains(buildingHit))
            {
				StartCoroutine(FadeInRoutine(buildingHit));    
            }
        }

        if(ObstacleHit.Length < 1)
           buildings.Clear();
        
    }
}
