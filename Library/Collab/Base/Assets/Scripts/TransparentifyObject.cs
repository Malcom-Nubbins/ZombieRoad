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

            foreach (MeshRenderer mesh in ObstacleHit.transform.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
                hiddenBuildings.Add(ObstacleHit);
            }
            
        }
        else
        {
            foreach(RaycastHit buildingHit in hiddenBuildings)
            {
                foreach(MeshRenderer mesh in buildingHit.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.enabled = true;
                }
            }

            hiddenBuildings.Clear();
        }
    }
}
