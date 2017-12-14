using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbOnVehicle : MonoBehaviour
{
    BaseVehicleClass vehicleAttachedTo;
    Rigidbody vehicleRigidbody;

    Rigidbody rb;
    Vector3 localStartClimbPosition;
    Vector3 localEndClimbPosition;
    Vector3 localEndPosition;
    bool climbing = false;
    float climbingPercentDone = 0;
    float climbingSpeed = 1.0f / 3.0f;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
        vehicleAttachedTo = null;
        GetComponent<Health>().onDeath += () =>
        {
            DetachFromVehicle();
            enabled = false;
        };
	}

	void Update()
    {
        if (vehicleAttachedTo != null)
        {
            if (climbing)
            {
                float percentageOfClimbingTimeSpentOnSide = 0.7f;
                float lerp;
                Vector3 start, end;
                if (climbingPercentDone <= percentageOfClimbingTimeSpentOnSide)
                {
                    lerp = climbingPercentDone / percentageOfClimbingTimeSpentOnSide;
                    start = localStartClimbPosition;
                    end = localEndClimbPosition;
                }
                else
                {
                    lerp = (climbingPercentDone - percentageOfClimbingTimeSpentOnSide) / (1.0f - percentageOfClimbingTimeSpentOnSide);
                    start = localEndClimbPosition;
                    end = localEndPosition;
                }
                Vector3 localTargetPos = Vector3.Lerp(start, end, lerp);
                transform.position = vehicleAttachedTo.transform.TransformPoint(localTargetPos);

                Debug.DrawLine(vehicleAttachedTo.transform.TransformPoint(localStartClimbPosition), vehicleAttachedTo.transform.TransformPoint(localEndClimbPosition), Color.red);

                climbingPercentDone += climbingSpeed * Time.deltaTime;
                if (climbingPercentDone >= 1.0f)
                {
                    climbing = false;
                    transform.parent = vehicleAttachedTo.transform;
                    Physics.IgnoreCollision(vehicleAttachedTo.GetComponent<Collider>(), GetComponent<ChangeColliderOnDeath>().aliveCollider, false);
                    rb.constraints &= ~RigidbodyConstraints.FreezePosition;
                }
            }
            else
            {
                transform.position += (vehicleAttachedTo.transform.TransformPoint(localEndPosition) - transform.position).normalized * 1.0f * Time.deltaTime;
                if (vehicleAttachedTo.GetDriver() == null)//driver has got out
                {
                    DetachFromVehicle();
                }
            }
            if (vehicleRigidbody && Vector3.Distance(transform.position, vehicleRigidbody.ClosestPointOnBounds(transform.position)) > 1.0f)
            {
                DetachFromVehicle();
            }
        }
	}

    void AttachToVehicle(BaseVehicleClass vehicle)
    {
        vehicleAttachedTo = vehicle;
        //transform.parent = vehicleAttachedTo.transform;
        vehicleAttachedTo.AddZombieOnRoof(gameObject);
        GetComponent<SeekPlayer>().enabled = false;
        if (GetComponent<DieWhenHitByVehicle>()) GetComponent<DieWhenHitByVehicle>().enabled = false;
        rb.constraints |= RigidbodyConstraints.FreezePosition;
        Physics.IgnoreCollision(vehicleAttachedTo.GetComponent<Collider>(), GetComponent<ChangeColliderOnDeath>().aliveCollider);
        climbing = true;
        climbingPercentDone = 0;

        vehicleRigidbody = vehicleAttachedTo.GetComponent<Rigidbody>();
        localStartClimbPosition = vehicleAttachedTo.transform.InverseTransformPoint(transform.position);
        localEndClimbPosition = localStartClimbPosition;
        localEndClimbPosition.y = 5.0f;
        localEndPosition = localEndClimbPosition * 0.5f;//towards centre of vehicle
        localEndPosition.y = localEndClimbPosition.y;
        //set end pos to point actually touching top of vehicle
        localEndPosition = vehicleAttachedTo.transform.InverseTransformPoint(vehicleRigidbody.ClosestPointOnBounds(vehicleAttachedTo.transform.TransformPoint(localEndPosition)));
        localEndClimbPosition.y = localEndPosition.y;
    }

    public void DetachFromVehicle()
    {
        if (vehicleAttachedTo == null) return;
        transform.parent = null;
        vehicleAttachedTo.RemoveZombieFromRoof(gameObject);
        GetComponent<SeekPlayer>().enabled = true;
        if (GetComponent<DieWhenHitByVehicle>()) GetComponent<DieWhenHitByVehicle>().enabled = true;
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
        Physics.IgnoreCollision(vehicleAttachedTo.GetComponent<Collider>(), GetComponent<ChangeColliderOnDeath>().aliveCollider, false);
        climbing = false;
        vehicleAttachedTo = null;
        vehicleRigidbody = null;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!enabled) return;
        if (vehicleAttachedTo == null)
        {
            BaseVehicleClass vehicle = collision.gameObject.GetComponent<BaseVehicleClass>();
            if (vehicle)
            {
                if (vehicle.GetDriver() != null)
                {
                    if (vehicle.speed < 5)
                    {
                        if (vehicle.GetNumZombiesOnRoof() < 5)
                        {
                            AttachToVehicle(vehicle);
                        }
                    }
                }
            } 
        }
    }
}
