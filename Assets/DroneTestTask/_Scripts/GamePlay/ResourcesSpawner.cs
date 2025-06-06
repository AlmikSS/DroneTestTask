using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField] private PoolType _poolType;
    [SerializeField] private float _xClamp = 15;
    [SerializeField] private float _zClamp = 5;
    
    private List<Resource> _resources = new();
    private float _interval = 1;
    
    public IEnumerable<Resource> Resources => _resources;

    public void Initialize(float interval)
    {
        _interval = interval;
        
        for (var i = 0; i < 10; i++)
        {
            SpawnResource();
        }
        
        StartCoroutine(SpawnerRoutine());
    }

    public void ChangeInterval(float interval)
    {
        _interval = interval;
    }

    public void ResetResourcesState(int _)
    {
        StopAllCoroutines();
        
        var resourcesCopy = _resources.ToList();
        
        foreach (var res in resourcesCopy)
        {
            res.PickUp();
        }
        
        _resources.Clear();
        Initialize(_interval);
    }
    
    private IEnumerator SpawnerRoutine()
    {
        while (true)
        {
            SpawnResource();
            yield return new WaitForSeconds(_interval);
        }
    }
    
    private void SpawnResource()
    {
        var position = GetRandomPosition();
        var newRes = ObjectPool.Instance.GetFromPool(_poolType, position, Quaternion.identity).GetComponent<Resource>();
        _resources.Add(newRes);
        newRes.OnPickUpEvent += OnOnPickUp;
    }

    private void OnOnPickUp(Resource obj)
    {
        if (_resources.Contains(obj))
            _resources.Remove(obj);
    }

    private Vector3 GetRandomPosition()
    {
        var x = Random.Range(-_xClamp, _xClamp);
        var z = Random.Range(-_zClamp, _zClamp);
        return new Vector3(x, 0.5f, z);
    }
}