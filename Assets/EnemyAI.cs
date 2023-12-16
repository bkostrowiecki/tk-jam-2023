using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyDebug enemyDebug;
    List<BaseEnemyState> states;
    public GameObject statesContainer;
    public NavMeshAgent navMeshAgent;
    public EnemySensor sensor;
    GameObject player;
    Vector3? lastPlayerPosition = null;
    public Vector3? LastPlayerPosition => lastPlayerPosition;
    public PlayerDetector playerCloseDetector;
    public float memoryTime;
    float memoryTimer = 0f;
    bool isPlayerSeen = false;

    public BaseEnemyState CurrentState
    {
        get
        {
            foreach (var state in states)
            {
                if (state.gameObject.activeInHierarchy)
                {
                    return state;
                }
            }

            return null;
        }
    }

    void Awake()
    {
        states = new List<BaseEnemyState>(statesContainer.GetComponentsInChildren<BaseEnemyState>(true));

        foreach (var state in states)
        {
            state.enemyAI = this;
        }

        states[0].gameObject.SetActive(true);
    }

    void Update()
    {
        enemyDebug.Log(CurrentState.gameObject.name);

        if (!isPlayerSeen && lastPlayerPosition.HasValue)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                ForgetPlayer();
            }
        }
    }

    void ForgetPlayer()
    {
        lastPlayerPosition = null;
    }

    public bool SensePlayer()
    {
        GameObject[] buffer = new GameObject[1];

        int result = sensor.Filter("Player", buffer);

        var isDetected = playerCloseDetector.Detect();

        if (result > 0)
        {
            player = buffer[0];
            lastPlayerPosition = player.transform.position;
            memoryTimer = memoryTime;
        }
        else if (isDetected)
        {
            player = playerCloseDetector.DetectedPlayer;
            lastPlayerPosition = player.transform.position;
            memoryTimer = memoryTime;
        }
        else
        {
            player = null;
        }

        isPlayerSeen = lastPlayerPosition.HasValue;

        return isPlayerSeen;
    }

    public void ActivateState(BaseEnemyState baseEnemyState)
    {
        foreach (var child in states)
        {
            child.gameObject.SetActive(child == baseEnemyState);
        }
    }
}
