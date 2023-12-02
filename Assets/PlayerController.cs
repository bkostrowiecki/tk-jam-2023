using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    public GameObject statesContainer;

    [Header("Gravity")]
    public float gravity = 10f;
    public float maxGravityVelocity = 100f;
    float gravityVelocity;

    void Awake()
    {
        var children = statesContainer.GetComponentsInChildren<BasePlayerState>();

        foreach (var child in children)
        {
            child.playerController = this;
        }
    }

    public Vector3 AddGravity(Vector3 movement)
    {
        gravityVelocity += gravity;

        gravityVelocity = Mathf.Min(maxGravityVelocity, gravityVelocity);

        if (characterController.isGrounded)
        {
            gravityVelocity = -0.2f;
        }

        return movement + Vector3.down * gravityVelocity;
    }
}
