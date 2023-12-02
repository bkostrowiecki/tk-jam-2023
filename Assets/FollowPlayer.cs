using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        transform.position = playerTransform.position;
    }

    [EditorCools.Button]
    void SetPosition()
    {
        Update();
    }
}
