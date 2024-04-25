using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    public static ElementManager instance;
    private Dictionary<int[], ComboAttackEntry> attackDict = new Dictionary<int[], ComboAttackEntry>(new EqualityComparer());

    private void Awake()
    {
        instance = this;
        ComboAttackEntry[] attacks = Resources.FindObjectsOfTypeAll<ComboAttackEntry>();
        foreach (ComboAttackEntry attack in attacks)
        {
            attackDict.Add(attack.id, attack);
            Debug.Log("Added attack with ID: " + attack.id[0] + attack.id[1]);
        }
    }

    public static ComboAttackEntry GetAttackEntry(int id1, int id2)
    {
        int[] key = new int[2];
        if (id1 > id2)
        {
            key[0] = id2; key[1] = id1;
        }
        else
        {
            key[1] = id2; key[0] = id1;
        }

        if (instance.attackDict.ContainsKey(key))
        {
            return instance.attackDict[key];
        }
        else
        {
            return null;
        }
    }

    public class EqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }
}
