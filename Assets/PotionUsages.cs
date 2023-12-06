using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionUsages : MonoBehaviour
{
    PotionUsage[] potionUsages;
    public PlayerController playerController;

    void Awake()
    {
        potionUsages = GetComponentsInChildren<PotionUsage>();
    }

    public void UsePotion(InventoryItemSO inventoryItemSO)
    {
        if (!inventoryItemSO.IsPotion)
        {
            throw new System.Exception("Selected inventory item is not a potion " + inventoryItemSO.name);
        }

        var potionUsage = FindPotionUsageByInventoryItemSO(inventoryItemSO);

        if (potionUsage == null)
        {
            Debug.LogError("Selected inventory item has not PotionUsage declared in Potion Usages " + inventoryItemSO.name, gameObject);

            return;
        }

        StartCoroutine(potionUsage.UseAsync(playerController));
    }

    PotionUsage FindPotionUsageByInventoryItemSO(InventoryItemSO inventoryItemSO)
    {
        foreach (var usage in potionUsages)
        {
            if (usage.inventoryItemSO == inventoryItemSO)
            {
                return usage;
            }
        }

        return null;
    }
}
