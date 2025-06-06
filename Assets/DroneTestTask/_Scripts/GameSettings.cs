using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField _intervalInputField;
    [SerializeField] private Slider _droneSpeedSlider;
    [SerializeField] private Slider _droneCountSlider;
    
    private float _interval;
    private float _droneSpeed;
    private int _droneCount;
    private bool _showPath;

    public event Action<float> OnIntervalChangedEvent;
    public event Action<float> OnDroneSpeedChangedEvent;
    public event Action<int> OnDroneCountChangedEvent; 
    public event Action<bool> OnShowPathChangedEvent; 
    
    public float Interval => _interval;
    public float DroneSpeed => _droneSpeed;
    public int DroneCount => _droneCount;

    public void Initialize()
    {
        _interval = 3;
        _droneSpeed = _droneSpeedSlider.value;
        _droneCount = (int)_droneCountSlider.value;
    }
    
    public void ShowDronePathBtn()
    {
        _showPath = !_showPath;
        OnShowPathChangedEvent?.Invoke(_showPath);
    }

    public void ChangeInterval()
    {
        if (!float.TryParse(_intervalInputField.text, out _interval))
            _interval = 3f;
        
        OnIntervalChangedEvent?.Invoke(_interval);
    }
    
    public void ChangeDroneSpeed()
    {
        _droneSpeed = _droneSpeedSlider.value;
        OnDroneSpeedChangedEvent?.Invoke(_droneSpeed);
    }

    public void ChangeDroneCount()
    {
        _droneCount = (int)_droneCountSlider.value;
        OnDroneCountChangedEvent?.Invoke(_droneCount);
    }
}