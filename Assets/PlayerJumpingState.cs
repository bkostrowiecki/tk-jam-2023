using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerJumpingState : BasePlayerState
{
    public float jumpDistance = 3f;
    public float jumpSpeed = 5f;
    private Vector3 jumpDirection;
    private Vector3 startPoint;
    private Vector3 projectedInputByCamera;
    private float jumpTime;
    private float jumpStartTimer;
    private float previousEvaluated;
    public AnimationCurve jumpCurve;
    public PlayerWalkingState playerWalkingState;
    private float cachedHeight;

    void OnEnable()
    {
        cachedHeight = playerController.characterController.height;
        playerController.characterController.height = 1;
    }

    void OnDisable()
    {
        playerController.characterController.height = cachedHeight;
    }

    public void JumpTorwards(Vector3 rawInput)
    {
        jumpDirection = rawInput.normalized;
        startPoint = playerController.characterController.transform.position;

        projectedInputByCamera = playerWalkingState.ProjectInputToWorld(rawInput);

        jumpTime = jumpDistance / jumpSpeed;
        jumpStartTimer = Time.time;

        previousEvaluated = jumpCurve.Evaluate(0f);
    }

    void Update()
    {
        var relativeTime = Time.time - jumpStartTimer;

        var evaluated = jumpCurve.Evaluate(relativeTime / jumpTime);
        var step = evaluated - previousEvaluated;

        playerWalkingState.HandleDirection(projectedInputByCamera, jumpDirection);

        playerController.characterController.Move(jumpSpeed * jumpDirection * Time.deltaTime);

        if (jumpStartTimer + jumpTime < Time.time)
        {
            playerController.ActivateState(playerWalkingState);
        }
    }
}