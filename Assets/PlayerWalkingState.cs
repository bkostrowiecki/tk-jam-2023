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
    private Vector3 previousDirectionApplied;
    private Vector3 directionApplied;
    private float directionAppliedTime;
    private Vector3 lastGoalMovement;
    private float accelerationProgress;
    public AnimationCurve accelerationCurve;
    public float directionLerpTime = 0.25f;
    float directionAppliedTimer;
    public AnimationCurve rotationLerpCurve;

    public PlayerJumpingState playerJumpingState;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 120, 120), new GUIContent("Direction applied" + directionApplied.ToString()));
        GUI.Label(new Rect(120, 120, 120, 120), new GUIContent("Acceleration progress " + accelerationProgress.ToString()));
        GUI.Label(new Rect(220, 220, 120, 120), new GUIContent("lastMovementTimer " + lastProjectedInputByCameraTimer.ToString()));
        GUI.Label(new Rect(120, 320, 120, 120), new GUIContent("previousDirectionApplied " + previousDirectionApplied.ToString()));
    }

    void Start()
    {
        previousDirectionApplied = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        directionApplied = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        directionAppliedTimer = Time.time;
    }

    void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 rawInput = new Vector3(horizontalInput, 0, verticalInput);

        if (Input.GetButtonDown("Jump"))
        {
            playerController.ActivateState(playerJumpingState);
            playerJumpingState.JumpTorwards(directionApplied);
        }

        Vector3 projectedInputByCamera = ProjectInputToWorld(rawInput);

        HandleAccelerationProgress(rawInput);

        Vector3 goalMovement = projectedInputByCamera == Vector3.zero
            ? lastGoalMovement
            : projectedInputByCamera.normalized * speed;

        HandleDirection(projectedInputByCamera, goalMovement);
        
        ApplyMovement(goalMovement);
        ApplyRotation();
    }

    private void ApplyMovement(Vector3 goalMovement)
    {
        float lerpProgress = accelerationCurve.Evaluate(accelerationProgress / accelerationTime);
        Vector3 movement = Vector3.Lerp(Vector3.zero, goalMovement, lerpProgress);

        playerController.characterController.Move(playerController.AddGravity(movement) * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        var rotationLerpProgress = rotationLerpCurve.Evaluate((Time.time - directionAppliedTimer) / directionLerpTime);

        var rotationLerp = Vector3.Lerp(previousDirectionApplied, directionApplied, rotationLerpProgress);

        playerController.characterController.transform.rotation = Quaternion.LookRotation(rotationLerp);
    }

    public Vector3 ProjectInputToWorld(Vector3 rawInput)
    {
        Vector3 inputByCamera = Camera.main.transform.rotation * rawInput.normalized;

        Vector3 projectedInputByCamera = Vector3.ProjectOnPlane(inputByCamera, Vector3.up);

        Debug.DrawRay(transform.position, projectedInputByCamera, Color.red);

        return projectedInputByCamera;
    }

    private void HandleAccelerationProgress(Vector3 input)
    {
        if (input.magnitude > 0)
        {
            accelerationProgress += Time.deltaTime;
        }
        else
        {
            accelerationProgress -= Time.deltaTime;
        }

        accelerationProgress = Mathf.Clamp(accelerationProgress, 0f, accelerationTime);
    }

    public void HandleDirection(Vector3 projectedInputByCamera, Vector3 goalMovement)
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
            if (lastProjectedInputByCamera.normalized != directionApplied)
            {
                previousDirectionApplied = directionApplied;
                directionApplied = lastProjectedInputByCamera.normalized;
                directionAppliedTimer = Time.time;
            }
            lastGoalMovement = goalMovement;
        }
    }
}
