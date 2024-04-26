using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A22_Swords : ComboAttack, IProjectileOwner
{
    private CharacterStatus status;
    public GameObject swordPrefab;
    public int swordCount;
    public float interval;
    public float spread;
    public float damage;
    public float speed;

    public override void OnFire(CharacterStatus _status)
    {
        transform.position = _status.transform.position;
        transform.parent = _status.transform;
        status = _status;
        StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        while (swordCount > 0)
        {
            FireSword();
            swordCount--;
            yield return new WaitForSeconds(interval);
        }
        Destroy(this);
    }

    private void FireSword()
    {
        Vector2 direction = (status.LookDirection + Random.insideUnitCircle * spread).normalized;
        Instantiate(swordPrefab).GetComponent<StraightLineProjectile>().Setup(1, speed, direction, transform.position, this);
    }

    public bool OnHit(Collider2D other)
    {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && (object)damageable != status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
        }
        return true;
    }
}

