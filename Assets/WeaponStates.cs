using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponStates : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject statesContainer;
    BaseWeaponState[] states;
    Dictionary<InventoryItemSO, BaseWeaponState> keyedWeaponStates = new();

    public InventoryItemSO activated;
    bool isInitialized;
    void Awake()
    {
        if (isInitialized)
        {
            return;
        }
        states = statesContainer.GetComponentsInChildren<BaseWeaponState>(true);

        foreach (var child in states)
        {
            child.playerController = playerController;
            child.weaponStates = this;
            child.isDebugging = playerController.isDebugging;
            child.gameObject.SetActive(false);

            keyedWeaponStates[child.inventoryItemSO] = child;
        }

        isInitialized = true;
    }
    public void SetCurrentWeapon(InventoryItemSO selectedWeaponSO)
    {
        if (keyedWeaponStates.Keys.Count == 0)
        {
            Awake();
        }

        var cached = activated;

        if (cached != null)
        {
            keyedWeaponStates[cached].gameObject.SetActive(false);
        }

        activated = selectedWeaponSO;

        if (activated != null)
        {
            keyedWeaponStates[activated].gameObject.SetActive(true);
        }
    }
}
