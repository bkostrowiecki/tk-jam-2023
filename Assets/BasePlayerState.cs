using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerState : MonoBehaviour
{
    [HideInInspector]
    public bool isDebugging;

    [HideInInspector]
    public PlayerController playerController;
}
