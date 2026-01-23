using UnityEngine;

public class BallistaMouseRotator : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform _body;

    [Header("Movement Settings")]
    [SerializeField] private float _sensitivity = 50f; 
    [SerializeField] private float _acceleration = 5f; 
    [SerializeField] private float _friction = 3f;
    [SerializeField] private float _maxSpeed = 100f;

    [Header("Limits")]
    [SerializeField] private float _angleLimit = 79f;

    private float _currentVelocityY; 
    private float _currentAngleY;   
    private float _startAngleY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _startAngleY = _body.localEulerAngles.y;
        _currentAngleY = 0;
    }

    private void Update()
    {
        float mouseInput = Input.GetAxis("Mouse X");

        if (Mathf.Abs(mouseInput) > 0.01f)
        {
            _currentVelocityY += mouseInput * _sensitivity * _acceleration * Time.deltaTime;
        }
        else
        {
            _currentVelocityY = Mathf.Lerp(_currentVelocityY, 0, _friction * Time.deltaTime);
        }

        _currentVelocityY = Mathf.Clamp(_currentVelocityY, -_maxSpeed, _maxSpeed);

        _currentAngleY += _currentVelocityY * Time.deltaTime;

        _currentAngleY = Mathf.Clamp(_currentAngleY, -_angleLimit, _angleLimit);

        if (_currentAngleY >= _angleLimit || _currentAngleY <= -_angleLimit)
        {
            _currentVelocityY = 0;
        }

        _body.localRotation = Quaternion.Euler(0, _startAngleY + _currentAngleY, 0);
    }
}