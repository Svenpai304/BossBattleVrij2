using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A11_HydroSphere : SimpleAttack
{
    protected override void CreateAttack()
    {
        Instantiate(prefab).GetComponent<HydroSphere>().Setup(status.getPowerDamageMod(), transform, transform.position, this);
    }
    public override bool OnProjectileHit(Collider2D other, GameObject p)
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

