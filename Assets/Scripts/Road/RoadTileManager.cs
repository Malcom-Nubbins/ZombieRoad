using System.Collections;
using UnityEngine;

namespace ZR.Road
{
	public class RoadTileManager : MonoBehaviour
	{
		public static RoadTileManager instance;

		public static GameObject Grass;
		public static GameObject Pavement;
		public static GameObject Straight;
		public static GameObject Corner;
		public static GameObject T;

		public static GameObject DeadEnd;
		public static GameObject FourWay;

		public static Checkpoint checkpoint;

		[SerializeField, NonNull] GameObject _Grass;
		[SerializeField, NonNull] GameObject _Pavement;
		[SerializeField, NonNull] GameObject _Straight;
		[SerializeField, NonNull] GameObject _Corner;
		[SerializeField, NonNull] GameObject _T;

		[SerializeField, NonNull] GameObject _DeadEnd;
		[SerializeField, NonNull] GameObject _FourWay;

		[SerializeField, NonNull] Checkpoint _Checkpoint;

		[Range(0, 100)]
		[SerializeField] int ChanceStraight;
		[Range(0, 100)]
		[SerializeField] int ChanceCorner;
		[Range(0, 100)]
		[SerializeField] int ChanceT;

		[Range(0, 100)]
		[SerializeField] int ChancePavement;
		[Range(0, 100)]
		[SerializeField] int ChanceGrass;

		[Range(0, 100)]
		public int ChanceBarrier;

		[SerializeField] bool _Cull = true;
		public static bool bCull;

		public static bool bMainMenu;

		public static bool bDebugEnv;

		//int MaxTimeUntilFieldCheck = 450;
		//int TimeUntilFieldCheck;
		TilePosition hitTest;
		Vector3 roundedVec;

		GameObject EmergencyFieldRemover;

		void Awake()
		{
			instance = this;
			Grass = _Grass;
			Pavement = _Pavement;
			Straight = _Straight;
			Corner = _Corner;
			T = _T;

			DeadEnd = _DeadEnd;
			FourWay = _FourWay;

			checkpoint = _Checkpoint;

			bCull = _Cull;

			//TimeUntilFieldCheck = MaxTimeUntilFieldCheck;

			bMainMenu = GetComponent<MainMenuDummyCheckpoint>() != null;

			bDebugEnv = Application.platform == RuntimePlatform.WindowsEditor;

			hitTest = new TilePosition();
			roundedVec = new Vector3();
		}

		void Start()
		{
			StartCoroutine(BatchedUpdate());
		}

		float RoundDownToGrid(float a)
		{
			return WorldTileManager.TILE_SIZE * (int)(a / WorldTileManager.TILE_SIZE);
		}

		Vector3 RoundDownToGrid(Vector3 a)
		{
			roundedVec.x = RoundDownToGrid(a.x);
			roundedVec.y = RoundDownToGrid(a.y);
			roundedVec.z = RoundDownToGrid(a.z);
			return roundedVec;
		}

		private IEnumerator BatchedUpdate()
		{
			while (true)
			{
				if (!bMainMenu || Time.timeSinceLevelLoad < 5.0f)
				{
					if (bMainMenu)
						yield return new WaitForSeconds(0.05f);
					else
					{
						yield return new WaitForSeconds(0.2f);
					}

					WorldTile[] array = WorldTileManager.instance.GetAllTiles();
					for (int i = 0; i < array.Length; ++i)
					{
						WorldTile tile = array[i];
						var generator = tile as RoadGenerator;
						generator?.Extend(false);
					}
				}

				yield return null;
			}
		}

		void FixedUpdate()
		{
			if (Input.GetKey(KeyCode.Keypad0))
				bCull = !bCull;

			Transform playerTransform = checkpoint.FollowCamera.target.transform;

			//Debug.Log(checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>().Length + " 35:" + checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[35].gameObject.name);

			if (checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>().Length > 100 /*usually around 140, prevent checking until the map is up to size*/ && EmergencyFieldRemover == null && checkpoint.RoadMapRoot.GetComponentsInChildren<RoadGenerator>()[35] as DisabledRoadGenerator != null /*35 is around a quarter, meaning grass at this position indicates a map with three quarters grass*/)
			{
				int i = RoadGenerator.Wrap0to7(Mathf.RoundToInt(playerTransform.rotation.eulerAngles.y / 45.0f));

				Vector3 player = RoundDownToGrid(playerTransform.position);
				hitTest.x = Mathf.FloorToInt(player.x / WorldTileManager.TILE_SIZE) + RoadGenerator.Xoffset(i) * 8;
				hitTest.z = Mathf.FloorToInt(player.z / WorldTileManager.TILE_SIZE) + RoadGenerator.Zoffset(i) * 8;
				//Debug.Log("Testing for a hit at grid "+hitTest.x+","+hitTest.z+"; World location "+hitTest.GetWorldPosition());
				WorldTile Hit = WorldTileManager.instance.GetTile(hitTest);

				if (!Hit)
				{
					//Debug.Log("FUCKIN FIELD, heading " + playerTransform.rotation.eulerAngles.y + "≈" + (RoadGenerator.Direction)i);

					EmergencyFieldRemover = Instantiate(FourWay, hitTest.GetWorldPosition() + new Vector3(0, FourWay.GetComponent<RoadGenerator>().YOffset, 0), Quaternion.identity, checkpoint.RoadMapRoot.transform);
					//Debug.Log("EFR: "+EmergencyFieldRemover.gameObject.name + " at " + (hitTest.GetWorldPosition() + new Vector3(0, FourWay.GetComponent<RoadGenerator>().YOffset, 0)));
					WorldTileManager.instance.AddTile(EmergencyFieldRemover.GetComponent<WorldTile>());
					EmergencyFieldRemover.transform.SetAsFirstSibling();
					RoadGenerator newRG = EmergencyFieldRemover.GetComponent<RoadGenerator>();
					newRG.CullingExempt = true;
					newRG.RefreshExits();
					//newRG.Extend(true);
					//checkpoint.RoadMapRoot.BroadcastMessage("Extend", true);
				}
			}
			else if // Emergency Field Remover has come within the invisible boundary lines (σ回ω・)σ
				((EmergencyFieldRemover != null && Vector3.Distance(playerTransform.position, EmergencyFieldRemover.transform.position) < checkpoint.FollowCamera.CullDistance)
				|| // Emergency Field Remover is no longer in front of the player
				(EmergencyFieldRemover != null && Vector3.Dot(playerTransform.position - EmergencyFieldRemover.transform.position, playerTransform.forward) > 90))
			{
				EmergencyFieldRemover.GetComponent<RoadGenerator>().CullingExempt = false;
				EmergencyFieldRemover = null;
			}
		}

		public static GameObject RandomRoadTile(bool bAllowQuad)
		{
			int r = Random.Range(0, 100);

			if (r < instance.ChanceCorner)
			{
				return Corner;
			}
			else if (r < instance.ChanceCorner + instance.ChanceT)
			{
				return bAllowQuad ? FourWay : T;
			}

			return Straight;
		}

		public static GameObject RandCornerT(bool bAllowQuad)
		{
			int r = Random.Range(0, instance.ChanceCorner + instance.ChanceT);

			if (r < instance.ChanceCorner)
			{
				return bAllowQuad ? T : Corner;
			}

			return bAllowQuad ? FourWay : T;
		}

		public static GameObject RandStraightT()
		{
			int r = Random.Range(0, instance.ChanceStraight + instance.ChanceT);

			if (r < instance.ChanceStraight)
			{
				return Straight;
			}

			return T;
		}

		public static GameObject RandQuadT()
		{
			//int r = Random.Range(0, instance.ChanceStraight + instance.ChanceT);

			//if (r < instance.ChanceStraight)
			//{
			//	return FourWay;
			//}

			return FourWay;
		}

		public static GameObject RandCornerDead()
		{
			int r = Random.Range(0, instance.ChanceCorner + instance.ChanceT);

			if (r < instance.ChanceT)
			{
				return Corner;
			}

			return DeadEnd;
		}

		public static GameObject RandPavementGrass()
		{
			int r = Random.Range(0, 100);

			if (r < instance.ChanceGrass)
			{
				return Grass;
			}

			return Pavement;
		}

		public void OnValidate()
		{
			if (ChanceStraight + ChanceCorner + ChanceT != 100)
				Debug.LogError("ChanceStraight + ChanceCorner + ChanceT ≠ 100");
			else
				Debug.Log("Road chances are fine");

			if (ChanceGrass + ChancePavement != 100)
				Debug.LogError("ChanceGrass + ChancePavement ≠ 100");
			else
				Debug.Log("Pavement vs Grass chances are fine");
		}
	}
}
