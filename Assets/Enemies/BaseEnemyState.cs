using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyState : MonoBehaviour
{
    public EnemyAI enemyAI;

    public void OverrideNavMeshAgentSteering(NavMeshAgentSteeringOverride navMeshAgentSettingsOverride)
    {
        if (navMeshAgentSettingsOverride.shouldOverrideAcceleration)
        {
            enemyAI.navMeshAgent.acceleration = navMeshAgentSettingsOverride.acceleration;
        }

        if (navMeshAgentSettingsOverride.shouldOverrideSpeed)
        {
            enemyAI.navMeshAgent.speed = navMeshAgentSettingsOverride.speed;
        }

        if (navMeshAgentSettingsOverride.shouldOverrideAngularSpeed)
        {
            enemyAI.navMeshAgent.angularSpeed = navMeshAgentSettingsOverride.angularSpeed;
        }
    }

    public void DisableSteering()
    {
        enemyAI.navMeshAgent.enabled = false;
    }

    public void EnableSteering()
    {
        enemyAI.navMeshAgent.enabled = true;
    }
}
