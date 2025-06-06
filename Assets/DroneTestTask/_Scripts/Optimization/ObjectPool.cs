using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<PoolItem> _poolItems = new();
    
    private Dictionary<PoolType, Queue<GameObject>> _objectPools = new();
    
    public static ObjectPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePools();
    }

    public GameObject GetFromPool(PoolType pool, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!_objectPools.ContainsKey(pool))
        {
            Debug.LogError($"Pool {pool} not found");
            return null;
        }

        GameObject prefab = null;
        
        if (_objectPools[pool].Count > 0)
            prefab = _objectPools[pool].Dequeue();
        else
        {
            var obj = _poolItems.Find(p => p.Key == pool);
            if (obj.Prefab != null)
                prefab = InstantiateObjectToPool(obj);
        }
        
        if (prefab != null)
        {
            prefab.gameObject.SetActive(true);
            prefab.transform.SetPositionAndRotation(position, rotation);
            prefab.transform.SetParent(parent);
        }
        
        return prefab;
    }
    
    public void RemoveToPool(PoolType key, GameObject prefab)
    {
        prefab.transform.SetParent(transform);
        prefab.gameObject.SetActive(false);
        _objectPools[key].Enqueue(prefab);
    }
    
    private void InitializePools()
    {
        foreach (var item in _poolItems)
        {
            InitializePool(item);
        }
    }

    private void InitializePool(PoolItem item)
    {
        var queue = new Queue<GameObject>();

        for (var i = 0; i < item.Count; i++)
        {
            var obj = InstantiateObjectToPool(item);
            queue.Enqueue(obj);
        }
        
        _objectPools[item.Key] = queue;
    }

    private GameObject InstantiateObjectToPool(PoolItem item)
    {
        var obj = Instantiate(item.Prefab, transform);
        obj.gameObject.SetActive(false);
        return obj;
    }
}

[Serializable]
public struct PoolItem
{
    public PoolType Key;
    public GameObject Prefab;
    public int Count;
}

public enum PoolType
{
    Resource,
}