using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public FloatingText prefab;
    public Color color;
    public int poolInstancesCount = 5;
    List<FloatingText> pooledObjects;

    void Awake()
    {
        pooledObjects = new List<FloatingText>();
        FloatingText tmp;

        for (int i = 0; i < poolInstancesCount; i++)
        {
            tmp = Instantiate(prefab);
            tmp.gameObject.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public FloatingText GetPooledObject(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < poolInstancesCount; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].transform.position = position;
                pooledObjects[i].transform.rotation = rotation;
                pooledObjects[i].gameObject.SetActive(true);
                return pooledObjects[i];
            }
        }

        Debug.LogWarning("Pool used");

        return null;
    }

    public void SpawnText(string text)
    {
        var instance = GetPooledObject(transform.position, Quaternion.identity);
        instance.SetText(text);
        instance.SetColor(color);
    }

    public void SpawnNumber(int number)
    {
        var instance = GetPooledObject(transform.position, Quaternion.identity);
        instance.SetText(number.ToString());
        instance.SetColor(color);
    }

    public void SpawnNegatedNumber(int number)
    {
        SpawnNumber(-number);
    }
}
