using UnityEngine;

public class Health : MonoBehaviour
{
	private static FollowCamera _followCamera;
	public float startingHealth = 1.0f;
	private float _health;
	public float health
	{
		get
		{
			return _health;
		}
		set
		{
			bool wasAlive = _health > 0;
			_health = value;
			if (health <= 0 && wasAlive)
			{
				onDeath?.Invoke();
			}
		}
	}
	public delegate void OnDeath();
	public event OnDeath onDeath;

	void Awake()
	{
		health = startingHealth;

		if (_followCamera == null && RoadTileManager.checkpoint != null)
		{
			_followCamera = RoadTileManager.checkpoint.FollowCamera;
		}
	}

	void Update()
	{
		if (_followCamera != null)
		{
			if (transform.position.y < -10 || Vector3.Distance(transform.position, _followCamera.target.transform.position) > _followCamera.CullDistance)
			{
				health = 0;
				Destroy(gameObject);
			}
		}
	}

	public void TakeDamage(float damageAmount)
	{
		health -= damageAmount;
	}
}
