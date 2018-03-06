using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentifyObject : MonoBehaviour
{
    public Transform player;

    private List<RaycastHit> hiddenBuildings;
    private Camera _camera;

	// Use this for initialization
	void Start () {
        hiddenBuildings = new List<RaycastHit>();
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
           
            foreach (Renderer meshRenderer in ObstacleHit.collider.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach(Material material in meshRenderer.materials)
                {
                    material.SetInt("_ZWrite", 0);
                    material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);
                }
                hiddenBuildings.Add(ObstacleHit);
            }
        }
        else
        {
            foreach(RaycastHit buildingHit in hiddenBuildings)
            {
                foreach (Renderer mesh in buildingHit.collider.gameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material material in mesh.materials)
                    {
                        material.SetInt("_ZWrite", 1);
                        material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
                    }
                }

                //foreach (MeshRenderer mesh in buildingHit.transform.GetComponentsInChildren<MeshRenderer>())
                //{
                //    mesh.enabled = true;
                //}
            }

            hiddenBuildings.Clear();
        }
    }
}
