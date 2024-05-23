using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarManager : MonoBehaviour
{
    public static BossHealthBarManager Instance;

    [SerializeField] private Transform parent;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private float yOffset;

    private List<BossHealthBar> healthBarList = new();


    private void Awake()
    {
        Instance = this;
    }

    public BossHealthBar CreateHealthBar()
    {
        BossHealthBar healthBar = Instantiate(healthBarPrefab, parent).GetComponent<BossHealthBar>();
        healthBarList.Add(healthBar);
        healthBar.transform.Translate(new Vector2(0, yOffset * (healthBarList.Count - 1)));
        return healthBar;
    }

    public void DestroyHealthBar(BossHealthBar healthBar)
    {
        healthBarList.Remove(healthBar);
        Destroy(healthBar.gameObject);
    }
}
