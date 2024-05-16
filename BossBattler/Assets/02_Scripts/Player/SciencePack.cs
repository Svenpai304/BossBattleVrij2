using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SciencePack : MonoBehaviour
{
    CharacterStatus status;
    CharacterUI ui;
    [SerializeField] private ComboElement[] elements = new ComboElement[3];
    [SerializeField] private Queue<ComboElement> currentElements = new Queue<ComboElement>();
    [SerializeField] private float cursorResetTime = 0.5f;
    public event Action<int> CursorChanged;
    
    private void Start()
    {
        status = GetComponent<CharacterStatus>();
        ui = status.ui;
        ui.SetOwnedElements(elements);
    }

    public void ChangeElement(int index, ComboElement element)
    {
        elements[index] = element;
    }

    public void OnFire(InputAction.CallbackContext c)
    {
        bool isFired = false;
        if (c.started)
        {
            if (currentElements.Count != 2) { return; }
            CursorChanged?.Invoke(2);
            StopAllCoroutines();
            StartCoroutine(resetCursor());
            ComboAttackEntry entry = ElementManager.GetAttackEntry(currentElements.First().id, currentElements.Last().id);
            if (entry != null) { entry.Fire(status); isFired = true; }
            else Debug.Log("Combo not found");
            currentElements.Clear();
            ui.ClearActiveElements();
        } 
        if (c.canceled)
        {
            if (isFired) return;
            CursorChanged?.Invoke(1);
            StopAllCoroutines();
            StartCoroutine(resetCursor());
        }
    }

    public void OnElement1(InputAction.CallbackContext c)
    {
        if(!c.started || elements[0] == null) { return; }
        UseElement(0);
    }

    public void OnElement2(InputAction.CallbackContext c)
    {
        if (!c.started || elements[1] == null) { return; }
        UseElement(1);
    }
    public void OnElement3(InputAction.CallbackContext c)
    {
        if (!c.started || elements[2] == null) { return; }
        UseElement(2);
    }

    private void UseElement(int index)
    {
        if (currentElements.Count > 1)
        {
            currentElements.Dequeue();
        }
        currentElements.Enqueue(elements[index]);
        if (currentElements.Count > 1)
        {
            ui.SetActiveElement(0, currentElements.Last());
        }   
        ui.SetActiveElement(1, currentElements.First());
    }

    private IEnumerator resetCursor()
    {
        yield return new WaitForSeconds(cursorResetTime);
        CursorChanged?.Invoke(0);
    }

}
