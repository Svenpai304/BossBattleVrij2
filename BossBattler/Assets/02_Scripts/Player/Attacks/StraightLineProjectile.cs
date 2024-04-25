using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class StraightLineProjectile : MonoBehaviour
{
    private IStatus status;
    private float damage;
    private float speed;
    private Vector2 direction;
    public float lifetime = 20;

    public Transform rotateTransform;


    private void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && damageable != status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
        }
        Die();
    }

    public virtual void Setup(float _damage, float _speed, Vector2 _direction, Vector2 _position, IStatus _status)
    {
        damage = _damage;
        speed = _speed;
        direction = _direction;
        status = _status;

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
