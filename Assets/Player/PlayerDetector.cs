using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DetectionMode
{
    Update = 0,
    Manual = 1
}

public class PlayerDetector : MonoBehaviour
{
    public DetectionMode detectionMode;
    public LayerMask playerLayerMask;
    public LayerMask occlusionLayers;
    public float detectionRadius;

    public UnityEvent<GameObject> onDetected = new();
    bool isDetected = false;
    bool IsDetected => isDetected;
    GameObject detectedPlayer;
    public GameObject DetectedPlayer => detectedPlayer;
    public bool shouldCheckForObstacles = false;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void Update()
    {
        if (detectionMode == DetectionMode.Update)
        {
            Detect();
        }
    }

    public bool Detect()
    {
        isDetected = false;
        if (Physics.CheckSphere(transform.position, detectionRadius, playerLayerMask))
        {
            var overlapped = Physics.OverlapSphere(transform.position, detectionRadius, playerLayerMask);

            foreach (var obj in overlapped)
            {
                var origin = obj.transform.position;
                var dest = transform.position;

                Debug.DrawLine(origin, dest, Color.black);

                if (Physics.Linecast(origin, dest, occlusionLayers))
                {
                    continue;
                }

                isDetected = true;
                detectedPlayer = obj.gameObject;

                onDetected?.Invoke(detectedPlayer);

                break;
            }
        }

        return isDetected;
    }
}
