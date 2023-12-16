using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class RoomBSP
{
    RoomBSPNode rootNode;

    public RoomBSPNode RootNode => rootNode;

    public RoomBSP(RoomTiles roomTiles)
    {
        rootNode = new RoomBSPNode(roomTiles);
    }

    public RoomTiles TraverseAndGlue()
    {
        return RootNode.TraverseAndGlue();
    }
}

public enum RoomBSPSplit
{
    None = 0,
    Horizontal = 1,
    Vertical = 2
}

public class RoomBSPNode
{
    float minWidth = 3;
    float minHeight = 3;
    float maxWidth = 6;
    float maxHeight = 6;

    public RoomTiles roomTiles;

    RoomBSPNode left;
    RoomBSPNode right;
    private RoomBSPSplit splitType;

    public RoomBSPNode Left => left;
    public RoomBSPNode Right => right;

    public RoomBSPNode(RoomTiles roomTiles)
    {
        this.roomTiles = roomTiles;

        splitType = RoomBSPSplit.None;

        SetRandomTile();

        SplitAtRandom();
    }

    void SetRandomTile()
    {
        bool isHole = RandomDice(0.33333f);

        for (int x = 0; x < roomTiles.Size.x; x++)
        {
            for (int y = 0; y < roomTiles.Size.y; y++)
            {
                roomTiles.SetType(x, y, isHole ? RoomTileType.Hole : RoomTileType.Floor);
            }
        }
    }

    bool RandomDice(float chance)
    {
        int limit = Mathf.RoundToInt(6 * chance);

        var roll = Random.Range(1, 7);

        return limit > roll;
    }

    public void SplitAtRandom()
    {
        float rand = Random.value;

        if (rand < 0.5f && roomTiles.Size.x >= 2 * minWidth)
        {
            VerticalSplit();
            return;
        }
        else if (roomTiles.Size.y >= 2 * minHeight)
        {
            HorizontalSplit();
            return;
        }

        // Force split if theres too much space.
        if (roomTiles.Size.x > maxWidth)
        {
            VerticalSplit();
            return;
        }

        if (roomTiles.Size.y > maxHeight)
        {
            HorizontalSplit();
            return;
        }
    }

    void HorizontalSplit()
    {
        var topBottom = roomTiles.SplitHorizontally(roomTiles.RandomY);

        left = new RoomBSPNode(topBottom.Item1);
        right = new RoomBSPNode(topBottom.Item2);

        splitType = RoomBSPSplit.Horizontal;
    }

    void VerticalSplit()
    {
        var leftRight = roomTiles.SplitVertically(roomTiles.RandomX);

        left = new RoomBSPNode(leftRight.Item1);
        right = new RoomBSPNode(leftRight.Item2);

        splitType = RoomBSPSplit.Vertical;
    }

    public RoomTiles TraverseAndGlue()
    {
        if (left != null && right != null)
        {
            var leftTiles = left.TraverseAndGlue();
            var rightTiles = right.TraverseAndGlue();

            if (splitType == RoomBSPSplit.Horizontal)
            {
                return leftTiles.GlueHorizontal(rightTiles);
            }
            else if (splitType == RoomBSPSplit.Vertical)
            {
                return leftTiles.GlueVertical(rightTiles);
            }

            throw new Exception("Unsupported split");
        }
        else
        {
            return roomTiles;
        }
    }
}