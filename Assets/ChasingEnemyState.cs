using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingEnemyState : BaseEnemyState
{
    public WonderingEnemyState wonderingEnemyState;
    public NavMeshAgentSteeringOverride navMeshAgentSteeringOverride;

    void OnEnable()
    {
        OverrideNavMeshAgentSteering(navMeshAgentSteeringOverride);
    }

    void FixedUpdate()
    {
        if (enemyAI.SensePlayer())
        {
            enemyAI.navMeshAgent.SetDestination(enemyAI.LastPlayerPosition.Value);
        }
        else
        {
            enemyAI.ActivateState(wonderingEnemyState);
        }
    }
}
