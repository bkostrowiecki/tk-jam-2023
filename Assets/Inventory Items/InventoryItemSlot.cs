using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class InventoryItemSlot : MonoBehaviour
{
    public TMPro.TMP_Text amountTmp;
    public Slider bloodSlider;
    public Image iconImage;
    public Image selectionImage;
    public UnityEvent onHover = new();
    public UnityEvent offHover = new();
    public UnityEvent onSelect = new();
    public Color selectedColor;
    public Color neutralColor;

    public void ApplyInventoryItem(InventoryItem inventoryItem)
    {
        if (inventoryItem.inventoryItemSO.IsWeapon)
        {
            bloodSlider.gameObject.SetActive(true);
            bloodSlider.minValue = 0;
            bloodSlider.wholeNumbers = true;
            bloodSlider.maxValue = inventoryItem.inventoryItemSO.requiredBlood;
        }

        iconImage.sprite = inventoryItem.inventoryItemSO.icon;

        inventoryItem.AmountObservable.Subscribe((amount) =>
        {
            amountTmp.text = amount.ToString();
        });

        inventoryItem.BloodObservable.Subscribe((blood) =>
        {
            bloodSlider.value = blood;
        });
    }

    public void HandleOnClick()
    {
        onSelect?.Invoke();
    }

    public void HandleOnHover()
    {
        onHover?.Invoke();
    }

    public void HandleOffHover()
    {
        offHover?.Invoke();
    }

    public void MarkUnselected()
    {
        selectionImage.gameObject.SetActive(false);
    }

    public void MarkSelected()
    {
        selectionImage.gameObject.SetActive(true);
    }
}
