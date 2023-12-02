using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    public GameObject statesContainer;

    void Awake()
    {
        var children = statesContainer.GetComponentsInChildren<BasePlayerState>();

        foreach (var child in children)
        {
            child.playerController = this;
        }
    }
}
