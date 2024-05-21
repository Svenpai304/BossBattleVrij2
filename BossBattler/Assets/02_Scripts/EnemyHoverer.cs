using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHoverer : EnemyMinion
{
    //This enemy hovers around an area and does pew pew pow pow
    public StraightLineProjectile SpawnProj;
    public float minAttackDelay;
    public float maxAttackDelay;
    private float attackCooldown;
    protected override void Init()
    {
    }
    protected override void AITick()
    {
        if (PlayerTarget == null)
        {
            setNewMoveTarget();
            return;
        }
        if (CurDistance <= (CurSpeed / (AccelMod*MaxSpeed))*CurSpeed*0.5f) MoveDir = -1;
        CurSpeed = Mathf.Clamp(CurSpeed + MaxSpeed * AccelMod * MoveDir * Time.deltaTime, 0, MaxSpeed);
        //4 Distance means stop if Current speed is 8 and deaccel is 8 (Distance = 4)
        //Speed is 8 and deaccel is 4 means (6+2 = 8) (Speed/Deaccel)*Speed*0.5
        //Current speed is 8. Deacceleration is 8*1 = 8. Speed until depletion is 4. Thus, lower speed. (8 / (1*8))*0.5f
        //Speed is 8 and deaccel is 16 means average speed of 2.
        //Deaccel is 0.5*8 max speed = 4 deaccel.
        //Speed is 4 with 4 deaccel means 2 distance left (1*4*0.5=2)
        //Speed/Deaccel == 1
        CurDistance -= CurSpeed * Time.deltaTime;
        transform.position += (MoveTarget - transform.position).normalized * CurSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, Vector2.SignedAngle(getLookVector(), PlayerTarget.transform.position-transform.position));

        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0)
        {
            attackCooldown = Random.Range(minAttackDelay, maxAttackDelay);
            Vector3 dir = (PlayerTarget.transform.position - transform.position).normalized;
            Instantiate(SpawnProj).Setup(1f, 1, 7f, dir, transform.position + dir * 0.4f, this);
        }
        if ((CurSpeed == 0 && MoveDir == -1)) //transform.position-MoveTarget).magnitude < CurSpeed+0.05f
        {
            setNewMoveTarget();
        }
    }

    private void setNewMoveTarget()
    {
        SeekNearestTarget();
        if (PlayerTarget == null) return;
        float mov = Random.Range(0f, 1f) < 0.5f ? -Random.Range(10f, 19f) : Random.Range(10f, 19f);
        setMoveTarget(PlayerTarget.transform.position + new Vector3(mov,Random.Range(8f, 15f)));
    }
}
