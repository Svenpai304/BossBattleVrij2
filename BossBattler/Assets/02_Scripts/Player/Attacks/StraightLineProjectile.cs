using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class StraightLineProjectile : MonoBehaviour
{
    private IProjectileOwner owner;
    private int maxHits;
    private float damage;
    private float speed;
    [SerializeField] private float accel;
    private Vector2 direction;
    public float lifetime = 20;

    public Transform rotateTransform;


    private void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        speed += accel * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Die();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision.collider);
    }

    public void OnCollision(Collider2D other)
    {
        if(owner.OnHit(other))
        {
            maxHits--;
        }
        if (maxHits <= 0)
        {
            Die();
        }
    }

    public virtual void Setup(int maxHits, float _speed, Vector2 _direction, Vector2 _position, IProjectileOwner _owner)
    {
        speed = _speed;
        direction = _direction;
        owner = _owner;

        transform.position = _position;
        if (rotateTransform != null)
        {
            rotateTransform.Rotate(0, 0, Vector2.SignedAngle(Vector2.up, _direction));
        }
        else
        {
            transform.Rotate(0, 0, Vector2.SignedAngle(Vector2.up, _direction));
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
public interface IProjectileOwner
{
    public bool OnHit(Collider2D other);
}
