using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFloor : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Mesh mesh;

    public void GenerateMesh(RoomTiles roomTiles)
    {
        mesh = new Mesh();

        List<Vector3> verts = new();
        List<Vector2> uvs = new();
        List<int> faces = new();

        Vector2[] tileUvs = new [] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1),
        };

        int index = 0;
        for (int y = 0; y < roomTiles.Size.y; y++)
        {
            for (int x = 0; x < roomTiles.Size.x; x++)
            {
                if (roomTiles.Get(x, y).tileType == RoomTileType.Floor)
                {
                    index = GenerateFloor(verts, uvs, faces, tileUvs, index, y, x);
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = faces.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    public void GenerateBSPMesh(RoomBSP roomBSP)
    {
        var roomTiles = roomBSP.TraverseAndGlue();

        GenerateMesh(roomTiles);
    }

    void Traverse(RoomBSPNode roomBSPNode)
    {
        if (roomBSPNode.Left != null && roomBSPNode.Right != null)
        {
            Traverse(roomBSPNode.Left);
            Traverse(roomBSPNode.Right);
        }
    }

    int GenerateFloor(List<Vector3> verts, List<Vector2> uvs, List<int> faces, Vector2[] tileUvs, int index, int y, int x)
    {
        Vector3 vec = MapXYtoXZ(new Vector2(x, y));

        verts.Add(vec);
        verts.Add(vec + Vector3.forward);
        verts.Add(vec + Vector3.forward + Vector3.right);

        verts.Add(vec);
        verts.Add(vec + Vector3.forward + Vector3.right);
        verts.Add(vec + Vector3.right);

        uvs.AddRange(tileUvs);

        faces.AddRange(new[] { index, index + 1, index + 2, index + 3, index + 4, index + 5 });

        index += 6;

        return index;
    }

    Vector3 MapXYtoXZ(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0f, vector2.y);
    }
}
