using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask target;

    private void FixedUpdate()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((target & (1 << other.gameObject.layer)) != 0)
        {
            IDamageable d = other.GetComponent<IDamageable>();
            if (d != null)
            {
                d.TakeDamage(damage);
            }
        }
    }
}
