using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private float _pickUpTime = 2f;
    [SerializeField] private PoolType _poolType;
    
    private bool _isBooked;

    public event Action<Resource> OnPickUpEvent;
    
    public Vector3 Position => transform.position;
    public float PickUpTime => _pickUpTime;
    public bool IsBooked => _isBooked;

    public void Book()
    {
        _isBooked = true;
    }

    public void PickUp()
    {
        _isBooked = false;
        OnPickUpEvent?.Invoke(this);
        ObjectPool.Instance.RemoveToPool(_poolType, gameObject);
    }
}