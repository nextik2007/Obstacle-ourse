using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CarController))]
public class EngineSound : MonoBehaviour
{
    private AudioSource _engineSound;
    private CarController _carController;

    [SerializeField] private float _speedRatio;
    [SerializeField] private int _maxGear;

    private float _minSpeed = 0;
    private float _maxSpeed = 0;
    private float _currentGear = 0;

    private void Start()
    {
        _engineSound = GetComponent<AudioSource>();
        _carController = GetComponent<CarController>();

        if (_speedRatio <= 0)
            _speedRatio = 4;

        if (_maxGear < 1)
            _maxGear = 5;
    }

    private void Update()
    {
        SwitchGear();
    }

    private void SwitchGear()
    {
        if (_carController.Speed >= _maxSpeed)
        {
            if (_currentGear < _maxGear)
            {
                _currentGear++;
                _minSpeed = _maxSpeed;

                _maxSpeed += GetMaxSpeedValue();
            }
        }
        else if (_carController.Speed < _minSpeed)
        {
            _maxSpeed -= GetMaxSpeedValue();
            _currentGear--;

            _minSpeed -= GetMaxSpeedValue();
        }

        _engineSound.pitch = 1 + _carController.Speed / _maxSpeed;
    }

    private float GetMaxSpeedValue() => _speedRatio * _currentGear;
}