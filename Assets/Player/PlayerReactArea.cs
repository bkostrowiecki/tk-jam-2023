using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerReactArea : MonoBehaviour
{
    public float reactRadius = 1.5f;
    public LayerMask playerLayerMask;
    PlayerController playerController;
    public UnityEvent onReactionPossible = new UnityEvent();
    public UnityEvent onReactionImpossible = new UnityEvent();
    public UnityEvent onReact = new UnityEvent();

    public bool isOneTimer = true;

    void Update()
    {
        if (Physics.CheckSphere(transform.position, reactRadius, playerLayerMask))
        {
            var colliders = Physics.OverlapSphere(transform.position, reactRadius, playerLayerMask);

            var withTag = (new List<Collider>(colliders)).Find((item) => item.CompareTag("Player"));

            if (withTag != null)
            {
                playerController = withTag.GetComponent<PlayerController>();
            }
            else
            {
                onReactionImpossible?.Invoke();
                playerController = null;
            }
        }
        else
        {
            playerController = null;
        }

        if (playerController != null)
        {
            playerController.MakeReactionPossible(this);
            onReactionPossible?.Invoke();
        }
        else
        {
            onReactionImpossible?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reactRadius);
    }

    public void React()
    {
        onReact?.Invoke();

        if (isOneTimer)
        {
            enabled = false;
        }

        onReactionImpossible?.Invoke();
    }
}
