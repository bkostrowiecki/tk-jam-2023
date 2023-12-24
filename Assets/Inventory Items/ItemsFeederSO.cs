using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsFeederSO", menuName = "tk-jam-2023/ItemsFeederSO", order = 0)]
public class ItemsFeederSO : ScriptableObject
{
    public List<InventoryItemSO> inventoryItemSOs = new();

    public InventoryItemSO FindByName(string itemName)
    {
        return inventoryItemSOs.Find((item) => item.itemName == itemName);
    }
}