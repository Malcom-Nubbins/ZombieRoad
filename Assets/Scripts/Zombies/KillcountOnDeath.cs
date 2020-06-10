using UnityEngine;

public class KillcountOnDeath : MonoBehaviour
{
	[SerializeField, NonNull] Health health;
	
	void Start()
	{
		health.onDeath += OnDeath;
	}

	void OnDeath()
	{
		if (transform.position.y < -5)
		{
			//fell out of world
		}
		else
		{
			FollowCamera camera = RoadTileManager.checkpoint.FollowCamera;
			if (Vector3.Distance(transform.position, camera.target.transform.position) > camera.CullDistance)
			{
				// Culled
			}
			else if (RoadTileManager.bMainMenu)
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
}
