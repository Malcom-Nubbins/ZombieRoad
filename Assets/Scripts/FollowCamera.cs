using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
	public float CullDistance;
    public bool rotate = true;

	void Start()
    {
    }
        
    void LateUpdate()
    {
        if (target)
        {
            if (rotate)
            {
                float desiredAngle = target.transform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

                transform.position = target.transform.position + (rotation * offset);
                transform.LookAt(target.transform);
            }
            else
            {
                transform.position = target.transform.position + offset;
            }

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
				offset = new Vector3(0, 33, -31);
		}
    }
}
