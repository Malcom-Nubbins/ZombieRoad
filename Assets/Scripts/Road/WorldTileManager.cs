using System.Collections.Generic;
using UnityEngine;

namespace ZR.Road
{
	public struct TilePosition
	{
		public int x, z;
		public TilePosition(int x, int z) { this.x = x; this.z = z; }
		public TilePosition(Vector3 v) : this(Mathf.FloorToInt(v.x / WorldTileManager.TILE_SIZE), Mathf.FloorToInt(v.z / WorldTileManager.TILE_SIZE)) { }
		public static TilePosition operator +(TilePosition a, TilePosition b)
		{
			return new TilePosition(a.x + b.x, a.z + b.z);
		}
		public Vector3 GetWorldPosition()
		{
			return new Vector3(x, 0, z) * WorldTileManager.TILE_SIZE;
		}
	}

	public class WorldTileManager : MonoBehaviour
	{
		public static readonly float TILE_SIZE = 20;
		public static WorldTileManager instance;
		Dictionary<TilePosition, WorldTile> tiles = new Dictionary<TilePosition, WorldTile>();

		void Awake()
		{
			if (instance) Debug.LogError("More than one instance of WorldTileManager");
			instance = this;
		}

		void Start()
		{
			//add tiles already in scene

			foreach (WorldTile tile in FindObjectsOfType<WorldTile>())
			{
				AddTile(tile);
			}
		}

		public WorldTile GetTile(TilePosition pos)
		{
			WorldTile tile = null;
			tiles.TryGetValue(pos, out tile);
			return tile;
		}

		public WorldTile[] GetAllTiles()
		{
			//copy so tiles can be created and destroyed while iterating
			WorldTile[] tilesCopy = new WorldTile[tiles.Values.Count];
			tiles.Values.CopyTo(tilesCopy, 0);
			return tilesCopy;
		}

		public void AddTile(WorldTile tile)
		{
			TilePosition pos = tile.GetTilePosition();
			if (tiles.ContainsKey(pos))
			{
				Debug.LogError("AddTile: Tile already exists at (" + pos.x + "," + pos.z + ")");
				Destroy(tiles[pos].gameObject);
				tiles.Remove(pos);
			}
			tiles[pos] = tile;
			//Debug.Log("AddTile " + tile.gameObject.name);
		}

		public void RemoveTile(TilePosition pos)
		{
			if (!tiles.Remove(pos))
			{
				Debug.LogError("RemoveTile: Tile does not exist at (" + pos.x + "," + pos.z + ")");
			}
		}

		public void DestroyTile(WorldTile tile)
		{
			RemoveTile(tile.GetTilePosition());
			Destroy(tile.gameObject);
		}
	}
}
