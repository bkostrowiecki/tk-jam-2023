using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonDelayed : MonoBehaviour
{
    public float time;
    
    public UnityEvent onDelayedClick = new UnityEvent();

    public void HandleOnClick()
    {
        StartCoroutine(HandleDelay(time));
    }

    IEnumerator HandleDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        onDelayedClick?.Invoke();
    }
}
