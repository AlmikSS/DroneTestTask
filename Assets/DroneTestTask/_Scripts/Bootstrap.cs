using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<Base> _bases = new();
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private GameSettings _gameSettings;
    
    private void Start()
    {
        _gameSettings.Initialize();
        _resourcesSpawner.Initialize(_gameSettings.Interval);

        foreach (var @base in _bases)
        {
            @base.Initialize(_resourcesSpawner, _gameSettings.DroneSpeed, _gameSettings.DroneCount);
            _gameSettings.OnDroneSpeedChangedEvent += @base.ChangeDroneSpeed;
            _gameSettings.OnDroneCountChangedEvent += @base.ChangeDroneCount;
            _gameSettings.OnShowPathChangedEvent += @base.ChangeShowPath;
        }
        
        _gameSettings.OnIntervalChangedEvent += _resourcesSpawner.ChangeInterval;
        _gameSettings.OnDroneCountChangedEvent += _resourcesSpawner.ResetResourcesState;
    }

    private void OnDestroy()
    {
        _gameSettings.OnDroneSpeedChangedEvent -= _resourcesSpawner.ChangeInterval;
        _gameSettings.OnDroneCountChangedEvent -= _resourcesSpawner.ResetResourcesState;
        
        foreach (var @base in _bases)
        {
            _gameSettings.OnDroneSpeedChangedEvent -= @base.ChangeDroneSpeed;
            _gameSettings.OnDroneCountChangedEvent -= @base.ChangeDroneCount;
            _gameSettings.OnShowPathChangedEvent -= @base.ChangeShowPath;
        }
    }
}