using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    GameObject equippedWeaponObject;


    public HandWeapon weapon;
	// Use this for initialization
	void Start()
	{
        
	}

	// Update is called once per frame
	void Update()
	{
        if(equippedWeaponObject == null)
        {
            equippedWeaponObject = GameObject.Find("EquippedWeapon");
        }
        else
        {
            EquippedWeapon equipped = equippedWeaponObject.GetComponent<EquippedWeapon>();
		    if(equipped.equippedWeapon!=null && equipped.equippedWeapon.getDurability() <= 0)
		    {
			    dropWeapon();
		    }
        }
		

	}

	void OnCollisionEnter(Collision collision)
	{
		//collision.gameObject is the player
		if (collision.gameObject.tag == "Player" && weapon.getDurability() > 0)
		{
			equipWeapon();
		}
	}


	public void equipWeapon()
	{
		GameObject equippedWeaponObject = GameObject.Find("EquippedWeapon");
		EquippedWeapon equipped = equippedWeaponObject.GetComponent<EquippedWeapon>();

		if (equipped.equippedWeapon != null && (equipped.equippedWeapon.getWeaponName() == weapon.getWeaponName()))
		{

			freezeRigidbody();
			return;

		} else {

			if (equipped.equippedWeapon != null)
			{
				dropWeapon();
			}

		}


		//remove components
		Destroy(GetComponent<Rigidbody>());
		Destroy(GetComponent<MeshCollider>());

		//set weapon position to parents
		Transform equippedWeaponTransform = equippedWeaponObject.GetComponent<Transform>();
		Transform groundWeaponTransform = GetComponent<Transform>();

		groundWeaponTransform.SetPositionAndRotation(equippedWeaponTransform.position, equippedWeaponTransform.rotation);
		groundWeaponTransform.parent = equippedWeaponTransform;

		//assign the equipped weapon
		equipped.equippedWeapon = weapon;
    }

	public void dropWeapon()
	{

		GameObject equippedWeaponObject = GameObject.Find("EquippedWeapon");
		GameObject playerObject = GameObject.Find("PlayerCharacter");

		//should only ever have 1 child so fetching 0 is fine
		Transform child = equippedWeaponObject.transform.GetChild(0);
		GameObject childObject = child.gameObject;

		//add mesh collider
		childObject.AddComponent<MeshCollider>();
		childObject.GetComponent<MeshCollider>().convex = true;

		//set drop position
		child.SetPositionAndRotation(child.transform.position + (-playerObject.transform.forward * 5), child.transform.rotation);

		//assign new parent
		GameObject objects = GameObject.Find("Objects");
		childObject.transform.parent = objects.transform;

		//remove the equpped weapon
		EquippedWeapon equipped = equippedWeaponObject.GetComponent<EquippedWeapon>();
        if (equipped.equippedWeapon.getDurability() <= 0)
        {
            Destroy(childObject);
        }
		equipped.equippedWeapon = null;
       // Debug.Log(equippedWeaponObject.transform.GetChild(0));
        //Destroy(childObject);

	
	}

	public void freezeRigidbody()
	{
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.velocity = new Vector3();
		rigidbody.angularVelocity = new Vector3();
		rigidbody.ResetInertiaTensor();
		rigidbody.rotation = Quaternion.Euler(new Vector3());

		//freezes rigidbody so it doesn't interact with the player as mesh collider is still active
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

}
