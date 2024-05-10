using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class HydroSphere : MonoBehaviour
{
    private IProjectileOwner owner;
    [SerializeField] private float sizeCurrent;
    [SerializeField] private float sizeReduction;
    private Vector2 direction;
    public float lifetime = 4;
    public GameObject explosionObject; //Add Later
    private float defensivePower;

    public SpriteRenderer spr;


    private void FixedUpdate()
    {
        sizeCurrent -= sizeReduction * Time.deltaTime;
        transform.localScale = new Vector3(sizeCurrent, sizeCurrent, 1)*(0.5f+defensivePower);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Die(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision.collider);
    }
    public void OnCollision(Collider2D other)
    {
        if (isFading) return;
        Die(true);
        // This shield absorbs large projectiles and explodes if
    }
    public virtual void Setup(float _defensePower, Transform parent, Vector2 _position, IProjectileOwner _owner)
    {
        owner = _owner;
        defensivePower = _defensePower;

        transform.position = _position;
        Quaternion rotref = transform.rotation;
        transform.SetParent(parent);
        StartCoroutine(SmoothFadeIn());
    }
    public virtual void Die(bool doExplosion)
    {
        if (isFading) return;
        //Instantiate(explosionObject, transform.position, transform.rotation);
        StartCoroutine(SmoothFade());
    }

    bool isFading = false;
    IEnumerator SmoothFadeIn()
    {
        float Fade = spr.color.a;
        while (Fade < 0.6f)
        {
            Fade += Time.deltaTime;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Fade);
            yield return null;
        }
    }
    IEnumerator SmoothFade()
    {
        float Fade = spr.color.a;
        while (Fade > 0f)
        {
            Fade -= Time.deltaTime;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Fade);
            yield return null;
        }
        Destroy(gameObject);
    }
}
