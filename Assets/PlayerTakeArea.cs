using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerTakeArea : MonoBehaviour
{
    public float takeRadius = 1.5f;
    public LayerMask playerLayerMask;
    Transform playerTransform;
    public float speed;

    void Update()
    {
        if (Physics.CheckSphere(transform.position, takeRadius, playerLayerMask))
        {
            var colliders = Physics.OverlapSphere(transform.position, takeRadius, playerLayerMask);

            var playerCollider = colliders[0];

            playerTransform = playerCollider.transform;
        }

        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;

            Vector3 step = direction * speed * Time.deltaTime;

            transform.position += step;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, takeRadius);
    }
}
