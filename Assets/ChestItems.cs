using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItems : MonoBehaviour
{
    public Inventory inventory;
    public FloatingTextSpawner floatingTextSpawner;
    
    public void GiveToPlayer()
    {
        var player = GameObject.Find("Player").GetComponent<PlayerController>();

        string text = "";
        foreach (var item in inventory.inventoryItems)
        {
            text += "+" + item.amount + " " + item.inventoryItemSO.name + "\n";
        }

        floatingTextSpawner.SpawnText(text);

        player.TakeItems(inventory);

        inventory.Clear();
    }
}
