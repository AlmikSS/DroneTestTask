using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Team _team;
    [SerializeField] private GameObject _vfxPrefab;
    [SerializeField] private Drone _dronePrefab;
    
    private List<Drone> _drones = new();
    private ResourcesSpawner _resourcesSpawner;
    private float _droneSpeed;
    private int _resCount;
    
    public Vector3 Position => transform.position;
    public int ResCount => _resCount;

    public void Initialize(ResourcesSpawner resourcesSpawner, float droneSpeed, int droneCount = 3)
    {
        _resourcesSpawner = resourcesSpawner;
        _droneSpeed = droneSpeed;
        
        StartCoroutine(SpawnDroneRoutine(droneCount));
    }
    
    public void GiveRes(Drone drone)
    {
        if (drone.Team == _team)
        {
            _resCount++;
            var vfx = Instantiate(_vfxPrefab, Position, Quaternion.identity, transform);
            Destroy(vfx, 2f);
            drone.StartGameCycle();
        }
    }

    public void ChangeDroneCount(int value)
    {
        foreach (var drone in _drones)
        {
            Destroy(drone.gameObject);
        }
        
        _drones.Clear();
        StartCoroutine(SpawnDroneRoutine(value));
    }

    private IEnumerator SpawnDroneRoutine(int value)
    {
        for (var i = 0; i < value; i++)
        {
            var drone = Instantiate(_dronePrefab, transform.position, Quaternion.identity);
            drone.Initialize(_resourcesSpawner, this, _team);
            drone.SetSpeed(_droneSpeed);
            drone.StartGameCycle();
            _drones.Add(drone);
            yield return null;
        }
    }
    
    public void ChangeDroneSpeed(float value)
    {
        _droneSpeed = value;

        foreach (var drone in _drones)
        {
            drone.SetSpeed(_droneSpeed);
        }
    }

    public void ChangeShowPath(bool value)
    {
        foreach (var drone in _drones)
        {
            drone.AgentPathRenderer.ChangeShowPath(value);
        }
    }
}

public enum Team
{
    Red,
    Blue,
}