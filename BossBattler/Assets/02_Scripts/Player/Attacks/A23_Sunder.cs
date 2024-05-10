using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A23_Sunder : A22_Swords
{
    public override bool OnProjectileHit(Collider2D other)
    {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && (object)damageable != status)
        {
            damageable.TakeDamage(damage * status.DamageDealMult);
        }
        return true;
    }
}

