using UnityEngine;

namespace ZR.Road
{
	public class WorldTile : MonoBehaviour
	{
		public TilePosition GetTilePosition()
		{
			return new TilePosition(transform.position);
		}

		public void SetTilePosition(TilePosition pos)
		{
			transform.position = pos.GetWorldPosition();
		}
	}
}
