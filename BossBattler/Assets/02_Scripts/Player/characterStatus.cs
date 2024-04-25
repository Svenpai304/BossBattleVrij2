
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

    public float Health { get { return health; } set { health = value; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get { return dashTime; } set { dashTime = value; } }

    [SerializeField, Unity.Collections.ReadOnly] private float health;
    [SerializeField, Unity.Collections.ReadOnly] private float power;
    [SerializeField, Unity.Collections.ReadOnly] private float dashTime;

    private void Awake()
    {
        
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
