using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingEnemyState : BaseEnemyState
{
    public WonderingEnemyState wonderingEnemyState;
    public HittingEnemyState hitEnemyState;
    public NavMeshAgentSteeringOverride navMeshAgentSteeringOverride;

    public float hitDistance = 1.5f;

    void OnEnable()
    {
        OverrideNavMeshAgentSteering(navMeshAgentSteeringOverride);
    }

    void FixedUpdate()
    {
        if (enemyAI.SensePlayer())
        {
            enemyAI.navMeshAgent.SetDestination(enemyAI.LastPlayerPosition.Value);

            if (enemyAI.navMeshAgent.remainingDistance < hitDistance)
            {
                if (enemyAI.SensePlayerForSure())
                {
                    enemyAI.ActivateState(hitEnemyState);
                }
            }
        }
        else
        {
            enemyAI.ActivateState(wonderingEnemyState);
        }
    }
}
