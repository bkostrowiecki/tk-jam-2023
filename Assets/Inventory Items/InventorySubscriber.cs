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
    InventoryItemSO selectedPotionSO;
    InventoryItemSO selectedWeaponSO;
    public TMPro.TMP_Text descriptionTmp;
    private InventoryItem currentDescriptionSource;

    void Start()
    {
        playerController.InventoryItemsObservable.Subscribe((inventoryItems) =>
        {
            UpdateSlots(inventoryItems);
        });

        playerController.SelectedPotionSOObservable.Subscribe((itemSO) =>
        {
            if (selectedPotionSO != itemSO)
            {
                var newInventoryItem = FindInventoryItem(itemSO);

                if (newInventoryItem == null)
                {
                    return;
                }

                if (selectedPotionSO != null)
                {
                    var oldInventoryItem = FindInventoryItem(selectedPotionSO);
                    MarkSlotUnselected(oldInventoryItem);
                }

                selectedPotionSO = newInventoryItem.inventoryItemSO;
                MarkSlotSelected(newInventoryItem);
            }
        });

        playerController.SelectedWeaponSOObservable.Subscribe((itemSO) =>
        {
            if (itemSO != selectedWeaponSO)
            {
                var newInventoryItem = FindInventoryItem(itemSO);

                if (newInventoryItem == null)
                {
                    return;
                }

                if (selectedWeaponSO != null)
                {
                    var oldInventoryItem = FindInventoryItem(selectedWeaponSO);
                    MarkSlotUnselected(oldInventoryItem);
                }

                selectedWeaponSO = newInventoryItem.inventoryItemSO;
                MarkSlotSelected(newInventoryItem);
            }
        });
    }

    InventoryItem FindInventoryItem(InventoryItemSO inventoryItemSO)
    {
        foreach (var key in inventoryItemSlots.Keys)
        {
            if (inventoryItemSO == key.inventoryItemSO)
            {
                return key;
            }
        }

        return null;
    }

    void MarkSlotUnselected(InventoryItem inventoryItem)
    {
        var isFound = inventoryItemSlots.ContainsKey(inventoryItem);

        if (isFound)
        {
            var currentInventoryItem = inventoryItemSlots[inventoryItem];

            currentInventoryItem.MarkUnselected();
        }
    }

    void MarkSlotSelected(InventoryItem inventoryItem)
    {
        var isFound = inventoryItemSlots.ContainsKey(inventoryItem);

        if (isFound)
        {
            var currentInventoryItem = inventoryItemSlots[inventoryItem];

            currentInventoryItem.MarkSelected();
        }
    }

    void UpdateSlots(List<InventoryItem> inventoryItems)
    {
        List<InventoryItem> copy = new List<InventoryItem>(inventoryItems.ToArray());

        foreach (var inventoryItem in inventoryItems)
        {
            var found = inventoryItemSlots.ContainsKey(inventoryItem);

            if (!found && inventoryItem != null)
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

        instance.onHover.AddListener(() =>
        {
            currentDescriptionSource = inventoryItem;
            descriptionTmp.text = "<b>" + inventoryItem.inventoryItemSO.itemName + "</b>\n" + inventoryItem.inventoryItemSO.description;
        });

        instance.offHover.AddListener(() =>
        {
            if (currentDescriptionSource == inventoryItem)
            {
                descriptionTmp.text = "...";
            }
        });

        instance.onSelect.AddListener(() =>
        {
            if (inventoryItem.inventoryItemSO.IsWeapon)
            {
                playerController.SetWeapon(inventoryItem.inventoryItemSO);
            }
            else if (inventoryItem.inventoryItemSO.IsPotion)
            {
                playerController.SetPotion(inventoryItem.inventoryItemSO);
            }
        });
    }
}
