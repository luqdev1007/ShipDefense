using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShipAndCameraControl : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Ship Settings")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _pathStartPoint;
    [SerializeField] private LayerMask _waterMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _lineYOffset = 0.2f;
    [SerializeField] private float _navMeshSampleRadius = 10f;

    [Header("HoMM Style Path (Tiling)")]
    [SerializeField] private float _anchorSize = 1.0f;
    [SerializeField] private float _animationSpeed = 2.0f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _blockedColor = new Color(0.81f, 0f, 0.12f);

    [Header("Camera Settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _dragSensitivity = 1.5f;
    [SerializeField] private float _zoomSpeed = 10f;      // Скорость зума
    [SerializeField] private float _minHeight = 5f;       // Минимальная высота (приближение)
    [SerializeField] private float _maxHeight = 40f;      // Максимальная высота (отдаление)

    private NavMeshPath _previewPath;
    private Vector3 _pendingTarget;
    private bool _hasPendingPath = false;
    private bool _isPathBlocked = false;
    private Vector3 _lastMousePosition;
    private float _currentCameraY; // Теперь Y переменная для зума

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _previewPath = new NavMeshPath();

        if (_camera == null) _camera = Camera.main;
        _currentCameraY = _camera.transform.position.y;

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.alignment = LineAlignment.View;
        _lineRenderer.textureMode = LineTextureMode.Tile;

        if (_pathStartPoint == null) _pathStartPoint = transform;
    }

    void Update()
    {
        HandleShipInput();
        HandleCameraMovement();
        HandleCameraZoom(); // Новый метод

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteMove();
        }

        UpdatePathVisualization();
    }

    // ЛОГИКА ЗУМА
    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Вычисляем новую высоту. Минус, чтобы скролл вперед приближал.
            _currentCameraY -= scroll * _zoomSpeed * 10f * Time.deltaTime;

            // Ограничиваем высоту пределами
            _currentCameraY = Mathf.Clamp(_currentCameraY, _minHeight, _maxHeight);

            // Применяем высоту к камере
            Vector3 pos = _camera.transform.position;
            pos.y = _currentCameraY;
            _camera.transform.position = pos;
        }
    }

    private void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(2)) _lastMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;
            // Чувствительность теперь немного зависит от высоты, чтобы на большой высоте камера не "ползла"
            float factor = _currentCameraY * 0.01f * _dragSensitivity;
            Vector3 move = new Vector3(-delta.x * factor, 0, -delta.y * factor);

            _camera.transform.position += move;

            // Фиксируем высоту после перемещения (чтобы не уплыла по Y)
            Vector3 pos = _camera.transform.position;
            pos.y = _currentCameraY;
            _camera.transform.position = pos;

            _lastMousePosition = Input.mousePosition;
        }
    }

    // --- Остальные методы без изменений (HandleShipInput, CheckIfPathBlocked, ExecuteMove, UpdatePathVisualization) ---

    private void HandleShipInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _waterMask))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, _navMeshSampleRadius, NavMesh.AllAreas))
                {
                    _pendingTarget = navHit.position;
                    _agent.CalculatePath(_pendingTarget, _previewPath);

                    if (_previewPath.status == NavMeshPathStatus.PathComplete)
                    {
                        _hasPendingPath = true;
                        _isPathBlocked = CheckIfPathBlocked(_previewPath.corners);
                    }
                    else
                    {
                        _hasPendingPath = false;
                    }
                }
            }
        }
    }

    private bool CheckIfPathBlocked(Vector3[] corners)
    {
        if (corners == null || corners.Length < 2) return false;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = (i == 0) ? _pathStartPoint.position : corners[i];
            Vector3 end = corners[i + 1];
            if (Physics.Linecast(start + Vector3.up * _lineYOffset, end + Vector3.up * _lineYOffset, _obstacleMask))
                return true;
        }
        return false;
    }

    private void ExecuteMove()
    {
        if (_hasPendingPath)
        {
            _agent.SetDestination(_pendingTarget);
            _hasPendingPath = false;
        }
    }

    private void UpdatePathVisualization()
    {
        Vector3[] corners = null;
        bool currentlyBlocked = false;

        if (_agent.hasPath)
        {
            corners = _agent.path.corners;
            currentlyBlocked = CheckIfPathBlocked(corners);
        }
        else if (_hasPendingPath)
        {
            corners = _previewPath.corners;
            currentlyBlocked = _isPathBlocked;
        }
        else
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        if (corners == null || corners.Length == 0) return;

        corners[0] = _pathStartPoint.position;
        _lineRenderer.positionCount = corners.Length;
        float totalDistance = 0f;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 point = corners[i];
            if (i > 0) point.y += _lineYOffset;
            _lineRenderer.SetPosition(i, point);
            if (i > 0) totalDistance += Vector3.Distance(corners[i - 1], corners[i]);
        }

        float tilingValue = totalDistance / _anchorSize;
        _lineRenderer.material.mainTextureScale = new Vector2(tilingValue, 1);
        _lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * _animationSpeed, 0);

        Color finalColor = currentlyBlocked ? _blockedColor : _normalColor;
        _lineRenderer.startColor = finalColor;
        _lineRenderer.endColor = finalColor;
    }
}