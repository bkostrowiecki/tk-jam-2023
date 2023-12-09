using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponState : MonoBehaviour
{
    [Header("Dependencies")]
    public PlayerController playerController;
    public WeaponStates weaponStates;
    public Sacrifice sacrifice;
    public GameObject model;
    [Header("Debug")]
    public bool isDebugging;

    [Header("SO")]

    public InventoryItemSO inventoryItemSO;

    [Header("Attack animation parameters")]
    public string animationAttackTrigger;
    public float animationAttackSpeed;
    public float comboLastHitAnimationSpeed;

    

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

    
    float? attackTimer = null;
    List<Collider> alreadyHandled = new List<Collider>();
    Vector3 movementVector;
    private Vector3 jumpStepVector;
    bool isSacrificing = false;
    public int currentCombo = 0;
    
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
        if (Input.GetButtonDown("Fire1") && !attackTimer.HasValue && !sacrifice.IsSacrificing)
        {
            StartAttack();
        }

        if (Input.GetKeyDown(KeyCode.E) && !sacrifice.IsSacrificing && !attackTimer.HasValue)
        {
            if (sacrifice.MakeSacrifice(this))
            {
                currentCombo = 0;
            }
        }

        if (attackTimer.HasValue)
        {
            HandleAttack();
        }
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
