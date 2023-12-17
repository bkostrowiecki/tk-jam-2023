using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float time = 3f;
    public bool justDisable = false;
    void OnEnable()
    {
        if (justDisable)
        {
            Disable(gameObject, time);
        }
        else
        {
            Destroy(gameObject, time);
        }
    }

    void Disable(GameObject gameObject, float time)
    {
        StartCoroutine(DisableAsync(gameObject, time));
    }

    IEnumerator DisableAsync(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
