using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A12_Swords : ComboAttack, IProjectileOwner
{
    private CharacterStatus status;
    public GameObject swordPrefab;
    public int swordCount;
    public float spreadAngle;
    public float damage;
    public float speed;

    private float power;

    public override void OnFire(CharacterStatus _status)
    {
        power = _status.getPowerDamageMod();
        damage *= _status.getPowerDamageMod();
        transform.position = _status.transform.position;
        transform.parent = _status.transform;
        status = _status;
        StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        for (float a = -spreadAngle + ((spreadAngle * 1f) / swordCount); a <= spreadAngle; a+= (spreadAngle*2f)/swordCount)
        { //Fire even shotgun spread
            FireSword(a);
        }
        Destroy(this);
        yield break;
    }

    private void FireSword(float ang)
    {
        Vector2 direction = (status.LookDirection).normalized;
        Instantiate(swordPrefab).GetComponent<BlastSwordProjectile>().Setup(power,1, speed, direction, ang, transform.position, this);
    }

    public bool OnProjectileHit(Collider2D other, GameObject p)
    {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && (object)damageable != status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
        }
        return true;
    }
}

