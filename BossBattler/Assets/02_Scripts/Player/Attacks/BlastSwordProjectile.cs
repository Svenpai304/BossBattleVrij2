using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class BlastSwordProjectile : MonoBehaviour
{
    private IProjectileOwner owner;
    private int maxHits;
    private float damage;
    private float speed;
    private Vector2 direction;
    public float lifetime = 20;
    [SerializeField] private float RotationFast = 360f;

    public Transform rotateTransform;
    public SpriteRenderer spr;


    private void FixedUpdate()
    {
        //Rotate very fast animation!
        RotationFast -= Time.deltaTime*RotationFast*0.5f;
        rotateTransform.Rotate(Vector3.forward, RotationFast * Time.deltaTime);
        speed -= Time.deltaTime * speed*2f;

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
        if(owner.OnProjectileHit(other, gameObject))
        {
            maxHits--;
        }
        if (maxHits <= 0)
        {
            Die();
        }
    }
    public virtual void Setup(float Power, int maxHits, float _speed, Vector2 _direction, float _angle, Vector2 _position, IProjectileOwner _owner)
    {
        speed = _speed;
        direction = _direction;
        owner = _owner;

        transform.localScale = new Vector3(0.5f+Power, 0.5f+Power, 1);

        transform.position = _position;
        Quaternion rotref = transform.rotation;

        transform.Rotate(0, 0, Vector2.SignedAngle(Vector2.up, _direction) + _angle);
        float rot = Mathf.Deg2Rad * (rotref.eulerAngles.z+90);
        float dxf = Mathf.Cos(rot);
        float dyf = Mathf.Sin(rot);
        direction = new Vector3(dxf, dyf, 0);
    }
    public virtual void Die()
    {
        if (isFading) return;
        StartCoroutine(SmoothFade());
    }

    bool isFading = false;
    IEnumerator SmoothFade()
    {
        float Fade = 1f;
        while (Fade > 0f)
        {
            Fade -= Time.deltaTime;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Fade);
            yield return null;
        }
        Destroy(gameObject);
    }
}
