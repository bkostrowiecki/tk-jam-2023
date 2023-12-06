using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimePotionUsage : PotionUsage
{
    public override IEnumerator UseAsync(PlayerController playerController)
    {
        feedbacks?.PlayFeedbacks();

        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(1.5f);

        Time.timeScale = 1f;

        feedbacks?.StopFeedbacks();
    }
}
