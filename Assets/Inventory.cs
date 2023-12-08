using System;
using System.Collections.Generic;
using UniRx;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> inventoryItems = new();

    BehaviorSubject<List<InventoryItem>> inventoryItemsSubject;

    public IObservable<List<InventoryItem>> InventoryItemsObservable => inventoryItemsSubject.AsObservable();

    public Inventory()
    {
        inventoryItemsSubject = new BehaviorSubject<List<InventoryItem>>(inventoryItems);
    }

    public void Emit()
    {
        foreach (var inventoryItem in inventoryItems)
        {
            inventoryItem.Emit();
        }
    }

    public void AddInventoryItem(InventoryItemSO inventoryItemSO, int amount)
    {
        var inventoryItem = CreateOrGetInventoryItem(inventoryItemSO, amount);

        inventoryItem.AddAmount(amount);
    }

    private InventoryItem CreateOrGetInventoryItem(InventoryItemSO inventoryItemSO, int amount)
    {
        var inventoryItem = FindInventoryItemBySO(inventoryItemSO);

        if (inventoryItem != null)
        {
            return inventoryItem;
        }
        else
        {
            var newInventoryItem = new InventoryItem();
            newInventoryItem.AddAmount(amount);

            inventoryItems.Add(newInventoryItem);

            inventoryItemsSubject.OnNext(inventoryItems);

            return newInventoryItem;
        }
    }

    public void UseInventoryItem(InventoryItemSO inventoryItemSO, int amount)
    {
        var inventoryItem = CreateOrGetInventoryItem(inventoryItemSO, amount);

        inventoryItem.UseAmount(amount);

        if (inventoryItem.amount == 0)
        {
            inventoryItems.Remove(inventoryItem);
            inventoryItemsSubject.OnNext(inventoryItems);
        }
    }

    public int GetInventoryItemAmount(InventoryItemSO inventoryItemSO)
    {
        var inventoryItem = FindInventoryItemBySO(inventoryItemSO);

        return inventoryItem != null ? inventoryItem.amount : 0;
    }

    public InventoryItem FindInventoryItemBySO(InventoryItemSO inventoryItemSO)
    {
        return inventoryItems.Find((item) => item.inventoryItemSO == inventoryItemSO);
    }
}
