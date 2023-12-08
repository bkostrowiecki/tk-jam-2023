using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnFire : MonoBehaviour
{
    public float timeToRestart = 2f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(RestartAsync());
        }
    }

    IEnumerator RestartAsync()
    {
        yield return new WaitForSecondsRealtime(timeToRestart);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
