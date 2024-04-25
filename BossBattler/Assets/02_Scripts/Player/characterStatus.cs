
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStatus : MonoBehaviour, IDamageable
{
    public int MaxHealth;
    public float MaxPower;
    public float MaxDashTime;
    public float MinDashTime;
    public float PowerRegen;

    [HideInInspector] public float DamageDealMult;
    [HideInInspector] public float DamageTakenMult;
    [HideInInspector] public float GroundSpeedMult;
    [HideInInspector] public float PowerRegenMult;
    [HideInInspector] public bool Invulnerable;

    public float Health { get { return health; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get { return dashTime; } set { dashTime = value; } }

    private float health;
    private float power;
    private float dashTime;

    private void Start()
    {
        ResetStatus();
    }

    private void Update()
    {
        power = Mathf.Clamp(power + PowerRegen * PowerRegenMult, 0, MaxPower);
    }



    public void ResetStatus()
    {
        health = MaxHealth;
        power = MaxPower;
        dashTime = MaxDashTime;

        DamageDealMult = 1;
        DamageTakenMult = 1;
        GroundSpeedMult = 1;
        PowerRegenMult = 1;
        Invulnerable = false;

    }

    public void TakeDamage(float damage)
    {
        if (Invulnerable) { return; }
        damage *= DamageTakenMult;
        health = Mathf.Clamp(health - damage, 0, MaxHealth);
        if (health == 0) { Die(); }
    }

    public void HealDamage(float damage)
    {
        health = Mathf.Clamp(health + damage, 0, MaxHealth);
        if (health == 0) { Die(); }
    }

    private void Die()
    {

    }
}
