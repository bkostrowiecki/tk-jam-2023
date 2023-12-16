using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseIndicator : MonoBehaviour
{
    public PlayerController playerController;

    void Update()
    {
        var direction = playerController.CalculateMouseDirection();
        var lookThere = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0) * Quaternion.Euler(90, 0, 0);

        transform.position = playerController.transform.position + direction * 1.2f;

        transform.rotation = lookThere;
    }
}