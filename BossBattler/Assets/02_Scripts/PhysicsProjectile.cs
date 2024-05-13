using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProjectile : MonoBehaviour
{
    private IProjectileOwner owner;
    public Rigidbody2D rb;

    [SerializeField] private float lifetime;

    public void Setup(IProjectileOwner _owner)
    {
        owner = _owner;
    }

    private void FixedUpdate()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (owner.OnProjectileHit(collision.collider, gameObject))
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
