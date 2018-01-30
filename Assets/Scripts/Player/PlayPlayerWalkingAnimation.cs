using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPlayerWalkingAnimation : MonoBehaviour
{
    Animator animator;

	void Start()
    {
        animator = GetComponent<Animator>();
	}
	
	void Update()
    {
        bool moving = Movement.InputLeft() || Movement.InputRight();
        animator.SetBool("Walking", moving);
	}
}
