using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComboElement : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField] public string elementName;
    [SerializeField] public Sprite icon;
}
