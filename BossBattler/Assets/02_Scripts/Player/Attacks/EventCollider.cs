using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCollider : MonoBehaviour
{
    public UnityEvent<Collider2D> OnCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision?.Invoke(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollision?.Invoke(other);
    }


}
