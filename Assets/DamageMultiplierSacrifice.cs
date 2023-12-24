using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMultiplierSacrifice : BaseSacrifice
{
    public PlayerController playerController;

    public float damageIncrease = 0.25f;

    public float damageMultiplierMax = 2f;

    public override void Execute()
    {
        playerController.damageMultiplier = Mathf.Min(damageMultiplierMax, playerController.damageMultiplier + damageIncrease);
    }
}
