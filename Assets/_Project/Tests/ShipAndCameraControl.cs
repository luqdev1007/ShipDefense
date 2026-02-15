using UnityEngine;
using UnityEngine.AI;
using Assets._Project.Develop.Runtime.Utilites.RaycastManagment;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class ShipAndCameraControl : MonoBehaviour
{
    [Header("Ship Settings")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _waterMask;
    [SerializeField] private float _lineYOffset = 0.5f;
    [SerializeField] private float _navMeshSampleRadius = 5f;

    [Header("Camera Settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _dragSensitivity = 1.5f;

    private SurfaceRaycaster _raycaster = new SurfaceRaycaster();
    private LineRenderer _lineRenderer;
    private NavMeshPath _previewPath; // Путь для предпросмотра

    private Vector3 _pendingTarget;
    private bool _hasPendingPath = false;
    private Vector3 _lastMousePosition;
    private float _fixedCameraY;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        _previewPath = new NavMeshPath();

        if (_camera == null) _camera = Camera.main;
        _fixedCameraY = _camera.transform.position.y;

        // Принудительная настройка LineRenderer
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 0;
    }

    void Update()
    {
        HandleShipInput();
        HandleCameraMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteMove();
        }

        UpdatePathVisualization();
    }

    private void HandleShipInput()
    {
        // ПКМ — Выбор цели и моментальный расчет "умного" пути
        if (Input.GetMouseButtonDown(1))
        {
            if (_raycaster.TryGetHitInfo(_camera, _waterMask, out RaycastHit hit))
            {
                // Проверяем ближайшую точку на NavMesh, чтобы путь строился по сетке
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, _navMeshSampleRadius, NavMesh.AllAreas))
                {
                    _pendingTarget = navHit.position;

                    // Сразу рассчитываем путь, который пойдет через AI
                    if (_agent.CalculatePath(_pendingTarget, _previewPath))
                    {
                        _hasPendingPath = true;
                        Debug.Log("Маршрут построен. Нажмите Space для движения.");
                    }
                }
            }
        }
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
        Vector3[] corners;

        // 1. Если корабль уже в движении — берем его актуальный путь
        if (_agent.hasPath)
        {
            corners = _agent.path.corners;
        }
        // 2. Если мы только наметили цель — берем предрассчитанный путь
        else if (_hasPendingPath && _previewPath.status == NavMeshPathStatus.PathComplete)
        {
            corners = _previewPath.corners;
        }
        else
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        // Отрисовка линии по точкам (corners)
        _lineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 point = corners[i];
            point.y = _lineYOffset; // Фиксируем высоту над водой
            _lineRenderer.SetPosition(i, point);
        }
    }

    private void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(0)) _lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;

            // Движение по X и Z без изменения Y
            Vector3 move = new Vector3(-delta.x * _dragSensitivity * 0.01f, 0, -delta.y * _dragSensitivity * 0.01f);

            _camera.transform.position += move;
            _camera.transform.position = new Vector3(_camera.transform.position.x, _fixedCameraY, _camera.transform.position.z);

            _lastMousePosition = Input.mousePosition;
        }
    }
}