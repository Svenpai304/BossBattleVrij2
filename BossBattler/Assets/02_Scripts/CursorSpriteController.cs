using System.Collections.Generic;
using UnityEngine;

public class CursorSpriteController : MonoBehaviour
{
    [SerializeField] private List<Sprite> cursorSprites = new List<Sprite>();
    private SciencePack sciencePack;
    private SpriteRenderer spriteRenderer;

    public void Setup(SciencePack _sciencePack)
    {
        sciencePack = _sciencePack;
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sciencePack.onCursorChange += ChangeSprite;
    }
    private void OnDisable()
    {
        sciencePack.onCursorChange -= ChangeSprite;
    }

    private void ChangeSprite(int newIndex)
    {
        Debug.Log(newIndex + "index");

        if (spriteRenderer != null)
            spriteRenderer.sprite = cursorSprites[newIndex];
    }
}
