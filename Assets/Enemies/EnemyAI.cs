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
    Vector3? lastPlayerPosition = null;
    public Vector3? LastPlayerPosition => lastPlayerPosition;
    public PlayerDetector playerCloseDetector;
    public float memoryTime;
    float memoryTimer = 0f;
    bool isPlayerSeen = false;
    bool isPlayerVisible;
    Vector3? surePlayerPosition;
    Vector3 lastPosition;
    float currentVelocity;
    float currentSpeed;
    public float maxSpeed;
    public Animator animator;

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

    public Vector3? SurePlayerPosition => surePlayerPosition;

    public float DistanceToPlayer
    {
        get
        {
            if (SurePlayerPosition.HasValue)
            {
                return (SurePlayerPosition.Value - transform.position).magnitude;
            }
            
            return Mathf.Infinity;
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

        if (isPlayerSeen && lastPlayerPosition.HasValue)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                ForgetPlayer();
            }
        }

        Vector3 velocity = (lastPosition - transform.position) / Time.deltaTime;
        lastPosition = transform.position;

        currentSpeed = velocity.magnitude;

        animator?.SetFloat("speed", currentSpeed / maxSpeed);
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

        GameObject player;

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

        isPlayerSeen = lastPlayerPosition.HasValue;

        return isPlayerSeen;
    }

    public bool SensePlayerForSure()
    {
        GameObject[] buffer = new GameObject[1];

        int result = sensor.Filter("Player", buffer);

        var isDetected = playerCloseDetector.Detect();

        GameObject player;

        if (result > 0)
        {
            player = buffer[0];
            surePlayerPosition = player.transform.position;
            lastPlayerPosition = player.transform.position;
        }
        else if (isDetected)
        {
            player = playerCloseDetector.DetectedPlayer;
            surePlayerPosition = player.transform.position;
            lastPlayerPosition = player.transform.position;
        }

        isPlayerVisible = result > 0 || isDetected;

        return isPlayerVisible;
    }

    public void ActivateState(BaseEnemyState baseEnemyState)
    {
        foreach (var child in states)
        {
            child.gameObject.SetActive(child == baseEnemyState);
        }
    }
}
