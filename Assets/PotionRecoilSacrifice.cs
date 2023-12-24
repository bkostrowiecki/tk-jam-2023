using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionRecoilSacrifice : BaseSacrifice
{
    public PlayerController playerController;

    public float timeSubtraction = 0.5f;
    public float minimalTime = 0.1f;

    public override void Execute()
    {
        playerController.potionUseBreakTime = Mathf.Max(minimalTime, playerController.potionUseBreakTime - timeSubtraction);
    }
}
