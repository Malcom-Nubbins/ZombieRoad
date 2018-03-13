using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    void Awake()
    {

    }

	void Start()
    {
		
	}
	
	void Update()
    {
		
	}

    public TilePosition GetTilePosition()
    {
        return new TilePosition(transform.position);
    }

    public void SetTilePosition(TilePosition pos)
    {
        transform.position = pos.GetWorldPosition();
    }
}
