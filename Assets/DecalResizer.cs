using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalResizer : MonoBehaviour
{
    public Vector2 sizeRange;
    public DecalProjector decalProjector;
    
    void OnEnable()
    {
        decalProjector.size = new Vector3(Random.Range(sizeRange.x, sizeRange.y), Random.Range(sizeRange.x, sizeRange.y), 8f);
    }
}
