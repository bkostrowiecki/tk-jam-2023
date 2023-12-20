using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public List<InventoryItem> inventoryItems;
    bool isOpened;

    public bool IsOpened
    {
        get
        {
            return isOpened;
        }
    }

    public void Open()
    {
        isOpened = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public List<InventoryItem> InventoryItems
    {
        get
        {
            if (isOpened)
            {
                return new List<InventoryItem>();
            }
            return inventoryItems;
        }
    }
}
