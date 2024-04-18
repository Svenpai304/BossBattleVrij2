using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SciencePack : MonoBehaviour
{
    characterStatus status;

    [SerializeField] private ComboElement[] elements = new ComboElement[3];
    [SerializeField] private Queue<ComboElement> currentElements = new Queue<ComboElement>();

    private void Start()
    {
        status = GetComponent<characterStatus>();
    }

    public void ChangeElement(int index, ComboElement element)
    {
        elements[index] = element;
    }

    public void OnFire(InputAction.CallbackContext c)
    {
        if(currentElements.Count < 2) { return; }
        Debug.Log($"Firing elements: {currentElements.First().name} + {currentElements.Last().name}");
        currentElements.Clear();
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
    }

}
