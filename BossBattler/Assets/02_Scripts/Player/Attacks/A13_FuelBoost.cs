using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A13_FuelBoost : SimpleAttack
{
    public float BuffRange = 7f;
    protected override void CreateAttack()
    {
        Instantiate(prefab).GetComponent<VFX>().Setup(status.getPowerDamageMod(),transform.position);
        //Hier moet de buff worden uitgedeeld.
        foreach (CharacterStatus stat in PlayerConnector.instance.players)
        {
            if (status.Dist(stat.transform) < BuffRange)
            {
                stat.BuffDamageDone("FuelBoostDamageDone", 9f, 1, 1f + (0.3f*power));
                stat.BuffDamageDone("FuelBoostDamageTaken", 9f, 1, 1f - (0.3f * power));
            }
        }
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

