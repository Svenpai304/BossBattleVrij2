using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A22_Swords : ComboAttack, IProjectileOwner
{
    protected CharacterStatus status;
    public GameObject swordPrefab;
    public int swordCount;
    public float interval;
    public float spread;
    public float damage;
    public float speed;
    private float PowerLevel;

    public override void OnFire(CharacterStatus _status)
    {
        PowerLevel = _status.getPowerDamageMod();
        damage *= _status.getPowerDamageMod();
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
        Instantiate(swordPrefab).GetComponent<StraightLineProjectile>().Setup(PowerLevel, 1, speed, direction, transform.position, this);
    }

    public virtual bool OnProjectileHit(Collider2D other, GameObject p)
    {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && damageable != (IDamageable)status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
            Quaternion particleRotation = Quaternion.Euler(0, 0, p.transform.GetChild(0).rotation.eulerAngles.z - 90);
            ParticleManager.SpawnParticles(1, p.transform.position, p.transform.localScale, particleRotation);
            return true;
        }
        return false;
    }
}

