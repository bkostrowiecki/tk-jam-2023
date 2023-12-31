using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Sacrifice : MonoBehaviour
{
    public PlayerController playerController;

    [Header("Sacrifice animation parameters")]
    public string animationJumpSacrificeTrigger;
    public string animationFinishSacrificeTrigger;

    [Header("Effects")]
    public GameObject effectsContainer;
    public FloatingTextSpawner floatingTextSpawner;

    [Header("Sacrifice parameter")]
    public float sacrificeJumpTime;
    public float sacrificeDoneTime;
    public float sacrificeRecoilTime;
    public float sacrificeRange;
    public Transform sacrificeTransform;

    public LayerMask sacrificableLayerMask;
    float? sacrificeTimer = null;
    Collider sacrificeTarget;
    private BaseWeaponState weaponState;

    Vector3 movementVector;
    private Vector3 jumpStepVector;
    public MMF_Player sacrificeFeedbacks;

    bool haveUsedWeapon = false;
    BaseSacrifice[] sacrificeEffects;

    void Awake()
    {
        sacrificeEffects = effectsContainer.GetComponentsInChildren<BaseSacrifice>();
    }

    void ExecuteWeaponSO(InventoryItemSO inventoryItemSO)
    {
        FindEffectBySO(inventoryItemSO).Execute();

        floatingTextSpawner.SpawnText(inventoryItemSO.shortDescription);
    }

    BaseSacrifice FindEffectBySO(InventoryItemSO inventoryItemSO)
    {
        var effects = new List<BaseSacrifice>(sacrificeEffects);

        return effects.Find((item) => item.weaponSO == inventoryItemSO);
    }

    void Update()
    {
        if (sacrificeTimer.HasValue)
        {
            if (sacrificeTimer + sacrificeJumpTime < Time.time && sacrificeTimer + sacrificeJumpTime + sacrificeDoneTime > Time.time)
            {
                sacrificeTarget.GetComponent<Sacrificable>().MakeLayDown();

                playerController.animator.SetTrigger(animationFinishSacrificeTrigger);

                var projected = Vector3.ProjectOnPlane(sacrificeTarget.transform.forward, Vector3.up);
                playerController.transform.forward = projected;
            }
            else if (sacrificeTimer + sacrificeJumpTime > Time.time)
            {
                var difference = sacrificeTarget.transform.position - playerController.transform.position;
                var direction = difference.normalized;

                var projected = Vector3.ProjectOnPlane(direction, Vector3.up);

                playerController.transform.forward = projected;

                playerController.characterController.Move(jumpStepVector * Time.deltaTime);
            }

            if (sacrificeTimer + sacrificeJumpTime + sacrificeDoneTime + sacrificeRecoilTime < Time.time)
            {
                playerController.RestoreMovement();
                sacrificeTimer = null;
                weaponState.currentCombo = 0;

                StartCoroutine(playerController.ResetTriggerAsync(animationFinishSacrificeTrigger, 0f));

                weaponState.model.SetActive(true);
                haveUsedWeapon = false;

            }
            else if (sacrificeTimer + sacrificeJumpTime + sacrificeDoneTime < Time.time)
            {
                StartCoroutine(playerController.ResetTriggerAsync(animationJumpSacrificeTrigger, 0f));

                if (!haveUsedWeapon)
                {
                    var selectedWeaponSO = playerController.selectedWeaponSO;

                    playerController.UseWeapon();
                    haveUsedWeapon = true;

                    if (sacrificeTarget != null)
                    {
                        ExecuteWeaponSO(selectedWeaponSO);
                        sacrificeTarget.GetComponent<Sacrificable>().MakeLayDead();
                    }

                    weaponState.model.SetActive(false);
                }
            }
        }
    }

    void FindSacrificeTarget()
    {
        sacrificeTarget = null;

        var colliders = Physics.OverlapSphere(sacrificeTransform.position, sacrificeRange, sacrificableLayerMask);

        Collider closest = null;

        float minDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            var distance = CalculateDistanceToPlayer(collider.transform);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = collider;
            }
        }

        if (closest != null)
        {
            sacrificeTarget = closest;
        }
    }

    public float CalculateDistanceToPlayer(Transform transform)
    {
        return (playerController.transform.position - transform.position).sqrMagnitude;
    }

    public void StartSacrifice(BaseWeaponState weaponState)
    {
        this.weaponState = weaponState;

        playerController.HoldMovement(1f);
        sacrificeTimer = Time.time;

        StartCoroutine(playerController.SetTriggerAsync(animationJumpSacrificeTrigger, 0f));

        var difference = sacrificeTarget.transform.position - playerController.transform.position;
        var direction = difference.normalized;
        var distance = difference.magnitude;

        var projected = Vector3.ProjectOnPlane(direction, Vector3.up);

        playerController.transform.forward = projected;

        movementVector = projected;

        jumpStepVector = projected * (distance / sacrificeJumpTime);

        sacrificeTarget.GetComponent<Sacrificable>().StartSacrifice();
    }

    public bool MakeSacrifice(BaseWeaponState weaponState)
    {
        FindSacrificeTarget();

        if (sacrificeTarget != null)
        {
            StartSacrifice(weaponState);

            sacrificeFeedbacks?.PlayFeedbacks();

            return true;
        }

        return false;
    }

    public bool IsSacrificing => sacrificeTimer.HasValue;
}
