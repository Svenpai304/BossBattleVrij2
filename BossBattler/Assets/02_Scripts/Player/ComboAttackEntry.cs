using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComboAttackEntry : ScriptableObject
{
    public int[] id = new int[2];
    public GameObject effectObject;

    public void Fire(characterStatus status)
    {
        Debug.Log("Using combo attack: " + id[0].ToString() + id[1].ToString());
        ComboAttack atk = Instantiate(effectObject).GetComponent<ComboAttack>();
        if(atk != null)
        {
            atk.OnFire(status);
        }
    }
}
