using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SequenceSaver : MonoBehaviour
{
    public FOVAdjuster fovAdjuster;
    public RenderTargetToPNG renderTargetToPNG;

    [EditorCools.Button]
    void SaveAll()
    {
        var colliders = GetComponentsInChildren<Collider>();

        // HandleAll(colliders);
        StartCoroutine(HandleAllAsync(colliders));
    }

    [EditorCools.Button]
    void SaveSelected()
    {
        StartCoroutine(HandleColliderAsync(selected));
    }

    public Collider selected;

    private void HandleAll(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            HandleCollider(collider);
        }

        Debug.Log("DONE");
    }

    private void HandleCollider(Collider collider)
    {
        fovAdjuster.SetupForCollider(collider);
        renderTargetToPNG.Save(collider);
    }

    private IEnumerator HandleAllAsync(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            yield return HandleColliderAsync(collider);
        }

        Debug.Log("DONE");
    }

    IEnumerator HandleColliderAsync(Collider collider)
    {
        fovAdjuster.SetupForCollider(collider);

        yield return new WaitForSecondsRealtime(1f);

        renderTargetToPNG.Save(collider);

        yield return new WaitForSecondsRealtime(1f);
    }
}
