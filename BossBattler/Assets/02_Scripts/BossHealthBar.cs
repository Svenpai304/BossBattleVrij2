using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image healthBar;
    [SerializeField] private float speed;

    private float maxHealth;
    private float health;
    private float displayedHealth;
    
    public void Setup(float max, string name)
    {
        maxHealth = max;
        health = max;
        displayedHealth = health;
        nameText.text = name;
    }

    public void SetHealth(float _health)
    {
        health = _health;
    }

    private void Update()
    {
        if (displayedHealth != health)
        {
            displayedHealth += Mathf.Sign(health - displayedHealth) * speed * Mathf.Abs(health - displayedHealth) * Time.deltaTime;
            if(Mathf.Abs(displayedHealth - health) < 0.5f)
            {
                displayedHealth = health;
            }
            healthBar.fillAmount = displayedHealth / maxHealth;
        }
    }
}
