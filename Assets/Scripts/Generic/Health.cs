using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float startingHealth = 1.0f;
    private float _health;
	public static FollowCamera _followCamera;
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
            if (health <= 0 && wasAlive && onDeath != null)
            {
                onDeath();
            }
        }
    }
    public delegate void OnDeath();
    public event OnDeath onDeath;

    void Start()
    {
        health = startingHealth;

		if (_followCamera == null && RoadTileManager.checkpoint != null) _followCamera = RoadTileManager.checkpoint.FollowCamera.GetComponent<FollowCamera>();
	}

    void Update()
    {
        if(_followCamera != null)
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
