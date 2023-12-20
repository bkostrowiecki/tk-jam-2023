using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTargetToPNG : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public RenderTexture renderTexture;

    [EditorCools.Button]
    public void Save(Collider collider)
    {
        byte[] bytes = toTexture2D(renderTexture).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Repos/tk-jam-2023/Assets/Inventory Items/Screenshots/" + collider.gameObject.name.Trim() + ".png", bytes);

        Debug.Log("Saved " + collider.gameObject.name.Trim());
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
