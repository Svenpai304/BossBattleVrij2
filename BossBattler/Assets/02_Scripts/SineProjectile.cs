using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineProjectile : MonoBehaviour, IProjectile
{

    private IProjectileOwner owner;
    [SerializeField] private int maxHits;
    private float Xstart;
    private float Ystart;
    private float Xdirection;
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [SerializeField] private float wavelength;
    [SerializeField] private float lifetime = 20;


    private void FixedUpdate()
    {

        float Ypos = Ystart + Mathf.Sin((transform.position.x - Xstart) / wavelength) * amplitude;
        transform.position = new Vector3(transform.position.x + speed * Xdirection * Time.deltaTime , Ypos, transform.position.z);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollision(other);
    }

    public void OnCollision(Collider2D other)
    {
        if (owner.OnProjectileHit(other, gameObject))
        {
            maxHits--;
        }
        if (maxHits <= 0)
        {
            Die();
        }
    }

    public void Setup(int power, int _Xdirection, IProjectileOwner _owner)
    {
        Xdirection = _Xdirection;
        owner = _owner;
        Xstart = transform.position.x; 
        Ystart = transform.position.y;

        transform.localScale = new Vector3(0.5f + power, 0.5f + power, 1);
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
            yield return null;
        }
        Destroy(gameObject);
    }
    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
