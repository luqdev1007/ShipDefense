using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.RaycastManagment;

public class ShipAndCameraControl : MonoBehaviour
{
    [Header("Ship Settings")]
    [SerializeField] private Transform _shipTransform;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 300f;
    [SerializeField] private LayerMask _waterMask;

    [Header("Camera Settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _dragSensitivity = 1.5f; // Чуть увеличил для отзывчивости

    private SurfaceRaycaster _raycaster = new SurfaceRaycaster();
    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private Vector3 _lastMousePosition;
    private float _fixedShipY;
    private float _fixedCameraY;

    void Start()
    {
        if (_shipTransform != null)
        {
            _fixedShipY = _shipTransform.position.y;
            _targetPosition = _shipTransform.position;
        }

        if (_camera != null)
        {
            _fixedCameraY = _camera.transform.position.y;
        }
    }

    void Update()
    {
        HandleShipInput();
        HandleCameraMovement();
        MoveShip();
    }

    private void HandleShipInput()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            if (_raycaster.TryGetHitInfo(_camera, _waterMask, out RaycastHit hit))
            {
                // Сразу фиксируем Y точки назначения на уровне корабля
                _targetPosition = new Vector3(hit.point.x, _fixedShipY, hit.point.z);
                _isMoving = true;
            }
        }
    }

    private void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;

            // Вычисляем смещение. Используем коэффициент относительно экрана, 
            // чтобы скорость не зависела от FPS (Time.deltaTime здесь не нужен, если мы считаем дельту мыши)
            float moveX = -delta.x * _dragSensitivity * 0.01f;
            float moveZ = -delta.y * _dragSensitivity * 0.01f;

            Vector3 newPos = _camera.transform.position + new Vector3(moveX, 0, moveZ);

            // Жесткая фиксация Y камеры
            newPos.y = _fixedCameraY;
            _camera.transform.position = newPos;

            _lastMousePosition = Input.mousePosition;
        }
    }

    private void MoveShip()
    {
        if (!_isMoving || _shipTransform == null) return;

        Vector3 currentPos = _shipTransform.position;

        // 1. Поворот (смотрим только в плоскости XZ)
        Vector3 direction = (_targetPosition - currentPos).normalized;
        direction.y = 0; // На всякий случай обнуляем

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _shipTransform.rotation = Quaternion.RotateTowards(_shipTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        // 2. Движение с фиксацией Y
        Vector3 nextPos = Vector3.MoveTowards(currentPos, _targetPosition, _moveSpeed * Time.deltaTime);
        nextPos.y = _fixedShipY; // Игнорируем любые изменения высоты
        _shipTransform.position = nextPos;

        // Остановка
        if (Vector3.Distance(new Vector3(currentPos.x, 0, currentPos.z),
                             new Vector3(_targetPosition.x, 0, _targetPosition.z)) < 0.05f)
        {
            _isMoving = false;
        }
    }
}