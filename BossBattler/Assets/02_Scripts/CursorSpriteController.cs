using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CursorSpriteController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int statesCount;
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

    private void ChangeCursor(int newIndex)
    {
        if (!gameObject.activeSelf) { Debug.Log("Cursor inactive"); return; }
        Debug.Log("Change Cursor Sprite to: " + newIndex);

        if(newIndex > -1 && newIndex < statesCount)
        {
            StartCoroutine(WaitAFrame(newIndex));
        }
    }

    private IEnumerator WaitAFrame(int newIndex)
    {
        yield return new WaitForNextFrameUnit();

        animator.SetInteger("Sprites", newIndex);
        Debug.Log("Set sprite: " + animator.GetInteger("Sprites"));

    }
}
