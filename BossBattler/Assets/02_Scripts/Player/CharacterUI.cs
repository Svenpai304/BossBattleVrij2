using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private CharacterStatus status;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image powerBar;
    [SerializeField] private List<Image> ownedElements = new List<Image>();
    [SerializeField] private List<Image> activeElements = new List<Image>();


    public void Setup(CharacterStatus _status)
    {
        gameObject.SetActive(true);
        status = _status;
    }

    public void SetHealthBar(float value, float max)
    {
        healthBar.fillAmount = value / max;
    }

    public void SetPowerBar(float value, float max)
    {
        powerBar.fillAmount = value / max;
        powerBar.color = new Color(0.5f-(value/max), (value / max)*1.3f-0.3f, 0.5f+(value / max)*0.5f);
    }

    public void SetActiveElement(int index, ComboElement element)
    {
        if(index < activeElements.Count)
        {
            activeElements[index].sprite = element.icon;
            activeElements[index].color = Color.white;
        }
    }

    public void ClearActiveElements()
    {
        foreach(var element in activeElements)
        {
            element.sprite = null;
            element.color = Color.clear;
        }
    }

    public void SetOwnedElements(ComboElement[] newElements)
    {
        for(int i = 0; i < ownedElements.Count; i++)
        {
            ownedElements[i].sprite = newElements[i].icon;
            ownedElements[i].color = Color.white;
        }
    }
}
