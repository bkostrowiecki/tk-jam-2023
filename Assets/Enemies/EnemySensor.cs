using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnemySensor : MonoBehaviour
{
    public float distance = 10f;
    public float angle = 30f;
    public float height = 1f;

    public Color meshColor;
    private Mesh mesh;

    Collider[] colliders = new Collider[150];
    public List<GameObject> objects = new();

    public float scanFrequency = 0.5f;

    int count;
    float scanInterval;
    float scanTimer;
    public LayerMask layers;
    public LayerMask occlusionLayers;

    void Start()
    {
        scanInterval = 1f / scanFrequency;
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        objects.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;

            if (IsInSight(obj))
            {
                objects.Add(obj);
            }
        }
    }

    public int Filter(LayerMask layerMask, GameObject[] buffer)
    {
        int count = 0;

        foreach (var obj in objects)
        {
            if (layerMask == (layerMask | (1 << obj.layer)))
            {
                buffer[count++] = obj;
            }

            if (buffer.Length == count)
            {
                break;
            }
        }

        return count;
    }

    public int Filter(string tag, GameObject[] buffer)
    {
        int count = 0;

        foreach (var obj in objects)
        {
            if (obj.tag == tag)
            {
                buffer[count++] = obj;
            }

            if (buffer.Length == count)
            {
                break;
            }
        }

        return count;
    }

    public bool IsInSight(GameObject go)
    {
        Vector3 origin = transform.position;
        Vector3 dest = go.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0f;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > angle)
        {
            return false;
        }

        origin.y += height * 0.75f;
        dest.y = origin.y;

        if (Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTri = (segments * 4) + 2 + 2;
        int numVert = numTri * 3;

        UnityEngine.Vector3[] vertices = new Vector3[numVert];
        int[] triangles = new int[numVert];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {
            currentAngle += deltaAngle;

            bottomLeft = Quaternion.Euler(0, currentAngle - deltaAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
        }

        for (int i = 0; i < numVert; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }

    void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }
}
