using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMPro.TMP_Text textTmp;
    public void SetText(string text)
    {
        textTmp.text = text;
    }

    public void SetColor(Color color)
    {
        textTmp.color = color;
    }
}
