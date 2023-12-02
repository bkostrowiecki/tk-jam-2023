using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : BasePlayerState
{
    Vector3 prevMovement;
    Vector3 lastMovement;
    public float directionApplyTime = 0.05f;
    float lastMovementTimer;
    private Vector3 directionApplied;

    void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontalInput, 0, verticalInput);

        Vector3 inputByCamera = Camera.main.transform.rotation * input;

        Vector3 projectedInputByCamera = Vector3.ProjectOnPlane(inputByCamera, Vector3.up);

        Vector3 movement = projectedInputByCamera.normalized * 3;

        if (projectedInputByCamera.magnitude > 0.3)
        {
            if (lastMovement == movement)
            {
                lastMovementTimer += Time.deltaTime;
            }
            else
            {
                lastMovementTimer = 0;
            }
            lastMovement = movement;
        }

        if (lastMovementTimer > directionApplyTime)
        {
            directionApplied = lastMovement;
        }

        playerController.characterController.Move(movement * Time.deltaTime);
        playerController.characterController.transform.rotation = Quaternion.LookRotation(directionApplied);
    }
}
