using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class StraightLineProjectile : MonoBehaviour, IProjectile
{
    private IProjectileOwner owner;
    [SerializeField] private int maxHits;
    private float speed;
    private float accel;
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
        if(owner.OnProjectileHit(other, gameObject))
        {
            maxHits--;
        }
        if (maxHits <= 0)
        {
            Die();
        }
    }

    public virtual void Setup(float PowerLevel, int maxHits, float _speed, Vector2 _direction, Vector2 _position, IProjectileOwner _owner)
    {
        speed = _speed;
        direction = _direction;
        owner = _owner;

        transform.localScale = new Vector3(0.5f+PowerLevel, 0.5f+PowerLevel, 1);

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

    public void DestroyProjectile()
    {
        Die();
    }
}
public interface IProjectileOwner
{
    public bool OnProjectileHit(Collider2D other, GameObject projectile);
}
