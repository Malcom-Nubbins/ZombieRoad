using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentifyObject : MonoBehaviour
{
    public Transform player;

    private List<RaycastHit> hiddenBuildings;

	// Use this for initialization
	void Start () {
        hiddenBuildings = new List<RaycastHit>();
        player = GetComponent<FollowCamera>().target.transform;
	}
	
	void Update ()
    {

        //Debug.Log("Update is being called");
        RaycastHit ObstacleHit;
        Physics.Raycast(player.position, -transform.forward, out ObstacleHit, Mathf.Infinity, LayerMask.GetMask("Building"));
        
        if (ObstacleHit.collider)
        {
            //Debug.Log("Object collided with: " + ObstacleHit.collider.gameObject.name);
            if(!ObstacleHit.collider.gameObject.name.ToLower().Contains("shop"))
            {
                foreach (Renderer meshRenderer in ObstacleHit.collider.gameObject.GetComponentsInChildren<Renderer>())
                {
                    meshRenderer.material.SetInt("_ZWrite", 0);
                    meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0.1f);
                    hiddenBuildings.Add(ObstacleHit);
                }

                //foreach (MeshRenderer mesh in ObstacleHit.transform.GetComponentsInChildren<MeshRenderer>())
                //{
                //    mesh.enabled = false;
                //    if(!hiddenBuildings.Contains(ObstacleHit))
                //    {
                //        hiddenBuildings.Add(ObstacleHit);
                //    }
                //}
            }
        }
        else
        {
            foreach(RaycastHit buildingHit in hiddenBuildings)
            {
                foreach (Renderer mesh in buildingHit.collider.gameObject.GetComponentsInChildren<Renderer>())
                {
                    mesh.material.SetInt("_ZWrite", 1);
                    mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, 1.0f);
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
