using System;
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
    private BasePlayerState[] states;

    void Awake()
    {
        states = statesContainer.GetComponentsInChildren<BasePlayerState>(true);

        foreach (var child in states)
        {
            child.playerController = this;
            child.gameObject.SetActive(false);
        }

        states[0].gameObject.SetActive(true);
    }

    public Vector3 AddGravity(Vector3 movement)
    {
        gravityVelocity += gravity * Time.deltaTime;

        gravityVelocity = Mathf.Min(maxGravityVelocity, gravityVelocity);

        if (characterController.isGrounded)
        {
            gravityVelocity = -0.2f;
        }

        return movement + Vector3.down * gravityVelocity;
    }

    public void ActivateState(BasePlayerState basePlayerState)
    {
        foreach (var child in states)
        {
            child.gameObject.SetActive(child == basePlayerState);
        }
    }
}
