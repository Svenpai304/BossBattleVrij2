using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A11_HydroSphere : ComboAttack, IProjectileOwner
{
    private CharacterStatus status;
    public GameObject prefab;
    public float power = 40f;
    public override void OnFire(CharacterStatus _status)
    {
        power *= _status.getPowerDamageMod();
        transform.position = _status.transform.position;
        transform.parent = _status.transform;
        status = _status;
        StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        CreateShield();
        Destroy(this);
        yield break;
    }

    private void CreateShield()
    {
        Instantiate(prefab).GetComponent<HydroSphere>().Setup(power, transform, transform.position, this);
    }

    public bool OnHit(Collider2D other)
    {
        /*
         * (Ryan) [07/05 16:36] Weet niet precies wat ik hier mee moet
         * 
         * var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && (object)damageable != status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
        }*/
        return true;
    }
}

