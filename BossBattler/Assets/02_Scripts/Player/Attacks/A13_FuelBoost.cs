using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A13_FuelBoost : SimpleAttack
{
    protected override void CreateAttack()
    {
        Instantiate(prefab).GetComponent<VFX>().Setup(transform.position);
    }

    public override bool OnHit(Collider2D other)
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

