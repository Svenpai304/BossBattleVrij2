using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComboAttack : MonoBehaviour
{
    private CharacterStatus status;

    public abstract void OnFire(CharacterStatus _status);
}
