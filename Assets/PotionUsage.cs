using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class PotionUsage : MonoBehaviour
{
    public InventoryItemSO inventoryItemSO;
    public MMF_Player feedbacks;

    public abstract IEnumerator UseAsync(PlayerController playerController);
}
