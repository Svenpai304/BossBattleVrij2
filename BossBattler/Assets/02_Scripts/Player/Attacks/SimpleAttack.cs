using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : ComboAttack, IProjectileOwner
{
    //The Simple Attack simply creates an object
    protected CharacterStatus status;
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

    protected IEnumerator Process()
    {
        CreateAttack();
        Destroy(this);
        yield break;
    }

    protected virtual void CreateAttack()
    {
        Instantiate(prefab).GetComponent<HydroSphere>().Setup(power, transform, transform.position, this);
    }

    public virtual bool OnHit(Collider2D other)
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

