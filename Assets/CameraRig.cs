using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    Vector3 position;
    Vector3 relativePosition;
    public Transform playerTransform;

    void Start()
    {
        position = transform.position;
        relativePosition = position - playerTransform.position;
    }

    void Update()
    {
        transform.position = playerTransform.position + relativePosition;
    }
}
