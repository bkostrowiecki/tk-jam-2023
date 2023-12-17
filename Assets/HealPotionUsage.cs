using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotionUsage : PotionUsage
{
    public int healthPoints;

    public override IEnumerator UseAsync(PlayerController playerController)
    {
        playerController.killable.TakeHealing(new HealDamage(healthPoints));

        yield return null;

        feedbacks?.PlayFeedbacks();
    }
}

public class HealDamage : BaseDamage
{
    int healthPoints;

    public HealDamage(int healthPoints)
    {
        this.healthPoints = healthPoints;
    }

    public int CalculateDamage()
    {
        return this.healthPoints;
    }
}