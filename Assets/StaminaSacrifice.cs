using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSacrifice : BaseSacrifice
{
    public PlayerController playerController;
    public float fraction = 0.33f;

    public override void Execute()
    {
        playerController.staminaJumpUsage = Mathf.RoundToInt(playerController.staminaJumpUsage * (1f - 0.33f));
    }
}
