using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputExtensions : MonoBehaviour
{
    public static bool MouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
