using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSacrifice : BaseSacrifice
{
    public PlayerController playerController;
    public float fraction = 0.1f;

    public override void Execute()
    {
        playerController.killable.maxHealthPoints += Mathf.CeilToInt(fraction * 100);
        playerController.killable.TakeHealing(new HealDamage(Mathf.CeilToInt(fraction * 100)));
    }
}