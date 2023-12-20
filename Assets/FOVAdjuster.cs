using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FOVAdjuster : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    public float cameraDistance = 1f;
    public float elevation = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    [EditorCools.Button]
    void Setup()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();

        var centerAt = vcam.Follow.gameObject;
        Collider collider = centerAt.GetComponent<Collider>();
        SetupForCollider(collider);
    }

    public void SetupForCollider(Collider collider)
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        vcam.Follow = collider.transform;
        vcam.LookAt = collider.transform;

        var centerAt = vcam.Follow.gameObject;
        OffsetOverride offsetOverride = centerAt.GetComponent<OffsetOverride>();

        Vector3 objectSizes = collider.bounds.max - collider.bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * vcam.m_Lens.FieldOfView); // Visible height 1 meter in front

        float distance = objectSize / cameraView; // Combined wanted distance from the object

        distance -= 0.1f * objectSize; // Estimated offset from the center to the outside of the object

        vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(distance * 0.5f, 0f, -distance);

        if (offsetOverride != null)
        {
            vcam.GetCinemachineComponent<CinemachineComposer>().m_ScreenX = offsetOverride.screenOffset.x;
            vcam.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = offsetOverride.screenOffset.y;
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.5f;
            vcam.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.5f;
        }

        Camera.main.Render();
    }
}
