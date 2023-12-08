using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject tryAgainText;

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2f);

        tryAgainText.SetActive(true);
    }
}
