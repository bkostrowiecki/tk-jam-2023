using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    public bool isOn;
    public Animator animator;
    public FloatingTextSpawner floatingTextSpawner;
    public UnityEvent MadeOn = new UnityEvent();
    public UnityEvent MadeOff = new UnityEvent();
    public UnityEvent Toggled = new UnityEvent();

    public void MakeOn()
    {
        isOn = true;

        SpawnText();
    }

    public void MakeOff()
    {
        isOn = false;

        SpawnText();
    }

    public void Toggle()
    {
        isOn = !isOn;

        SpawnText();
    }

    private void SpawnText()
    {
        EmitEvents();

        animator.SetBool("isOn", isOn);

        floatingTextSpawner.SpawnText(isOn ? "Switch is ON" : "Switch is OFF");
    }

    public void EmitEvents()
    {
        Toggled?.Invoke();

        if (isOn)
        {
            MadeOn?.Invoke();
        }
        else
        {
            MadeOff?.Invoke();
        }
    }
}
