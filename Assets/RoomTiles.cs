using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomTiles
{
    public RoomTile[] roomTiles;

    int height;
    int width;

    public List<RoomTile> AsList => new List<RoomTile>(roomTiles);

    public int Length => roomTiles.Length;

    public Vector2 Size => new Vector2(width, height);

    public RoomTiles(int sizeX, int sizeY)
    {
        height = sizeY;
        width = sizeX;
        roomTiles = new RoomTile[sizeX * sizeY];
    }

    public RoomTile Get(int x, int y)
    {
        return roomTiles[y * width + x];
    }

    public void Set(int x, int y, RoomTile roomTile)
    {
        roomTiles[y * width + x] = roomTile;
    }

    public void SetType(int x, int y, RoomTileType type)
    {
        roomTiles[y * width + x].tileType = type;
    }

    public (RoomTiles, RoomTiles) SplitHorizontally()
    {
        int tileY = Mathf.RoundToInt(height / 2);

        return SplitHorizontally(tileY);
    }

    public (RoomTiles, RoomTiles) SplitHorizontally(int tileY)
    {
        RoomTiles top = new(width, tileY);
        RoomTiles bottom = new(width, height - tileY);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y < tileY)
                {
                    top.Set(x, y, Get(x, y));
                }
                else
                {
                    var bottomY = y - tileY;
                    bottom.Set(x, bottomY, Get(x, y));
                }
            }
        }

        return (top, bottom);
    }

    public (RoomTiles, RoomTiles) SplitVertically()
    {
        int tileX = Mathf.RoundToInt(width / 2);

        return SplitVertically(tileX);
    }

    public (RoomTiles, RoomTiles) SplitVertically(int tileX)
    {
        RoomTiles left = new(tileX, height);
        RoomTiles right = new(width - tileX, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x < tileX)
                {
                    left.Set(x, y, Get(x, y));
                }
                else
                {
                    var rightX = x - tileX;
                    right.Set(rightX, y, Get(x, y));
                }
            }
        }

        return (left, right);
    }

    public RoomTiles GlueHorizontal(RoomTiles rightTiles)
    {
        throw new System.NotImplementedException();
    }

    public RoomTiles GlueVertical(RoomTiles rightTiles)
    {
        throw new System.NotImplementedException();
    }

    public int RandomX => Random.Range((int)0, width);
    public int RandomY => Random.Range((int)0, height);
}
