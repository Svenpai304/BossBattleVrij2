
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStatus : MonoBehaviour, IStatus, IDamageable
{
    CharacterLook cl;
    [HideInInspector] public CharacterUI ui;

    public int MaxHealth;
    public float MaxPower;
    public float MaxDashTime;
    public float PowerRegen;

    public float DamageDealMult { get; set; }
    public float DamageTakenMult { get; set; }
    public float GroundSpeedMult { get; set; }
    public float PowerRegenMult { get; set; }
    public bool Invulnerable { get; set; }

    public float Health { get { return health; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get { return dashTime; } set { dashTime = value; } }
    public Vector2 LookDirection { get { return cl.LookDirection; } }

    private float health;
    private float power;
    private float dashTime;
    private float powerRegenDelay;

    public float getPowerDamageMod()
    {
        return (power / MaxPower)*1.2f;
    }
    public void usePower(float pow)
    {
        Power -= pow;
        powerRegenDelay = getDefaultPowerRegenDelay();
    }

    private float getDefaultPowerRegenDelay()
    {
        return 3f;
    }
    public void Setup(CharacterUI ui)
    {
        this.ui = ui;
        this.ui.Setup(this);
        cl = GetComponent<CharacterLook>();
        ResetStatus();
    }

    private void FixedUpdate()
    {
        float mod = 1f;
        if (powerRegenDelay > 0f)
        {
            powerRegenDelay -= Time.deltaTime;
            mod = ((getDefaultPowerRegenDelay()-powerRegenDelay) /getDefaultPowerRegenDelay())*0.4f;
        }
        power = Mathf.Clamp(power + PowerRegen * PowerRegenMult * mod * Time.deltaTime, 0, MaxPower);
        ui.SetPowerBar(power, MaxPower);
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
        ui.SetHealthBar(health, MaxHealth);
        if (health == 0) { Die(); }
    }

    public void HealDamage(float damage)
    {
        health = Mathf.Clamp(health + damage, 0, MaxHealth);
        ui.SetHealthBar(health, MaxHealth);
        if (health == 0) { Die(); }
    }

    private void Die()
    {
        Debug.Log("ouchie");
    }
}
