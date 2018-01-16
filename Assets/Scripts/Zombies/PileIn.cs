using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileIn : MonoBehaviour
{
    Animator animator;
    public Vector3 target;

	void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Pile In");
        GetComponent<SeekPlayer>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
	}
	
	void Update()
    {
        transform.position += (target - transform.position) * Time.deltaTime;
	}
}
