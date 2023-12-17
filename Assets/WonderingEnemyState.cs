using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class NavMeshAgentSteeringOverride
{
    public bool shouldOverrideSpeed;
    public float speed;
    public bool shouldOverrideAngularSpeed;
    public float angularSpeed;
    public bool shouldOverrideAcceleration;
    public float acceleration;
}

public class WonderingEnemyState : BaseEnemyState
{
    public float wonderingDistance = 6f;
    float WonderingRadius => wonderingDistance;
    public Vector3 breakTimeRange = new Vector2(2f, 4f);
    float breakTimer;
    Vector3? nextDestination = null;
    public LayerMask wallsLayerMask;
    public NavMeshAgentSteeringOverride navMeshAgentSteeringOverride;
    public ChasingEnemyState chasingState;

    void OnEnable()
    {
        AssignNewBreak();
        OverrideNavMeshAgentSteering(navMeshAgentSteeringOverride);
    }

    void AssignNewBreak()
    {
        breakTimer = UnityEngine.Random.Range(breakTimeRange.x, breakTimeRange.y);
    }

    void Update()
    {
        if (enemyAI.navMeshAgent.hasPath && enemyAI.navMeshAgent.remainingDistance < 0.1f)
        {
            nextDestination = null;
            AssignNewBreak();
        }

        if (nextDestination == null)
        {
            nextDestination = AssignNewDestination();
        }

        if (enemyAI.SensePlayer())
        {
            enemyAI.ActivateState(chasingState);
            return;
        }

        if (breakTimer > 0f)
        {
            breakTimer -= Time.deltaTime;
            enemyAI.enemyDebug.Log("Break " + breakTimer.ToString());
            return;
        }

        if (!enemyAI.navMeshAgent.hasPath)
        {
            if (nextDestination.HasValue)
            {
                enemyAI.navMeshAgent.SetDestination(nextDestination.Value);
            }
            else
            {
                nextDestination = AssignNewDestination();
                enemyAI.navMeshAgent.SetDestination(nextDestination.Value);
            }
        }
    }

    Vector3? AssignNewDestination()
    {
        return RandomNavMeshLocation(WonderingRadius);
    }

    public Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = RandomObjectiveNotColliding(radius);

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public Vector3 RandomObjectiveNotColliding(float radius)
    {
        bool isGood = false;
        Vector3 randomDirection = Vector3.zero;
        while (!isGood)
        {
            randomDirection = UnityEngine.Random.insideUnitSphere * radius;

            if (!Physics.Raycast(transform.position, randomDirection, radius * 2f, wallsLayerMask))
            {
                randomDirection += transform.position;
                isGood = true;
            }
        }

        return randomDirection;
    }
}
