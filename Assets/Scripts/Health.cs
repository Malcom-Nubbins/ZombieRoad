using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
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
                onDeath();
            }
        }
    }
    public delegate void OnDeath();
    public event OnDeath onDeath;

    void Start()
    {
        health = startingHealth;
    }

    void Update()
    {
        if (transform.position.y < -10)
        {
            health = 0;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
    }
}
