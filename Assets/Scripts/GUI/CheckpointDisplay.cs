using UnityEngine;
using ZR.Chkpnt;

public class CheckpointDisplay : MonoBehaviour
{
	public RectTransform playerDisplayImage;

	BaseCheckpoint checkpoint;
	FollowCamera followCam;
	float displayRadius;

	void Start()
	{
		checkpoint = GameObject.Find("CheckpointManager").GetComponent<BaseCheckpoint>();
		followCam = checkpoint.FollowCamera;
		displayRadius = GetComponent<RectTransform>().rect.width / 2;
	}
	
	void Update()
	{
		Vector3 checkpointPos = checkpoint.transform.position;
		Vector3 playerPos = followCam.target.transform.position;
		Vector2 offset = new Vector2(playerPos.x - checkpointPos.x, playerPos.z - checkpointPos.z);
		offset /= checkpoint.GetRadius();//between 0 and 1
		offset *= displayRadius;
		playerDisplayImage.localPosition = new Vector3(offset.x, offset.y, 0);
		playerDisplayImage.localRotation = Quaternion.AngleAxis(-followCam.target.transform.eulerAngles.y + 90, Vector3.forward);
	}
}
