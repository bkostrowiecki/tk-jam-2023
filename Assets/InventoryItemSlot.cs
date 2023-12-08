using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    public TMPro.TMP_Text amountTmp;
    public Slider bloodSlider;
    public Image iconImage;

    public void ApplyInventoryItem(InventoryItem inventoryItem)
    {
        if (inventoryItem.inventoryItemSO.IsWeapon)
        {
            bloodSlider.gameObject.SetActive(true);
            bloodSlider.minValue = 0;
            bloodSlider.wholeNumbers = true;
            bloodSlider.maxValue = inventoryItem.inventoryItemSO.requiredBlood;
        }
        else if (inventoryItem.inventoryItemSO.IsPotion)
        {
            amountTmp.gameObject.SetActive(true);
        }

        iconImage.sprite = inventoryItem.inventoryItemSO.icon;

        inventoryItem.AmountObservable.Subscribe((amount) =>
        {
            if (inventoryItem.inventoryItemSO.IsWeapon)
            {
                return;
            }

            amountTmp.text = amount.ToString();
        });

        inventoryItem.BloodObservable.Subscribe((blood) =>
        {
            bloodSlider.value = blood;
        });
    }
}
