using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDebugger : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Time.timeScale = 2;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Time.timeScale = 4;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Time.timeScale = 8;
        }
    }
}
