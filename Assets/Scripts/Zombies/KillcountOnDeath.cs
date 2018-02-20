using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillcountOnDeath : MonoBehaviour
{
    Health health;
    
	void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += Health_onDeath;
	}

    private void Health_onDeath()
    {
        if (transform.position.y < -5)
        {
            //fell out of world
        }
        else
        {
            FollowCamera camera = RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>();
            if(Vector3.Distance(transform.position, camera.target.transform.position) > camera.CullDistance)
            {
                // Culled
            }
            else if(RoadTileManager.bMainMenu)
            {
                // Main Menu screen
            }
            else
            {
                Killcount.AddKill();
                if (Killcount.GetKills() % 10 == 0)
                    Currency.AddCurrency();
            }

        }
    }

    void Update()
    {
		
	}
}
