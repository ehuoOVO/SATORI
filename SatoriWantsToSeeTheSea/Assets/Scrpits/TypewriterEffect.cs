using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public float waiting_seconds = Constants.Default_Waiting_Seconds;

    private Coroutine TC;
    private bool Typing;

    public void UpdateSpeed(float t)
    {
        waiting_seconds = t;
    }
    public void StartTyping(string text)
    {
        if (TC != null) StopCoroutine(TC);
        TC = StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string text)
    {
        Typing = true;
        textDisplay.text = text;
        textDisplay.maxVisibleCharacters = 0;
        
        for(int i=0;i <= text.Length; i++)
        {
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(waiting_seconds);
        }

        Typing = false;
    }

    public void CompleteTyping()
    {
        if(TC != null)  StopCoroutine(TC);
        textDisplay.maxVisibleCharacters = textDisplay.text.Length;
        Typing = false;
    }

    public bool isTyping()
    {
        return Typing;
    }
}
