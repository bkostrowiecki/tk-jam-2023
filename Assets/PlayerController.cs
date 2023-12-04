using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class ReactDistance
{
    public PlayerReactArea area;
    public float distance;
}

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    public GameObject statesContainer;
    [Header("Input")]
    public Transform playerRotationBaseTransform;
    [Header("Internals")]
    public Killable killable;

    [Header("Gravity")]
    public float gravity = 10f;
    public float maxGravityVelocity = 100f;
    float gravityVelocity;
    private BasePlayerState[] states;
    [Header("Hell")]
    public float hellHeight = -20f;
    public float snapshotTime = 1f;
    float lastSnapshotTimer = 0f;
    public List<Vector3> snapshots = new();
    public LayerMask hellDetectionLayers;

    [Header("Collectable")]
    public LayerMask collectablesLayerMask;
    float collectablesDetectionRadius = 0.5f;

    [Header("Reactions")]
    public List<ReactDistance> playerReactAreas = new();

    [Header("Immortality")]
    public MMF_Player becomeImmortalFeedbacks = new();
    public float hitTimeImmortality = 0.5f;

    void Awake()
    {
        states = statesContainer.GetComponentsInChildren<BasePlayerState>(true);

        foreach (var child in states)
        {
            child.playerController = this;
            child.gameObject.SetActive(false);
        }

        states[0].gameObject.SetActive(true);

        lastSnapshotTimer = Time.time;
        MakeSnapshot();
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

    void Update()
    {
        if (transform.position.y < hellHeight)
        {
            TakeHellDamage();
            RecoverPosition();
        }

        if (lastSnapshotTimer + snapshotTime < Time.time)
        {
            Ray ray = new Ray(transform.position + characterController.height * Vector3.down * 0.5f, Vector3.down);

            if (Physics.Raycast(ray, 5f, hellDetectionLayers))
            {
                MakeSnapshot();
                lastSnapshotTimer = Time.time;
            }
        }

        DetectCollectable();
    }

    void LateUpdate()
    {
        ResetPossibleReactions();
    }

    void ResetPossibleReactions()
    {
        playerReactAreas.Clear();
    }

    void DetectCollectable()
    {
        var collectablesOverlapping = Physics.OverlapSphere(transform.position, collectablesDetectionRadius, collectablesLayerMask);

        foreach (var collectableOverlapping in collectablesOverlapping)
        {
            var collectable = collectableOverlapping.GetComponent<Collectable>();

            collectable.Collect();
        }
    }

    #if UNITY_EDITOR
    void OnGUI()
    {
        GUI.Label(new Rect(40, 40, 120, 120), new GUIContent(playerReactAreas.ToArray().ToString()));
    }
    #endif

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectablesDetectionRadius);
    }

    void RecoverPosition()
    {
        characterController.enabled = false;
        characterController.transform.SetPositionAndRotation(snapshots[1] + Vector3.up, characterController.transform.rotation);

        StartCoroutine(RecoverPositionAsync());
    }

    IEnumerator RecoverPositionAsync()
    {
        yield return new WaitForEndOfFrame();
        characterController.enabled = true;
    }

    void MakeSnapshot()
    {
        if (snapshots.Count > 2)
        {
            snapshots.RemoveAt(0);
        }
        snapshots.Add(transform.position);
    }

    void TakeHellDamage()
    {
        killable.TakeDamage(new HellDamage(killable.maxHealthPoints));
        MakeImmortalForTime(hitTimeImmortality, 0.1f);
    }

    public void MakeReactionPossible(PlayerReactArea playerReactArea)
    {
        var found = playerReactAreas.Find((item) => item.area == playerReactArea);

        if (found != null)
        {
            found.distance = CalculateDistance(playerReactArea.transform.position);
        }
        else
        {
            var newEntry = new ReactDistance
            {
                distance = CalculateDistance(playerReactArea.transform.position),
                area = playerReactArea
            };

            playerReactAreas.Add(newEntry);
        }
    }

    public void MakeImmortalForTime(float time, float feedbackDelay)
    {
        StartCoroutine(MakeImmortalForTimeAsync(time, feedbackDelay));
    }

    public IEnumerator MakeImmortalForTimeAsync(float time, float feedbackDelay)
    {
        killable.enabled = false;

        yield return new WaitForSeconds(feedbackDelay);
        becomeImmortalFeedbacks?.PlayFeedbacks();

        yield return new WaitForSeconds(time - feedbackDelay);

        becomeImmortalFeedbacks?.StopFeedbacks();
        killable.enabled = true;
    }

    float CalculateDistance(Vector3 position)
    {
        return (transform.position - position).sqrMagnitude;
    }
}

public class HellDamage : BaseDamage
{
    int playerMaxHealth;

    public HellDamage(int playerMaxHealth)
    {
        this.playerMaxHealth = playerMaxHealth;
    }

    public int CalculateDamage()
    {
        return playerMaxHealth / 5;
    }
}