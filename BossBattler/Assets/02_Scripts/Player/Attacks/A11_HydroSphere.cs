using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A11_HydroSphere : SimpleAttack
{
    [SerializeField] private float projectileDestroyRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float lifetime;

    float damageMod;

    protected override void CreateAttack()
    {
        damageMod = status.getPowerDamageMod();
        Instantiate(prefab).GetComponent<HydroSphere>().Setup(status.getPowerDamageMod(), transform, transform.position, this);
    }
    protected override IEnumerator Process()
    {
        CreateAttack();
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    public override bool OnProjectileHit(Collider2D other, GameObject p)
    {

        Collider2D[] projectiles = Physics2D.OverlapCircleAll(transform.position, projectileDestroyRadius * damageMod);
        foreach (Collider2D col in projectiles)
        {
            var i = col.GetComponentInParent<IProjectile>();
            i?.DestroyProjectile();
            if (col.TryGetComponent<IDamageable>(out var d) && d != (IDamageable)status) { d.TakeDamage(explosionDamage * damageMod); }
        }
        return true;
    }

}

