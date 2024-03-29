﻿//using System;
using UnityEngine;

namespace ZR.Road
{
	public class RoadGenerator : WorldTile
	{
		public enum Direction : int
		{
			North,
			NorthEast,
			East,
			SouthEast,
			South,
			SouthWest,
			West,
			NorthWest
		}

		// road exit directions
		public bool PosX;
		public bool NegX;
		public bool PosZ;
		public bool NegZ;
		public bool[] Exit;

		// for newly placed prefabs
		public float YOffset;

		//RoadTileManager Tiles;

		public bool DebugLogs = false;

		[Multiline]
		public string MySpecificDebug;

		protected bool bHaveExpanded;

		protected WorldTile[] hit = new WorldTile[8];
		protected WorldTile[] hitPlus = new WorldTile[8]; // north of north, east of east, etc.

		protected int[] indices;

		public bool CullingExempt;

		protected Vector3 CachedPlayerPosition, CurrentPlayerPosition;

		private TilePosition tp;
		private static FollowCamera followCamera;

		// Use this for initialization
		void Start()
		{
			tp = new TilePosition();
			RefreshExits();
		}

		void Awake()
		{
		}

		void Update()
		{
			if (!followCamera) followCamera = RoadTileManager.checkpoint.FollowCamera;
			if (!CullingExempt
				&& RoadTileManager.bCull
				&& Vector3.Distance(followCamera.target.transform.position, transform.position) > followCamera.CullDistance + 60)
			{
				WorldTileManager.instance.DestroyTile(GetComponent<WorldTile>());
			}
		}

		bool bCanExtend;
		// perform common checks that mean we should not Extend
		protected bool ShouldExtend()
		{
			CurrentPlayerPosition = RoadTileManager.checkpoint.FollowCamera.target.transform.position;

			if (!RoadTileManager.bMainMenu && CurrentPlayerPosition.magnitude > 15 && Vector3.Distance(CachedPlayerPosition, CurrentPlayerPosition) < 15) { /*MySpecificDebug += "15\n";*/ return false; }

			CachedPlayerPosition = CurrentPlayerPosition;

			if (Vector3.Distance(CachedPlayerPosition, transform.position) > RoadTileManager.checkpoint.FollowCamera.CullDistance + (GetComponent<DisabledRoadGenerator>() ? 0 : 40)) return false;
			//if (Vector3.Distance(RoadTileManager.checkpoint.transform.position, transform.position) > (RoadTileManager.checkpoint.gameObject.transform.localScale.x/2) + 20) return false;
			//if (DebugLogs) Debug.Log(gameObject.name + " standing by at " + Vector3.Distance(RoadTileManager.checkpoint.transform.position, transform.position) + " from " + RoadTileManager.checkpoint.transform.position + " (compare to "+( RoadTileManager.checkpoint.gameObject.transform.localScale.x+10)+")" );

			bCanExtend = false;

			// if the Raycasts from last time have any no-hits, we might be able to expand, otherwise we should not need new Raycasts and can bail
			foreach (WorldTile h in hit)
				if (!h)
				{
					bCanExtend = true;
					break;
				}

			if (!bCanExtend) return bCanExtend;

			bCanExtend = DoHits();

			if (!bCanExtend) return bCanExtend; // if none of the new Raycasts didn't not hit anything then we can't expand and can bail

			return bCanExtend;
		}

		public virtual void Extend(bool bForceOOBExtension = false)
		{
			bHaveExpanded = false;

			if (!bForceOOBExtension)
			{
				if (!ShouldExtend()) return;
			}
			else
			{
				if (!DoHits()) return;
			}

			for (int i = 0; i < hitPlus.Length; i += 2)
			{
				hitPlus[i] = WorldTileManager.instance.GetTile(GetTilePosition() + new TilePosition(2 * Xoffset(i), 2 * Zoffset(i)));
				if (hitPlus[i] && hitPlus[i].GetComponent<RoadGenerator>().Exit.Length < 8)
					hitPlus[i].GetComponent<RoadGenerator>().RefreshExits();
			}

			for (int i = 0; i < hit.Length; i += 2)
			{
				if (!hit[i])//.gameObject.GetComponent<RoadGenerator>())
				{
					if (Exit[i])
					{
						if (!hitPlus[i]) // if there is empty space ahead, use this logic
						{
							if (!hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if spaces to left and right
							{
								GameObject newTile = GenerateRandomTile(i, RoadTileManager.checkpoint.RoadMapRoot, true);
								if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " due to freeze pace\n";
							}
							else if (!hit[Wrap0to7(i - 1)] && hit[Wrap0to7(i + 1)]) // if space left and not right
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the right (" + (Direction)Wrap0to7(i + 1) + ") has an exit facing " + (Direction)Wrap0to7(i - 2));
								if (hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)])
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot, true);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left is unoccupied\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateRandomTile(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right is blocked, and the left is unoccupied\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else if (hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if space right and not left
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the left (" + (Direction)Wrap0to7(i - 1) + ") has an exit facing " + (Direction)Wrap0to7(i + 2));
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)])
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot, true);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right is unoccupied\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateRandomTile(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left is blocked, and the right is unoccupied\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else // no unoccupied spaces left or right
							{
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exits to left && right
								{
									GameObject newTile = GenerateQuadOrT(i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tiles to the left and right have valid exits\n";
								}
								else if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && !hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit left only
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right is blocked\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else if (!hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit right only
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left is blocked\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)] || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GeneratePiece(RoadTileManager.Straight, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the left and right are blocked\n";
								}
							}
						}
						else if (hitPlus[i].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) // exit ahead
						{
							if (!hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if spaces to left and right
							{
								GameObject newTile = GenerateStraightOrT(i, RoadTileManager.checkpoint.RoadMapRoot);
								int CwCcw = randCwCcw();
								while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
								{
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

									newTile.transform.Rotate(0, CwCcw * 90, 0);
									newTile.GetComponent<RoadGenerator>().RefreshExits();
								}
								if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " since there is an exit ahead and free either side\n";
							}
							else if (!hit[Wrap0to7(i - 1)] && hit[Wrap0to7(i + 1)]) // if space left and not right
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the right (" + (Direction)Wrap0to7(i + 1) + ") has an exit facing " + (Direction)Wrap0to7(i - 2));
								if (hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)])
								{
									GameObject newTile = GeneratePiece(RoadTileManager.T, i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left is unoccupied, and exit ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateStraightOrT(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right is blocked, and the left is unoccupied, and exit ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else if (hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if space right and not left
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the left (" + (Direction)Wrap0to7(i - 1) + ") has an exit facing " + (Direction)Wrap0to7(i + 2));
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)])
								{
									GameObject newTile = GeneratePiece(RoadTileManager.T, i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right is unoccupied, and exit ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateStraightOrT(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left is blocked, and the right is unoccupied, and exit ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else // no unoccupied spaces left or right
							{
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exits to left && right (and ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.FourWay, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tiles ahead, and to the left and right have valid exits\n";
								}
								else if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && !hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit left only (and ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.T, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right is blocked, and exit ahead\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else if (!hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit right only (and ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.T, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left is blocked, and exit ahead\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)] || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GeneratePiece(RoadTileManager.Straight, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the left and right are blocked (and exit ahead fwiw)\n";
								}
							}
						}
						else // blockage straight ahead
						{
							if (!hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if spaces to left and right
							{
								GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot);
								int CwCcw = randCwCcw();
								while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)]))
								{
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

									newTile.transform.Rotate(0, CwCcw * 90, 0);
									newTile.GetComponent<RoadGenerator>().RefreshExits();
								}
								if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " since there is a blockage ahead but free either side\n";
							}
							else if (!hit[Wrap0to7(i - 1)] && hit[Wrap0to7(i + 1)]) // if space left and not right
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the right (" + (Direction)Wrap0to7(i + 1) + ") has an exit facing " + (Direction)Wrap0to7(i - 2));
								if (hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)])
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left is unoccupied, and blockage ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateCornerOrDead(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right is blocked, and the left is unoccupied, and blockage ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i + 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) + "\n";
									int CwCcw = randCwCcw();
									if (!newTile.name.Contains("Dead"))
										while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
										{
											if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

											newTile.transform.Rotate(0, CwCcw * 90, 0);
											newTile.GetComponent<RoadGenerator>().RefreshExits();
										}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else if (hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)]) // if space right and not left
							{
								if (DebugLogs) Debug.Log(gameObject.name + " checking " + (Direction)i + " that the tile to the left (" + (Direction)Wrap0to7(i - 1) + ") has an exit facing " + (Direction)Wrap0to7(i + 2));
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)])
								{
									GameObject newTile = GenerateCornerOrT(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right is unoccupied, and blockage ahead\n";
									//MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GenerateCornerOrDead(i, RoadTileManager.checkpoint.RoadMapRoot);

									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left is blocked, and the right is unoccupied, and blockage ahead\n";
									//if (RoadTileManager.bDebugEnv) MySpecificDebug += "exit " + (Direction)(Wrap0to7(i + 4)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) + " && exit " + (Direction)(Wrap0to7(i - 2)) + ":" + (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) + "\n";
									int CwCcw = randCwCcw();
									if (!newTile.name.Contains("Dead"))
										while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
										{
											if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

											newTile.transform.Rotate(0, CwCcw * 90, 0);
											newTile.GetComponent<RoadGenerator>().RefreshExits();
										}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
							else // no unoccupied spaces left or right
							{
								if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exits to left && right (and blocked ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.T, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied to the left and right have valid exits and blocked ahead\n";
									int CwCcw = randCwCcw();
									while (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else if (hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && !hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit left only (and blocked ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.Corner, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the left has a valid exit, and the right and ahead are blocked\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else if (!hit[Wrap0to7(i - 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)] && hit[Wrap0to7(i + 1)].gameObject.GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) // exit right only (and blocked ahead)
								{
									GameObject newTile = GeneratePiece(RoadTileManager.Corner, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because the occupied tile to the right has a valid exit, and the left and ahead are blocked\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]) || newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)] || !(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
								else
								{
									GameObject newTile = GeneratePiece(RoadTileManager.DeadEnd, i, RoadTileManager.checkpoint.RoadMapRoot);
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Placing " + newTile.name + " to the " + (Direction)i + " because it's all blocked\n";
									int CwCcw = randCwCcw();
									while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
									{
										if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", rotating\n";

										newTile.transform.Rotate(0, CwCcw * 90, 0);
										newTile.GetComponent<RoadGenerator>().RefreshExits();
									}
									if (RoadTileManager.bDebugEnv) MySpecificDebug += newTile.GetComponent<RoadGenerator>().LogExits() + ", final\n";
								}
							}
						}

						//if (DebugLogs) Debug.Log(gameObject.name + " expanding " + (Direction)i + " to "+new Vector3(Xoffset(i), newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i)));
					}
				}
			}

			if (!bHaveExpanded) // if I haven't placed roads that could expand, place pavements
				for (int i = 0; i < hit.Length; i += 2)
				{
					if (!hit[i])//.gameObject.GetComponent<RoadGenerator>())
					{
						//if (DebugLogs) Debug.Log(gameObject.name + " can expand " + (Direction)i+", space left "+(Direction)Wrap0to7(i-1)+", space right "+(Direction)Wrap0to7(i+1));
						if (RoadTileManager.bDebugEnv) MySpecificDebug += "I can expand " + (Direction)i + ", space left " + (Direction)Wrap0to7(i - 1) + ", space right " + (Direction)Wrap0to7(i + 1) + "\n";

						if (!Exit[i])
						{
							if (!hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i + 1)] && !hitPlus[i])
							{
								// if there's no neighbouring tiles
								GameObject newTileClass = RoadTileManager.RandPavementGrass();
								GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot);
								WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
								if (newTile.GetComponent<DisabledRoadGenerator>().TileClassification == DisabledRoadGenerator.Type.Pavement)
								{
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Generated pavement to the " + (Direction)i + ", rotating to " + (i * 45 - 90) + "\n";
									newTile.transform.localRotation = Quaternion.Euler(0, i * 45 - 90, 0);
								}
							}
							else if ((!hit[Wrap0to7(i - 1)] || hit[Wrap0to7(i - 1)] && !hit[Wrap0to7(i - 1)].GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 2)]) && (!hit[Wrap0to7(i + 1)] || hit[Wrap0to7(i + 1)] && !hit[Wrap0to7(i + 1)].GetComponent<RoadGenerator>().Exit[Wrap0to7(i - 2)]) && (!hitPlus[i] || hitPlus[i] && !hitPlus[i].GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
							{
								// if there's no neighbouring tiles with exits
								DisabledRoadGenerator.Type newTileType = DisabledRoadGenerator.Type.xInvalid;
								if (hit[Wrap0to7(i - 1)] && hit[Wrap0to7(i - 1)].GetComponent<DisabledRoadGenerator>())
								{
									newTileType = hit[Wrap0to7(i - 1)].GetComponent<DisabledRoadGenerator>().TileClassification;
								}
								else if (hit[Wrap0to7(i + 1)] && hit[Wrap0to7(i + 1)].GetComponent<DisabledRoadGenerator>())
								{
									newTileType = hit[Wrap0to7(i + 1)].GetComponent<DisabledRoadGenerator>().TileClassification;
								}
								else if (hitPlus[i] && hitPlus[i].GetComponent<DisabledRoadGenerator>())
								{
									newTileType = hitPlus[i].GetComponent<DisabledRoadGenerator>().TileClassification;
								}
								GameObject newTile;
								if (newTileType == DisabledRoadGenerator.Type.Pavement)
								{
									GameObject newTileClass = RoadTileManager.Pavement;
									newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot);
									WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
								}
								else if (newTileType == DisabledRoadGenerator.Type.Grass || newTileType == DisabledRoadGenerator.Type.Water)
								{
									GameObject newTileClass = RoadTileManager.Grass;
									newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot);
									WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
								}
								else
								{
									GameObject newTileClass = RoadTileManager.RandPavementGrass();
									newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadTileManager.checkpoint.RoadMapRoot);
									WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
								}

								if (newTile && newTile.GetComponent<DisabledRoadGenerator>().TileClassification == DisabledRoadGenerator.Type.Pavement)
								{
									if (RoadTileManager.bDebugEnv) MySpecificDebug += "Generated pavement to the " + (Direction)i + ", rotating to " + (i * 45 - 90) + "\n";
									newTile.transform.localRotation = Quaternion.Euler(0, i * 45 - 90, 0);
								}
							}
						}
					}
				}
		}

		// == this method performs this but in a neat loop ==
		//Physics.Raycast(transform.position + new Vector3(0, 500, 20), new Vector3(0, -1), out hit[0], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(20, 500, 20), new Vector3(0, -1), out hit[1], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(20, 500, 0), new Vector3(0, -1), out hit[2], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(20, 500, -20), new Vector3(0, -1), out hit[3], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(0, 500, -20), new Vector3(0, -1), out hit[4], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(-20, 500, -20), new Vector3(0, -1), out hit[5], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(-20, 500, 0), new Vector3(0, -1), out hit[6], Mathf.Infinity, 1 << 9);
		//Physics.Raycast(transform.position + new Vector3(-20, 500, 20), new Vector3(0, -1), out hit[7], Mathf.Infinity, 1 << 9);
		public bool DoHits()
		{
			bCanExtend = false;

			for (int i = 0; i < hit.Length; i++)
			{
				tp.x = Xoffset(i);
				tp.z = Zoffset(i);
				hit[i] = WorldTileManager.instance.GetTile(GetTilePosition() + tp);
				if (hit[i] && hit[i].GetComponent<RoadGenerator>().Exit.Length < 8)
					hit[i].GetComponent<RoadGenerator>().RefreshExits();
				if (!hit[i]) bCanExtend = true;
			}
			return bCanExtend;
		}

		public WorldTile[] GetNeighbours() { return hit; }

		// for looping through Direction
		public static int Wrap0to7(int i)
		{
			while (i < 0) i += 8;
			while (i > 7) i -= 8;
			return i;
		}

		// where i is the Direction
		public static int Xoffset(int i) { return ((i % 4 > 0) ? 1 : 0) * ((i >= 4) ? -1 : 1); }

		// where i is the Direction
		public static int Zoffset(int i) { return ((i % 4 != 2) ? 1 : 0) * ((i > 2 && i < 6) ? -1 : 1); }

		public static int randCwCcw() { return Random.Range(0, 2) * 2 - 1; }
		public int biasCwCCw()
		{
			int r = Random.Range(0, 3);
			if (transform.position.x < RoadTileManager.checkpoint.transform.position.x)
			{
				if (r < 2) return 1;
				else return -1;
			}
			else
			{
				if (r < 2) return -1;
				else return 1;
			}
		}

		public void RefreshExits()
		{
			Exit = new bool[8];
			Exit[0] = PosZ;
			Exit[2] = PosX;
			Exit[4] = NegZ;
			Exit[6] = NegX;
			Exit[1] = Exit[3] = Exit[5] = Exit[7] = false;

			// correct road exit directions to reflect a rotated tile
			if (Exit[2] || Exit[6] || Exit[0] || Exit[4])
			{
				bool oldNegZ, oldNegX;

				switch (transform.eulerAngles.y.ToString())
				{
					case "90":
						oldNegZ = Exit[4];
						Exit[4] = Exit[2];
						Exit[2] = Exit[0];
						Exit[0] = Exit[6];
						Exit[6] = oldNegZ;
						break;
					case "180":
						oldNegZ = Exit[4];
						Exit[4] = Exit[0];
						Exit[0] = oldNegZ;
						oldNegX = Exit[6];
						Exit[6] = Exit[2];
						Exit[2] = oldNegX;
						break;
					case "270":
						oldNegZ = Exit[4];
						Exit[4] = Exit[6];
						Exit[6] = Exit[0];
						Exit[0] = Exit[2];
						Exit[2] = oldNegZ;
						break;
					case "0.0":
					default:
						break;
				}
				//if (DebugLogs) Debug.Log(LogExits());
			}
			//if (RoadTileManager.bDebugEnv) MySpecificDebug += Time.unscaledTime + " Refreshed exits\n";
		}

		// where i is the Direction
		protected GameObject GenerateRandomTile(int i, Transform RoadMapRootTransform, bool bAllowQuad = false)
		{
			GameObject newTileClass = (Exit[Wrap0to7(i + 4)] && !(Exit[Wrap0to7(i + 2)] || Exit[Wrap0to7(i - 2)])) ? RoadTileManager.RandomRoadTile(bAllowQuad) : RoadTileManager.Straight;
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
			{
				newTile.transform.Rotate(0, CwCcw * 90, 0);
				newTile.GetComponent<RoadGenerator>().RefreshExits();
			}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		// where i is the Direction
		protected GameObject GenerateCornerOrT(int i, Transform RoadMapRootTransform, bool bAllowQuad = false)
		{
			GameObject newTileClass = RoadTileManager.RandCornerT(bAllowQuad);
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
			{
				newTile.transform.Rotate(0, CwCcw * 90, 0);
				newTile.GetComponent<RoadGenerator>().RefreshExits();
			}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		// where i is the Direction
		protected GameObject GenerateStraightOrT(int i, Transform RoadMapRootTransform)
		{
			GameObject newTileClass = (Exit[Wrap0to7(i + 4)] && !(Exit[Wrap0to7(i + 2)] || Exit[Wrap0to7(i - 2)])) ? RoadTileManager.RandStraightT() : RoadTileManager.Straight;
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
			{
				newTile.transform.Rotate(0, CwCcw * 90, 0);
				newTile.GetComponent<RoadGenerator>().RefreshExits();
			}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		// where i is the Direction
		protected GameObject GenerateQuadOrT(int i, Transform RoadMapRootTransform)
		{
			GameObject newTileClass = RoadTileManager.RandQuadT();
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			if (newTileClass == RoadTileManager.T) // quad will always be the right way around, otherwise T should be oriented to be a dead end in the i direction
				while (newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i)])
				{
					newTile.transform.Rotate(0, CwCcw * 90, 0);
					newTile.GetComponent<RoadGenerator>().RefreshExits();
				}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		protected GameObject GenerateCornerOrDead(int i, Transform RoadMapRootTransform)
		{
			GameObject newTileClass = RoadTileManager.RandCornerDead();
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
			{
				newTile.transform.Rotate(0, CwCcw * 90, 0);
				newTile.GetComponent<RoadGenerator>().RefreshExits();
			}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		// where i is the Direction
		protected GameObject GeneratePiece(GameObject newTileClass, int i, Transform RoadMapRootTransform)
		{
			GameObject newTile = Instantiate(newTileClass, transform.position + new Vector3(Xoffset(i) * WorldTileManager.TILE_SIZE, newTileClass.GetComponent<RoadGenerator>().YOffset, Zoffset(i) * WorldTileManager.TILE_SIZE), Quaternion.identity, RoadMapRootTransform);
			newTile.transform.SetAsFirstSibling();
			bHaveExpanded = true;
			//newTile.GetComponent<RoadGenerator>().Extend(RoadTileManager.checkpoint);
			newTile.GetComponent<RoadGenerator>().RefreshExits();
			int CwCcw = randCwCcw();
			if (newTileClass != RoadTileManager.FourWay) // quad will always be the right way around
				while (!(newTile.GetComponent<RoadGenerator>().Exit[Wrap0to7(i + 4)]))
				{
					newTile.transform.Rotate(0, CwCcw * 90, 0);
					newTile.GetComponent<RoadGenerator>().RefreshExits();
				}
			WorldTileManager.instance.AddTile(newTile.GetComponent<WorldTile>());
			return newTile;
		}

		public string LogExits() { return gameObject.name + " has exits:" + (Exit[0] ? " north," : "") + (Exit[2] ? " east," : "") + (Exit[4] ? " south," : "") + (Exit[6] ? " west," : ""); }
	}
}
