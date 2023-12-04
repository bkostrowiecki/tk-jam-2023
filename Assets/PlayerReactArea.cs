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

    void Update()
    {
        if (Physics.CheckSphere(transform.position, reactRadius, playerLayerMask))
        {
            var colliders = Physics.OverlapSphere(transform.position, reactRadius, playerLayerMask);

            var playerCollider = colliders[0];

            playerController = playerCollider.GetComponent<PlayerController>();
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reactRadius);
    }
}
