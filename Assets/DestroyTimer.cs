using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float time = 3f;
    void OnEnable()
    {
        Destroy(gameObject, time);
    }
}
