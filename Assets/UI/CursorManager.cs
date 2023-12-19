using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Image hotSpotDebug;
    public Vector2 hotspot;

    [EditorCools.Button]
    void Start()
    {
        var cursorHotspot = hotspot;
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        hotSpotDebug.transform.position = Input.mousePosition + new Vector3(hotspot.x, -hotspot.y);
    }
}
