using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HittingEnemyState : BaseEnemyState
{
    public NavMeshAgentSteeringOverride steeringOverride;
    public ChasingEnemyState chasingEnemyState;
    public BaseEnemyState returnEnemyState;
    private Vector3 startPosition;
    Vector3 hitDirection;
    private Vector3 hitDestination;
    public float hitDistance;
    public float hitTime = 1f;
    public AnimationCurve hitAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    float timer;
    private Vector3 newPosition;
    public List<Vector2> hitTimeRanges;
    public GameObject hitBox;
    public float doubleHitDistance;
    public string animationTrigger;

    void OnEnable()
    {
        DisableSteering();
        PrepareHit();
    }

    void OnDisable()
    {
        EnableSteering();
    }

    private void PrepareHit()
    {
        if (enemyAI.SensePlayerForSure() && enemyAI.SurePlayerPosition.HasValue)
        {
            startPosition = enemyAI.transform.position;
            hitDirection = Vector3.ProjectOnPlane((enemyAI.SurePlayerPosition.Value - enemyAI.transform.position).normalized, Vector3.up);
            enemyAI.transform.forward = hitDirection;
            hitDestination = enemyAI.transform.position + hitDirection * hitDistance;
            enemyAI.animator.SetTrigger(animationTrigger);
        }
        else
        {
            enemyAI.ActivateState(chasingEnemyState);
        }
    }

    void Update()
    {
        Hit();
    }

    void Hit()
    {
        if (timer >= hitTime)
        {
            enemyAI.animator.ResetTrigger(animationTrigger);

            timer = 0f;

            newPosition = hitDestination;

            if (enemyAI.SensePlayerForSure() && enemyAI.SurePlayerPosition.HasValue && enemyAI.DistanceToPlayer < doubleHitDistance)
            {
                enemyAI.transform.position = newPosition;

                PrepareHit();
            }
            else
            {
                enemyAI.navMeshAgent.transform.position = newPosition;
                enemyAI.ActivateState(returnEnemyState);
            }

            return;
        }

        bool enabledHitBox = false;
        foreach (var hitTimeRange in hitTimeRanges)
        {
            if (timer > hitTimeRange.x && timer < hitTimeRange.y)
            {
                EnableHitBox();
                enabledHitBox = true;
            }
        }
        if (!enabledHitBox)
        {
            DisableHitBox();
        }

        timer += Time.deltaTime;

        newPosition = Vector3.Lerp(startPosition, hitDestination, hitAnimationCurve.Evaluate(timer / hitTime));

        enemyAI.navMeshAgent.transform.position = newPosition;
    }

    void DisableHitBox()
    {
        hitBox.gameObject.SetActive(false);
    }

    void EnableHitBox()
    {
        hitBox.gameObject.SetActive(true);
    }
}
