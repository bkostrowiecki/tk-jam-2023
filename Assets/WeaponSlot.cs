using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class WeaponSlot : MonoBehaviour
{
    public PlayerController playerController;
    public Image slotImage;
    public TMPro.TMP_Text bloodTmp;
    public Image sacrificePossibleImage;
    private IDisposable disposableSelectedWeaponObservable;

    void Start()
    {
        disposableSelectedWeaponObservable = playerController.SelectedWeaponObservable.Subscribe((inventoryItem) =>
        {
            if (inventoryItem == null)
            {
                slotImage.gameObject.SetActive(false);
                bloodTmp.text = 0.ToString();

                return;
            }

            bloodTmp.text = inventoryItem.blood.ToString();
            slotImage.sprite = inventoryItem.inventoryItemSO.icon;
            sacrificePossibleImage.gameObject.SetActive(inventoryItem.CanSacrifice);
        });
    }

    void OnDestroy()
    {
        disposableSelectedWeaponObservable.Dispose();
    }
}

