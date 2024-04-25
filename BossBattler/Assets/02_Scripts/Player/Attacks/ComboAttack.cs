using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComboAttack : MonoBehaviour
{

    public abstract void OnFire(CharacterStatus _status);
}
