using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiedSubscriber : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public void OnPlayerDied()
    {
        gameOverCanvas.SetActive(true);
    }
}
