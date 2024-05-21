
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStatus : MonoBehaviour, IStatus, IDamageable
{
    CharacterLook cl;
    [HideInInspector] public CharacterUI ui;

    List<CharacterBuff> Buffs = new List<CharacterBuff>();

    public int MaxHealth;
    public float MaxPower;
    public float MaxDashTime;
    public float PowerRegen;

    public bool isCasting;

    public float DamageDealMult { get; set; }
    public float DamageTakenMult { get; set; }
    public float GroundSpeedMult { get; set; }
    public float PowerRegenMult { get; set; }
    public bool Invulnerable { get; set; }

    public float Health { get { return health; } set { health = value; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get { return dashTime; } set { dashTime = value; } }
    public Vector2 LookDirection { get { return cl.LookDirection; } }

    [SerializeField]private float health;
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
        UpdateBuffs();
    }

    private void UpdateBuffs()
    {
        DamageDealMult = 1f;
        DamageTakenMult = 1f;
        GroundSpeedMult = 1f;
        PowerRegenMult = 1f;
        foreach (CharacterBuff buff in new List<CharacterBuff>(Buffs))
        {
            buff.Duration -= Time.deltaTime;
            if (buff.Duration < 0f)
            {
                //(Ryan) [07/05/24 17:53] Werkt dit okay met memory om gewoon de class uit de lijst te halen?
                Buffs.Remove(buff);
            } else
            {
                for (int i = 0; i < buff.CurStacks; i++) //For each stack of this buff
                {
                    DamageDealMult *= buff.DamageDone;
                    DamageTakenMult *= buff.DamageTaken;
                    GroundSpeedMult *= buff.MovementSpeed;
                }
            }
        }
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

    public void BuffDamageTaken(string _ID, float _dur, int _maxStacks, float BuffMod)
    {
        CharacterBuff oldBuff = getBuff(_ID);
        if (oldBuff == null)
        {
            CharacterBuff newBuff = new CharacterBuff(_ID, _dur, _maxStacks, BuffMod < 1f);
            newBuff.DamageTaken = BuffMod;
            Buffs.Add(newBuff);
        } else
        {
            oldBuff.CurStacks = Mathf.Min(_maxStacks, oldBuff.CurStacks + 1);
            oldBuff.Duration = Mathf.Max(_dur, oldBuff.Duration);
            if (BuffMod < 1f) //If the overwriting buff is stronger than the old buff, overwrite old buff
            {
                if (BuffMod < oldBuff.DamageTaken) oldBuff.DamageTaken = BuffMod;
            } else
            {
                if (BuffMod > oldBuff.DamageTaken) oldBuff.DamageTaken = BuffMod;
            }
        }
    }
    public void BuffDamageDone(string _ID, float _dur, int _maxStacks, float BuffMod)
    {
        CharacterBuff oldBuff = getBuff(_ID);
        if (oldBuff == null)
        {
            CharacterBuff newBuff = new CharacterBuff(_ID, _dur, _maxStacks, BuffMod > 1f);
            newBuff.DamageDone = BuffMod;
            Buffs.Add(newBuff);
        } else
        {
            oldBuff.CurStacks = Mathf.Min(_maxStacks, oldBuff.CurStacks + 1);
            oldBuff.Duration = Mathf.Max(_dur, oldBuff.Duration);
            if (BuffMod < 1f) //If the overwriting buff is stronger than the old buff, overwrite old buff
            {
                if (BuffMod < oldBuff.DamageDone) oldBuff.DamageDone = BuffMod;
            }
            else
            {
                if (BuffMod > oldBuff.DamageDone) oldBuff.DamageDone = BuffMod;
            }
        }
    }
    public void BuffMoveSpeed(string _ID, float _dur, int _maxStacks, float BuffMod)
    {
        CharacterBuff oldBuff = getBuff(_ID);
        if (oldBuff == null)
        {
            CharacterBuff newBuff = new CharacterBuff(_ID, _dur, _maxStacks, BuffMod > 1f);
            newBuff.MovementSpeed = BuffMod;
            Buffs.Add(newBuff);
        } else
        {
            oldBuff.CurStacks = Mathf.Min(_maxStacks, oldBuff.CurStacks + 1);
            oldBuff.Duration = Mathf.Max(_dur, oldBuff.Duration);
            if (BuffMod < 1f) //If the overwriting buff is stronger than the old buff, overwrite old buff
            {
                if (BuffMod < oldBuff.MovementSpeed) oldBuff.MovementSpeed = BuffMod;
            }
            else
            {
                if (BuffMod > oldBuff.MovementSpeed) oldBuff.MovementSpeed = BuffMod;
            }
        }
    }
    public CharacterBuff getBuff(string _ID)
    {
        foreach (CharacterBuff buff in Buffs)
        {
            if (buff.ID.Equals(_ID)) return buff;
        }
        return null;
    }

    public float Dist(Transform other)
    {
        //Distance to other player
        return (transform.position - other.position).magnitude;
    }
    public float Dist(Vector3 po)
    {
        //Distance to other player
        return (transform.position - po).magnitude;
    }
}

public class CharacterBuff {
    public int MaxStacks = 1;
    public int CurStacks = 0;
    public string ID = "";
    public bool isPositive = true;
    public float Duration;
    public float DamageTaken = 1f;
    public float DamageDone = 1f;
    public float MovementSpeed = 1f;
    //public float DamageDone = 1f;
    public CharacterBuff(string _ID, float _dur, int _maxStacks, bool _positive)
    {
        ID = _ID;
        isPositive = _positive;
        Duration = _dur;
        MaxStacks = _maxStacks;
        CurStacks = 1;
    }
}