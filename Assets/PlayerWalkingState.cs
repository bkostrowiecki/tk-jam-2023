using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : BasePlayerState
{
    Vector3 prevMovement;
    Vector3 lastProjectedInputByCamera;
    public float directionApplyTime = 0.05f;
    public float accelerationTime = 0.5f;
    public float speed = 3f;
    public float deadZone = 0.2f;
    float lastProjectedInputByCameraTimer;
    private Vector3 directionApplied;
    private Vector3 lastGoalMovement;
    private float accelerationProgress;
    public AnimationCurve accelerationCurve;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 120, 120), new GUIContent("Direction applied" + directionApplied.ToString()));
        GUI.Label(new Rect(120, 120, 120, 120), new GUIContent("Acceleration progress " + accelerationProgress.ToString()));
        GUI.Label(new Rect(220, 220, 120, 120), new GUIContent("lastMovementTimer " + lastProjectedInputByCameraTimer.ToString()));
    }

    void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontalInput, 0, verticalInput);

        Vector3 inputByCamera = Camera.main.transform.rotation * input;

        Vector3 projectedInputByCamera = Vector3.ProjectOnPlane(inputByCamera, Vector3.up);

        if (input.magnitude > 0)
        {
            accelerationProgress += Time.deltaTime;
        }
        else
        {
            accelerationProgress -= Time.deltaTime;
        }

        accelerationProgress = Mathf.Clamp(accelerationProgress, 0f, accelerationTime);

        Vector3 goalMovement = projectedInputByCamera == Vector3.zero
            ? lastGoalMovement
            : projectedInputByCamera.normalized * speed;

        float lerpProgress = accelerationCurve.Evaluate(accelerationProgress / accelerationTime);

        HandleDirection(projectedInputByCamera, goalMovement);

        Vector3 movement = Vector3.Lerp(Vector3.zero, goalMovement, lerpProgress);

        playerController.characterController.Move(movement * Time.deltaTime);
        playerController.characterController.transform.rotation = Quaternion.LookRotation(directionApplied);
    }

    void HandleDirection(Vector3 projectedInputByCamera, Vector3 goalMovement)
    {
        if (projectedInputByCamera.magnitude > deadZone)
        {
            if (lastProjectedInputByCamera == projectedInputByCamera)
            {
                lastProjectedInputByCameraTimer += Time.deltaTime;
            }
            else
            {
                lastProjectedInputByCameraTimer = 0;
            }
            lastProjectedInputByCamera = projectedInputByCamera;
        }
        else
        {
            lastProjectedInputByCameraTimer = 0;
        }

        if (lastProjectedInputByCameraTimer > directionApplyTime)
        {
            directionApplied = lastProjectedInputByCamera.normalized;
            lastGoalMovement = goalMovement;
        }
    }
}
