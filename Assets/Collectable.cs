using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public MMF_Player collectedFeedbacks;
    public float destroyAfterCollectionTime = 1f;

    public bool isCollected = false;

    public void Collect()
    {
        isCollected = true;

        collectedFeedbacks?.PlayFeedbacks();

        Destroy(gameObject, destroyAfterCollectionTime);

        foreach (var meshRenderer in GetComponents<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }

        foreach (var meshRenderer in GetComponents<Collider>())
        {
            meshRenderer.enabled = false;
        }

        foreach (var meshRenderer in GetComponentsInChildren<Collider>())
        {
            meshRenderer.enabled = false;
        }
    }
}
