using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T prefab;

    private List<T> pooledObjects;
    private int amount;
    private bool isReady;

    public void PoolObjects(int amount = 0)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException("Amount to pool must be non-negative.");

        this.amount = amount;

        pooledObjects = new List<T>(amount);

        GameObject newGameObject;

        for (int i = 0; i < amount; i++)
        {
            newGameObject = Instantiate(prefab.gameObject, transform);
            newGameObject.SetActive(false);
            pooledObjects.Add(newGameObject.GetComponent<T>());
        }

        isReady = true;

    }

    public T GetPooledObject()
    {
        if (!isReady)
            PoolObjects(1);

        for (int i = 0; i < amount; i++)
        {
            if (!pooledObjects[i].isActiveAndEnabled)
                return pooledObjects[i];
        }

        GameObject newObject = Instantiate(prefab.gameObject, transform);
        newObject.SetActive(false);
        pooledObjects.Add(newObject.GetComponent<T>());
        amount++;
        return newObject.GetComponent<T>();
    }

    public void ReturnObjectToPool(T toBeReturned)
    {
        if (toBeReturned == null)
        {
            return;
        }

        if (!isReady)
        {
            PoolObjects();
            pooledObjects.Add(toBeReturned);
        }
        
        toBeReturned.gameObject.SetActive(false);
    }
}
