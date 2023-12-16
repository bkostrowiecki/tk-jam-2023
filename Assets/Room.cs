using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public Vector2Int size;

    public RoomFloor floor;

    public RoomTiles roomTiles;

    RoomBSP roomBSP;

    void Awake()
    {
        roomTiles = new (size.x, size.y);
    }

    [EditorCools.Button]
    public void GenerateFloor()
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                var isHole = RandomDice(0.34f);

                roomTiles.SetType(j, i, isHole ? RoomTileType.Hole : RoomTileType.Floor);
            }
        }

        floor.GenerateMesh(roomTiles);
    }

    [EditorCools.Button]
    public void GenerateBSP()
    {
        floor.GenerateBSPMesh(roomBSP);
    }

    [EditorCools.Button]
    public void GenerateRoomTiles()
    {
        roomTiles = new(size.x, size.y);
        roomBSP = new RoomBSP(roomTiles);
    }

    bool RandomDice(float chance)
    {
        int limit = Mathf.RoundToInt(6 * chance);

        var roll = Random.Range(1, 7);

        return limit > roll;
    }
}
