using System.Collections;
using TMPro;
using UnityEngine;

public class MessageRevealer : MonoBehaviour
{
    [SerializeField, TextArea] private string MessageText = "";
    [SerializeField] private float textInterval = 0.1f;
    private TextMeshProUGUI text;
    
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeText(string _newText)
    {
        MessageText = _newText;
    }
    
    [ContextMenu("activate")]
    public void ActivateText()
    {
        StopAllCoroutines();        
        StartCoroutine(RevealText());
    }

    private IEnumerator RevealText()
    {
        text.text = "";
        foreach (char a in MessageText)
        {
            text.text += a;
            yield return new WaitForSeconds(textInterval);
        }
        
    }
    
}
