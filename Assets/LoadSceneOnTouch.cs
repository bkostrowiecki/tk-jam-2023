using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTouch : MonoBehaviour
{
    public string sceneName;
    BoxCollider boxCollider;
    private bool shouldCheck;
    public LayerMask playerLayerMask;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        shouldCheck = true;
    }

    void Update()
    {
        if (!shouldCheck)
        {
            return;
        }

        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size / 1.9f;

        center.Scale(transform.localScale);
        size.Scale(transform.localScale);

        if (Physics.CheckBox(transform.position + center, size, boxCollider.transform.rotation, playerLayerMask))
        {
            SceneManager.LoadScene(sceneName);

            shouldCheck = false;
        }
    }
}
