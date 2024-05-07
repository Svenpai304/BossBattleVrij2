using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private float sizeCurrent = 1f;
    [SerializeField] private float sizeReduction;
    [SerializeField] private float sizeReductionAccel;
    [SerializeField] private float FadeSpeed = 1f;
    [SerializeField] private float FadeMax = 1f;
    public float lifetime = 4;

    public SpriteRenderer spr;


    private void FixedUpdate()
    {
        sizeCurrent -= sizeReduction * Time.deltaTime;
        if (sizeReduction < 0f) sizeReduction += sizeReductionAccel * Time.deltaTime;
        transform.localScale = new Vector3(sizeCurrent, sizeCurrent, 1);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Die(false);
        }
    }
    public virtual void Setup(Vector2 _position)
    {
        transform.position = _position;
        Quaternion rotref = transform.rotation;
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
        while (Fade < FadeMax)
        {
            Fade += FadeSpeed*Time.deltaTime;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Fade);
            yield return null;
        }
    }
    IEnumerator SmoothFade()
    {
        float Fade = spr.color.a;
        while (Fade > 0f)
        {
            Fade -= FadeSpeed*Time.deltaTime;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Fade);
            yield return null;
        }
        Destroy(gameObject);
    }
}
