using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    GameObject zombie;
    float speed = 30.0f;
    float step;
    Transform bulletTransform;
    bool weaponShot = false;





    // Use this for initialization
    void Start ()
    {

        bulletTransform = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update ()
    {

        //Debug.Log("WEAPON POS: " + WeaponUsed.GetComponent<Transform>().position);
        if(weaponShot)
        {

            GetComponent<Transform>().position = Vector3.MoveTowards(bulletTransform.position, zombie.GetComponent<Transform>().position, step);
            if(Vector3.Distance(bulletTransform.position, zombie.GetComponent<Transform>().position) < 0.5)
            {

                weaponShot = false;
                Destroy(gameObject);

            }
        }

//        Debug.Log("ASDASDadsasaddsadsa");
//        Debug.Log(zombie);
        step = speed * Time.deltaTime;
        // Vector3.MoveTowards(Bullet.GetComponent<Transform>().position, zombie.GetComponent<Transform>().position, step);
        //Bullet.transform.position = Vector3.MoveTowards(Weapon.transform.position, zombie.transform.position, step);
        //Debug.Log(zombie);
        transform.rotation = Quaternion.identity;
    }


    public void ShootBullet(GameObject AttackedZombie)
    {
        zombie = AttackedZombie;
        weaponShot = true;
        //Debug.Log("TARGET ACQUIRED! LAUNCHING BULLET");
        
    }

}
