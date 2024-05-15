using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSpriteController : MonoBehaviour
{
    [SerializeField] private List<Sprite> cursorSprites = new List<Sprite>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float resetTime = 0.4f;
    private SciencePack sciencePack;

    public void Initialize(SciencePack _sciencePack)
    {
        sciencePack = _sciencePack;
        if (sciencePack != null)
        {
            sciencePack.CursorChanged += ChangeSprite;
        }
        else
        {
            Debug.LogWarning("SciencePack reference is null in CursorSpriteController.");
        }
    }

    private void OnDisable()
    {
        if (sciencePack != null)
        {
            sciencePack.CursorChanged -= ChangeSprite;
        }
    }

    private void ChangeSprite(int newIndex)
    {
        Debug.Log("Change Cursor Sprite to:" + newIndex);
        spriteRenderer.sprite = cursorSprites[newIndex];

        StopAllCoroutines();
        StartCoroutine(ResetSpriteAfterDelay());
    }

    private IEnumerator ResetSpriteAfterDelay()
    {
        yield return new WaitForSeconds(resetTime);
        spriteRenderer.sprite = cursorSprites[0];
    }
}
