using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundTouchPoint : MonoBehaviour
{
    public PlayerController playerController;

    void Update()
    {
        transform.position = playerController.characterController.transform.position + (playerController.characterController.height * 0.5f * Vector3.down);
    }

    [EditorCools.Button]
    void SetPosition()
    {
        Update();
    }
}
