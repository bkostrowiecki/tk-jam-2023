using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDebug : MonoBehaviour
{
    public TMPro.TMP_Text tmp;
    string text = "";
    public void Log(string content)
    {
        text += content + "\n";
    }

    void LateUpdate()
    {
        tmp.text = text;

        text = "";
    }
}
