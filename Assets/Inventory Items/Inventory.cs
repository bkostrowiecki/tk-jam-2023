using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class InventorySaveDto
{
    public List<InventoryItemSaveDto> inventoryItems = new();
    public string selectedWeapon;
    public string selectedPotion;
}

[Serializable]
public class InventoryItemSaveDto
{
    public string itemName;
    public int amount;
    public int blood;

    public static InventoryItemSaveDto CreateFromInventoryItem(InventoryItem inventoryItem)
    {
        return new InventoryItemSaveDto
        {
            itemName = inventoryItem.inventoryItemSO.itemName,
            amount = inventoryItem.amount,
            blood = inventoryItem.blood
        };
    }
}

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

    public InventorySaveDto InventorySaveDto
    {
        get
        {
            var items = new List<InventoryItemSaveDto>();

            foreach (var item in inventoryItems)
            {
                items.Add(InventoryItemSaveDto.CreateFromInventoryItem(item));
            }

            return new InventorySaveDto
            {
                inventoryItems = items
            };
        }
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
            newInventoryItem.inventoryItemSO = inventoryItemSO;

            inventoryItems.Add(newInventoryItem);

            inventoryItemsSubject.OnNext(inventoryItems);

            return newInventoryItem;
        }
    }

    public void UseInventoryItem(InventoryItemSO inventoryItemSO, int amount)
    {
        var inventoryItem = CreateOrGetInventoryItem(inventoryItemSO, amount);

        inventoryItem.UseAmount(amount);

        if (inventoryItem.inventoryItemSO.IsWeapon)
        {
            inventoryItem.blood = 0;
            inventoryItem.bloodSubject.OnNext(0);
        }

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

    public InventoryItem FindInventoryItemBySOWithHighestBlood(InventoryItemSO inventoryItemSO)
    {
        var items = inventoryItems.FindAll((item) => item.inventoryItemSO == inventoryItemSO);

        if (items.Count == 0)
        {
            return null;
        }

        var last = items[0];
        foreach (var item in items)
        {
            if (last.blood < item.blood)
            {
                last = item;
            }
        }

        return last;
    }

    public void TakeFromOtherInventory(Inventory inventory)
    {
        foreach (var item in inventory.inventoryItems)
        {
            AddInventoryItem(item.inventoryItemSO, item.amount);
        }
    }

    public void Clear()
    {
        inventoryItems.Clear();
        inventoryItemsSubject.OnNext(inventoryItems);
        Emit();
    }

    public void ApplySave(InventorySaveDto data, ItemsFeederSO itemsFeederSO)
    {
        Clear();

        foreach (var item in data.inventoryItems)
        {
            var so = itemsFeederSO.FindByName(item.itemName);

            AddInventoryItem(so, item.amount);
        }
    }
}
