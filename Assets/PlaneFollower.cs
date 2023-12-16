using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFollower : MonoBehaviour
{
    public Transform toFollow;

    void Update()
    {
        if (toFollow == null)
        {
            enabled = false;
            return;
        }

        var direction = toFollow.forward;
        var projected = Vector3.ProjectOnPlane(direction, Vector3.up);

        transform.forward = projected;
    }
}
