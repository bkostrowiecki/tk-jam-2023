using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapRoom : MonoBehaviour
{
    public UnityEvent OnStabilized = new UnityEvent();
    public float stabilizationThreshold = 0.01f;
    bool isStabilized = false;
    public bool IsStabilized => isStabilized;

    void Start()
    {
        Color randomColor = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
    }
    public void SetSize(float width, float height)
    {
        transform.localScale = new Vector3(width, 1f, height);
    }

    public void SqueezeTorwards(Vector3 center)
    {
        var direction = center - transform.position;
        GetComponent<Rigidbody>().AddForce(direction * 0.3f, ForceMode.Impulse);
    }

    void LateUpdate()
    {
        if (!isStabilized && GetComponent<Rigidbody>().velocity.magnitude < stabilizationThreshold)
        {
            OnStabilized?.Invoke();
            isStabilized = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
