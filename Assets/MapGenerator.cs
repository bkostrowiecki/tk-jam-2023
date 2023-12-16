using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public MapRoom roomPrefab;

    [Header("Tiny")]
    public float radius;
    [Header("UOT")]
    public Vector2 gridSize;
    public Vector2 tileSize;
    public bool shouldSqueeze;

    [Header("General")]
    public int roomsAmount = 10;
    public Vector2 roomSizeRange;
    private List<MapRoom> instances = new();
    private int lastStabilizedAmount;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    class Room2D
    {
        public Vector2 leftTop;
        public Vector2 rightBottom;

        public bool IsPointInRoom(Vector2 point)
        {
            return point.x >= leftTop.x && point.x <= rightBottom.x
                && point.y >= leftTop.y && point.y <= rightBottom.y;
        }

        public Vector2 Center => leftTop + ((rightBottom - leftTop) / 2f);

        public Vector2 Size => rightBottom - leftTop;

        public float Width => Size.x;
        public float Height => Size.y;
    }

    void Start()
    {
        GenerateUsingUOT();
    }

    void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 48;
        GUI.Label(new Rect(20, 20, 100, 100), new GUIContent(lastStabilizedAmount.ToString()) + "/" + instances.Count, style);
    }

    bool RandomDice(float chance)
    {
        int limit = Mathf.RoundToInt(6 * chance);

        var roll = Random.Range(1, 7);

        return limit > roll;
    }

    void GenerateUsingUOT()
    {
        List<Room2D> rooms = new List<Room2D>();

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                var shouldHaveRoom = RandomDice(0.66f);

                if (!shouldHaveRoom)
                {
                    continue;
                }

                var randomWidth = Mathf.CeilToInt(Random.Range(roomSizeRange.x, roomSizeRange.y));
                var randomHeight = Mathf.CeilToInt(Random.Range(roomSizeRange.x, roomSizeRange.y));

                float x = transform.position.x + (float)i * tileSize.x + tileSize.x * 0.5f;
                float y = transform.position.x + (float)j * tileSize.y + tileSize.y * 0.5f;

                Vector2 point = new Vector2(x, y);

                var room = new Room2D
                {
                    leftTop = new Vector2(point.x - (float)randomWidth / 2, point.y - (float)randomHeight / 2),
                    rightBottom = new Vector2(point.x + (float)randomWidth / 2, point.y + (float)randomHeight / 2)
                };

                rooms.Add(room);
            }
        }

        foreach (var room in rooms)
        {
            var instance = Instantiate(roomPrefab, new Vector3(room.Center.x, transform.position.y, room.Center.y), Quaternion.identity);
            instance.SetSize(room.Width, room.Height);

            instances.Add(instance);
        }
    }

    void FixedUpdate()
    {
        if (!shouldSqueeze)
        {
            return;
        }

        int stabilizedAmount = 0;
        foreach (var instance in instances)
        {
            if (instance.IsStabilized)
            {
                stabilizedAmount++;
            }
        }
        
        this.lastStabilizedAmount = stabilizedAmount;

        if (stabilizedAmount == instances.Count)
        {
            Debug.Log("Stabilized");
            return;
        }

        Vector3 center = transform.position + new Vector3(gridSize.x * tileSize.x * 0.5f, 0f, gridSize.y * tileSize.y * 0.5f);
        foreach (var instance in instances)
        {
            instance.SqueezeTorwards(center);
        }
    }

    void GenerateUsingTiny()
    {
        List<Room2D> rooms = new List<Room2D>();
        for (int i = 0; i < roomsAmount; i++)
        {
            Vector2 randomPoint;
            Vector2 gridPoint;
            do
            {
                randomPoint = GetRandomPointInCircle(radius);
                gridPoint = RoundToGrid(randomPoint);
            }
            while (IsPointInAnyRoom(rooms, gridPoint));

            var random3D = new Vector3(randomPoint.x, transform.position.y, randomPoint.y);

            var instance = Instantiate(roomPrefab, random3D, Quaternion.identity);

            var randomWidth = Random.Range(roomSizeRange.x, roomSizeRange.y);
            var randomHeight = Random.Range(roomSizeRange.x, roomSizeRange.y);

            instance.SetSize(randomWidth, randomHeight);

            var room = new Room2D
            {
                leftTop = new Vector2(randomPoint.x - randomWidth / 2, randomPoint.y - randomHeight / 2),
                rightBottom = new Vector2(randomPoint.x + randomWidth / 2, randomPoint.y + randomHeight / 2)
            };

            rooms.Add(room);
        }
    }

    private Vector2 RoundToGrid(Vector2 point)
    {
        return new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
    }

    bool IsPointInAnyRoom(List<Room2D> rooms, Vector2 point)
    {
        foreach (var room in rooms)
        {
            if (room.IsPointInRoom(point))
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetRandomPointInCircle(float radius)
    {
        float t = 2 * Mathf.PI * Random.Range(0f, 1f);
        float u = Random.Range(0f, 1f) + Random.Range(0f, 1f);
        float r;
        if (u > 1) r = 2 - u;
        else r = u;
        return new Vector2(radius * r * Mathf.Cos(t), radius * r * Mathf.Sin(t));
    }
}
