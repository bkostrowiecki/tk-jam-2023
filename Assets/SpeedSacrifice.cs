using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSacrifice : BaseSacrifice
{
    public PlayerWalkingState playerWalkingState;
    public float fraction = 0.1f;

    public override void Execute()
    {
        playerWalkingState.speed += fraction * 7f;
    }
}

public abstract class BaseSacrifice : MonoBehaviour
{
    public InventoryItemSO weaponSO;

    public abstract void Execute();
}
