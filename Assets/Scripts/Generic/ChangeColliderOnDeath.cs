using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColliderOnDeath : MonoBehaviour
{
    public Collider aliveCollider;
    public Collider deadCollider;
    
	void Start()
    {
        GetComponent<Health>().onDeath += () =>
        {
            if (enabled)
            {
                aliveCollider.enabled = false;
                deadCollider.enabled = true;
            }
        };
	}
}
