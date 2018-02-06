using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOnDeath : MonoBehaviour
{
    Rigidbody rb;
    bool dead = false;
    float timeDead = 0;
    Vector3 axisToRotateAround;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Health>().onDeath += OnDeath;
        SetAxisToRotateAround(Vector3.zero);
	}

    void Update()
    {
        float fallTime = 0.5f;
        if (dead)
        {
            timeDead += Time.deltaTime;
            if (timeDead <= fallTime)
            {
                if (axisToRotateAround == Vector3.zero)
                {
                    axisToRotateAround = transform.right;
                }
                transform.Rotate(axisToRotateAround, (-90.0f / fallTime) * Time.deltaTime, Space.World);
            }
        }
    }

    void OnDeath()
    {
        dead = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!dead) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            Vector3 pointOnFloor = new Vector3(transform.position.x, 0.5f, transform.position.z);
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out hitInfo, 10, LayerMask.NameToLayer("Road")))
            {
                pointOnFloor = hitInfo.point;
            }
            transform.position = pointOnFloor;
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void SetAxisToRotateAround(Vector3 axis)
    {
        axisToRotateAround = axis;
    }
}
