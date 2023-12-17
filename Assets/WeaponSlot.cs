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
    public Slider bloodSlider;
    public Image sacrificePossibleImage;
    private IDisposable disposableSelectedWeaponObservable;
    private IDisposable disposableBloodObservable;

    void Start()
    {
        slotImage.gameObject.SetActive(true);

        disposableSelectedWeaponObservable = playerController.SelectedWeaponObservable.Subscribe((inventoryItem) =>
        {
            if (inventoryItem == null)
            {
                slotImage.gameObject.SetActive(false);
                bloodSlider.maxValue = 1;
                bloodSlider.value = 0;

                ToggleSacrificePossible(false);

                return;
            }

            disposableBloodObservable = inventoryItem.bloodSubject.Subscribe((x) =>
            {
                bloodSlider.maxValue = inventoryItem.inventoryItemSO.requiredBlood;
                bloodSlider.value = x;

                ToggleSacrificePossible(inventoryItem.CanSacrifice);
            });

            slotImage.sprite = inventoryItem.inventoryItemSO.icon;
            ToggleSacrificePossible(inventoryItem.CanSacrifice);
        });
    }

    void ToggleSacrificePossible(bool isPossible)
    {
        sacrificePossibleImage.gameObject.SetActive(isPossible);
    }

    void OnDestroy()
    {
        disposableBloodObservable.Dispose();
        disposableSelectedWeaponObservable.Dispose();
    }
}

