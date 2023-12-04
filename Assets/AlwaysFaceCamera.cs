using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [EditorCools.Button]
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position);
    }
}
