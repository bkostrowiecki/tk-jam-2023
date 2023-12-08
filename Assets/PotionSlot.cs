using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class PotionSlot : MonoBehaviour
{
    public PlayerController playerController;
    private IDisposable potionSODisposable;
    private IDisposable potionAmountDisposable;
    public Image slotImage;
    public TMPro.TMP_Text amountTmp;
    InventoryItemSO cachedInventoryItemSO;

    void Start()
    {
        potionSODisposable = playerController.SelectedPotionSOObservable.Subscribe((inventoryItemSO) =>
        {
            cachedInventoryItemSO = inventoryItemSO;
            if (inventoryItemSO == null)
            {
                slotImage.sprite = null;
                amountTmp.text = "";
                return;
            }
            slotImage.sprite = inventoryItemSO.icon;
        });

        potionAmountDisposable = playerController.SelectedPotionAmountObservable.Subscribe((amount) =>
        {
            if (cachedInventoryItemSO == null)
            {
                amountTmp.text = "";
                return;
            }
            amountTmp.text = amount.ToString();
        });
    }

    void OnDestroy()
    {
        potionSODisposable.Dispose();
        potionAmountDisposable.Dispose();
    }
}
