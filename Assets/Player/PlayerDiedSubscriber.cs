using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiedSubscriber : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public void HandleOnPlayerDied()
    {
        gameOverCanvas.SetActive(true);
    }

    void Start()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().onDied.AddListener(HandleOnPlayerDied);
    }
}
