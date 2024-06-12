using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerJoinedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] texts;
    [SerializeField] private float inactiveAlpha;

    public void SetEntryActive(int index)
    {
        Color newColor = texts[index].color;
        newColor.a = 1;
        texts[index].color = newColor;
    }

    public void SetEntryInactive(int index)
    {
        Color newColor = texts[index].color;
        newColor.a = inactiveAlpha;
        texts[index].color = newColor;
    }
}
