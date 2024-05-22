using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinion : MonoBehaviour, IDamageable, IProjectileOwner
{
    public SpriteRenderer spr;
    public float MaxHealth;
    public float MaxSpeed;
    public float AccelMod;
    public float Damage;
    protected float CurSpeed;
    protected float CurHealth;
    protected bool isAlive = true;
    protected CharacterStatus PlayerTarget;
    protected Vector3 MoveTarget;
    protected float CurDistance = 1f;
    protected int MoveDir = 1;
    protected bool isMoving = false;

    public void setMoveTarget(Vector3 pos)
    {
        MoveTarget = pos;
        isMoving = true;
        MoveDir = 1;
        CurDistance = (pos - transform.position).magnitude;
    }

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {

    }

    private void FixedUpdate()
    {
        AITick();
    }

    protected virtual void AITick()
    {

    }
    public void HealDamage(float damage)
    {
        CurHealth += damage;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        CurHealth -= damage;
        if (CurHealth < 0) Die();
    }

    protected void Die()
    {
        isAlive = false;
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        float Fade = 1f;
        while (Fade > 0f)
        {
            Fade -= Time.deltaTime;
            spr.color = new Color(1, 1, 1, Fade);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SeekNearestTarget()
    {
        float DistoMax = 999f;
        CharacterStatus trt = null;
        foreach (CharacterStatus pl in PlayerConnector.instance.players)
        {
            float Dist = pl.Dist(transform);
            if (Dist < DistoMax)
            {
                DistoMax = Dist;
                trt = pl;
            }
        }
        PlayerTarget = trt;
    }

    bool IProjectileOwner.OnProjectileHit(Collider2D other, GameObject projectile)
    {
        CharacterStatus stat = other.GetComponent<CharacterStatus>();
        if (stat != null)
        {
            stat.TakeDamage(Damage);
            return true;
        }
        return false;
    }
    public Vector3 getLookVector()
    {
        return getLookVector(transform.rotation);
    }
    protected Vector3 getLookVector(Quaternion rotref)
    {
        float rot = Mathf.Deg2Rad * (rotref.eulerAngles.z+90);
        float dxf = Mathf.Cos(rot);
        float dyf = Mathf.Sin(rot);
        return new Vector3(dxf, dyf, 0);
    }
}
