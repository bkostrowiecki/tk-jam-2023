using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodIncreaseSacrifice : BaseSacrifice
{
    public PlayerController playerController;

    public int increase = 20;
    public int max = 150;

    public override void Execute()
    {
        playerController.bloodIncrease = Mathf.Min(max, playerController.bloodIncrease += increase);
    }
}
