using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public Transform inventoryCanvas;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryCanvas.gameObject.SetActive(!inventoryCanvas.gameObject.activeInHierarchy);
        }
    }
}
