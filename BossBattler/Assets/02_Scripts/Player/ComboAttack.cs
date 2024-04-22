using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComboAttack : MonoBehaviour
{
    private characterStatus status;

    public abstract void OnFire(characterStatus _status);
}
