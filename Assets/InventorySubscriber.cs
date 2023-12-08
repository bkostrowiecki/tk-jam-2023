using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class InventorySubscriber : MonoBehaviour
{
    public PlayerController playerController;
    public Transform gridContainer;
    public InventoryItemSlot prefab;

    Dictionary<InventoryItem, InventoryItemSlot> inventoryItemSlots = new Dictionary<InventoryItem, InventoryItemSlot>();

    void Start()
    {
        playerController.InventoryItemsObservable.Subscribe((inventoryItems) =>
        {
            UpdateSlots(inventoryItems);
        });
    }

    void UpdateSlots(List<InventoryItem> inventoryItems)
    {
        List<InventoryItem> copy = new List<InventoryItem>(inventoryItems.ToArray());
        foreach (var inventoryItem in inventoryItems)
        {
            var found = inventoryItemSlots.ContainsKey(inventoryItem);

            if (!found)
            {
                CreateSlot(inventoryItem);
                copy.Remove(inventoryItem);
            }
            else
            {
                copy.Remove(inventoryItem);
            }
        }

        foreach (var inCopy in copy)
        {
            RemoveSlot(inCopy);
        }
    }

    void RemoveSlot(InventoryItem inventoryItem)
    {
        var isFound = inventoryItemSlots.ContainsKey(inventoryItem);

        if (isFound)
        {
            Destroy(inventoryItemSlots[inventoryItem]);
            inventoryItemSlots.Remove(inventoryItem);
        }
    }

    void CreateSlot(InventoryItem inventoryItem)
    {
        var instance = Instantiate(prefab, gridContainer);

        inventoryItemSlots[inventoryItem] = instance;
        instance.ApplyInventoryItem(inventoryItem);
    }
}
