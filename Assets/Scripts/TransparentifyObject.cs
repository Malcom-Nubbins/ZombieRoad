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

	void Update ()
    {
       
        //Debug.Log("Update is being called");
        RaycastHit[] ObstacleHit = Physics.RaycastAll(player.position, -_camera.transform.forward, Mathf.Infinity, LayerMask.GetMask("Building")); ;
        //Physics.RaycastAll(player.position, -_camera.transform.forward, out ObstacleHit, Mathf.Infinity, LayerMask.GetMask("Building"));
        
        foreach(RaycastHit hit in ObstacleHit)
        {
            if (!buildings.Contains(hit.collider.gameObject))
            {
                buildings.Add(hit.collider.gameObject);
            }

            foreach (Renderer meshRenderer in hit.collider.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in meshRenderer.materials)
                {
                    //material.SetInt("_ZWrite", 0);
                    if (material.color.a >= 0.4f)
                    {
                        material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a - 0.05f);
                    }

                }
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
                foreach (Renderer mesh in buildingHit.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material material in mesh.materials)
                    {
                        material.SetInt("_ZWrite", 1);
                        if (material.color.a <= 1.0f)
                        {
                            material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
                        }
                    }
                }
            }
            //foreach (MeshRenderer mesh in buildingHit.transform.GetComponentsInChildren<MeshRenderer>())
            //{
            //    mesh.enabled = true;
            //}
        }

        if(ObstacleHit.Length < 1)
           buildings.Clear();
        
    }
}
