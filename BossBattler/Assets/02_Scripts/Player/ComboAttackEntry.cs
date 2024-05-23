using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComboAttackEntry : ScriptableObject
{
    public int[] id = new int[2];
    public float powerCost;
    public float castingTime;
    public Color castingParticleColor;
    public GameObject effectObject;

    public void Fire(CharacterStatus status)
    {
        if (status.isCasting) return;
        Debug.Log("Using combo attack: " + id[0].ToString() + id[1].ToString());

        //if(status.Power < powerCost) { return; }
        status.usePower(powerCost);
        status.StartCoroutine(StrikeDelay(status));
        
    }
    IEnumerator StrikeDelay(CharacterStatus status)
    {
        status.isCasting = true;
        status.BuffMoveSpeed("Casting", castingTime, 1, 0.3f);
        var main = status.castingParticles.main;
        main.startColor = castingParticleColor;
        status.castingParticles.Play();
        yield return new WaitForSeconds(castingTime);
        status.isCasting = false;
        status.castingParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        ComboAttack atk = Instantiate(effectObject).GetComponent<ComboAttack>();
        if (atk != null)
        {
            atk.OnFire(status);
        }
    }
}
