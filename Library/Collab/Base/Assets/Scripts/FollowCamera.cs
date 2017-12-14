using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
	public float CullDistance;


	void Start()
    {
    }
        
    void LateUpdate()
    {
        if (target)
        {
            float desiredAngle = target.transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

            transform.position = target.transform.position + (rotation * offset);
            //transform.RotateAround(target.transform.position, Vector3.up, difference);
            transform.LookAt(target.transform);

            if (Input.GetKey(KeyCode.Keypad1))
				offset.y--;
			if (Input.GetKey(KeyCode.Keypad2))
				offset.z--;
			if (Input.GetKey(KeyCode.Keypad3))
				offset.y++;
			if (Input.GetKey(KeyCode.Keypad8))
				offset.z++;
			if (Input.GetKey(KeyCode.Keypad6))
				offset.x++;
			if (Input.GetKey(KeyCode.Keypad4))
				offset.x--;
			if (Input.GetKey(KeyCode.Keypad5))
				offset = new Vector3(0, 20, -10);
		}
    }
}
