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
        RaycastHit ObstacleHit;
        Physics.Raycast(player.position, -_camera.transform.forward, out ObstacleHit, Mathf.Infinity, LayerMask.GetMask("Building"));
        
        if (ObstacleHit.collider)
        {
            //Debug.Log("Object collided with: " + ObstacleHit.collider.gameObject.name);
            if(!buildings.Contains(ObstacleHit.collider.gameObject))
            {
                buildings.Add(ObstacleHit.collider.gameObject);
            }
            else
            {
                if(ObstacleHit.collider.gameObject != buildings[0])
                {
                    foreach(Renderer mesh in buildings[0].GetComponentsInChildren<Renderer>())
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

                    buildings.RemoveAt(0);
                }
                foreach (Renderer meshRenderer in ObstacleHit.collider.gameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material material in meshRenderer.materials)
                    {
                        //material.SetInt("_ZWrite", 0);
                        if(material.color.a >= 0.4f)
                        {
                            material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a - 0.05f);
                        }
                       
                    }
                }
            }
        }
        else
        {
            foreach(GameObject buildingHit in buildings)
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

                //foreach (MeshRenderer mesh in buildingHit.transform.GetComponentsInChildren<MeshRenderer>())
                //{
                //    mesh.enabled = true;
                //}
            }

            buildings.Clear();
        }
    }
}
