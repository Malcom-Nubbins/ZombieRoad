using System;
using UnityEngine;
namespace ZR.Chkpnt
{
	public class BaseCheckpoint : MonoBehaviour
	{
		public static int Level { get; protected set; } = 0;

		// persistent reference to the camera, from which we can retreive the active playercharacter or vehicle
		[NonNull] public FollowCamera FollowCamera;
		[NonNull] public Transform RoadMapRoot;

		[SerializeField] protected float checkpointRadius;

		public event Action OnCheckpointExtend;

		// Use this for initialization
		public virtual void Start() { Level = 10; }

		// Update is called once per frame
		public virtual void Update() { }

		public virtual void UpdateCheckpoint() { }

		protected void InvokeOnCheckpointExtend()
		{
			OnCheckpointExtend?.Invoke();
		}

		public float GetRadius()
		{
			return checkpointRadius;
		}
	}
}
