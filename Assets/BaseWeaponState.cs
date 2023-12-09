using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponState : MonoBehaviour
{
    [Header("Dependencies")]
    public PlayerController playerController;
    public WeaponStates weaponStates;
    public GameObject model;
    [Header("Debug")]
    public bool isDebugging;

    [Header("SO")]

    public InventoryItemSO inventoryItemSO;

    [Header("Attack animation parameters")]
    public string animationAttackTrigger;
    public float animationAttackSpeed;
    public float comboLastHitAnimationSpeed;

    [Header("Sacrifice animation parameters")]
    public string animationJumpSacrificeTrigger;
    public string animationFinishSacrificeTrigger;

    [Header("Slide parameters")]
    public float speed;
    public float slideDelay;
    public float slideTime;

    [Header("Hit parameters")]
    public float hitDelay;
    public float hitTime;
    public float hitRange;
    public LayerMask hittableLayerMask;
    public float recoilTime;
    public int comboLength;
    public float comboRecoilTime;

    [Header("Sacrifice parameter")]
    public float sacrificeJumpTime;
    public float sacrificeDoneTime;
    public float sacrificeRecoilTime;
    public float sacrificeRange;
    public Transform sacrificeTransform;

    public LayerMask sacrificableLayerMask;
    float? sacrificeTimer = null;

    float? attackTimer = null;
    List<Collider> alreadyHandled = new List<Collider>();
    Vector3 movementVector;
    private Vector3 jumpStepVector;
    bool isSacrificing = false;
    int currentCombo = 0;
    private Collider sacrificeTarget;

    void OnEnable()
    {
        model.SetActive(true);
    }

    void OnDisable()
    {
        model.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !attackTimer.HasValue && !sacrificeTimer.HasValue)
        {
            StartAttack();
        }

        if (Input.GetKeyDown(KeyCode.E) && !sacrificeTimer.HasValue && !attackTimer.HasValue)
        {
            FindSacrificeTarget();

            if (sacrificeTarget != null)
            {
                StartSacrifice();
            }
        }

        if (attackTimer.HasValue)
        {
            HandleAttack();
        }

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

                currentCombo = 0;

                playerController.UseWeapon();
            }
            else if (sacrificeTimer + sacrificeJumpTime + sacrificeDoneTime < Time.time)
            {
                sacrificeTarget.GetComponent<Sacrificable>().MakeLayDead();
                model.SetActive(false);
            }
        }
    }

    void FindSacrificeTarget()
    {
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
        return (playerController.transform.position -  transform.position).sqrMagnitude;
    }

    private void StartSacrifice()
    {
        currentCombo = 0;

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

    private void HandleAttack()
    {
        if (attackTimer + hitDelay < Time.time && attackTimer + hitDelay + hitTime > Time.time)
        {
            CheckAttackCollision();
        }

        if (attackTimer + slideDelay < Time.time && attackTimer + slideDelay + slideTime > Time.time)
        {
            playerController.characterController.Move(movementVector * Time.deltaTime);
        }

        if (attackTimer + hitDelay + hitTime < Time.time && attackTimer + hitDelay + hitTime + recoilTime > Time.time)
        {
            StartCoroutine(playerController.ResetTriggerAsync(animationAttackTrigger, 0f));
            alreadyHandled.Clear();

            if (Input.GetButtonDown("Fire1") && currentCombo < comboLength)
            {
                StartAttack();
            }

            if (currentCombo == comboLength)
            {
                playerController.HoldMovement(comboLastHitAnimationSpeed);
            }
        }

        if (attackTimer + hitDelay + hitTime + recoilTime < Time.time && currentCombo < comboLength)
        {
            attackTimer = null;
            playerController.RestoreMovement();

            currentCombo = 0;
        }

        if (attackTimer + hitDelay + hitTime + comboRecoilTime < Time.time && currentCombo >= comboLength)
        {
            attackTimer = null;
            playerController.RestoreMovement();

            currentCombo = 0;
        }
    }

    void StartAttack()
    {
        currentCombo++;

        playerController.HoldMovement(animationAttackSpeed);
        attackTimer = Time.time;

        StartCoroutine(playerController.SetTriggerAsync(animationAttackTrigger, 0f));

        movementVector = playerController.transform.forward * speed;
    }

    void CheckAttackCollision()
    {
        var colliders = Physics.OverlapSphere(transform.position, hitRange, hittableLayerMask);

        foreach (var collider in colliders)
        {
            if (alreadyHandled.Contains(collider))
            {
                continue;
            }

            var killable = collider.GetComponent<Killable>();

            if (killable != null)
            {
                killable.TakeDamage(new TouchDamage(inventoryItemSO.damage));

                alreadyHandled.Add(collider);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
