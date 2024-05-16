using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CursorSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite[] cursorSprites = new Sprite[3];
    [SerializeField] private SpriteRenderer spriteRenderer;
    private SciencePack sciencePack;

    public void Initialize(SciencePack _sciencePack)
    {
        sciencePack = _sciencePack;
        sciencePack.CursorChanged += ChangeCursor;
    }

    private void OnDisable()
    {
        if (sciencePack == null) return;
        sciencePack.CursorChanged -= ChangeCursor;
    }

    private void Update()
    {
        spriteRenderer.sprite = cursorSprites[1];
    }

    private void ChangeCursor(int newIndex)
    {
        Debug.Log("Change Cursor Sprite to:" + newIndex);

        if (newIndex >= 0 && newIndex < cursorSprites.Length)
        {
            spriteRenderer.sprite = cursorSprites[newIndex];
            Debug.Log("Sprite set: " + spriteRenderer.sprite.name);
        }
    }
}
