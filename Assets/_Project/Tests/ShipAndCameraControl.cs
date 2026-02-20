using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class ShipAndCameraControl : MonoBehaviour
{
    [Header("Ship Settings")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _waterMask;
    [SerializeField] private float _lineYOffset = 0.2f; // Чуть выше воды
    [SerializeField] private float _navMeshSampleRadius = 10f;

    [Header("Camera Settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _dragSensitivity = 1.5f;

    private LineRenderer _lineRenderer;
    private NavMeshPath _previewPath;
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

        // Настройка LineRenderer для пунктирной линии
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.alignment = LineAlignment.View;
        _lineRenderer.textureMode = LineTextureMode.Tile; // Важно для пунктира
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
        // ЛКМ — Выбор цели в стиле Героев 5
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _waterMask))
            {
                // Ищем ближайшую точку на NavMesh
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, _navMeshSampleRadius, NavMesh.AllAreas))
                {
                    _pendingTarget = navHit.position;

                    // Рассчитываем путь от текущего положения корабля
                    _agent.CalculatePath(_pendingTarget, _previewPath);

                    if (_previewPath.status == NavMeshPathStatus.PathComplete)
                    {
                        _hasPendingPath = true;
                        Debug.Log("Путь намечен. Нажми Space для движения.");
                    }
                    else
                    {
                        _hasPendingPath = false;
                        Debug.LogWarning("Сюда не проплыть!");
                    }
                }
            }
        }
    }

    private void ExecuteMove()
    {
        if (_hasPendingPath)
        {
            // Устанавливаем цель. Агент сам начнет движение.
            _agent.SetDestination(_pendingTarget);
            _hasPendingPath = false; // Сбрасываем предпросмотр
        }
    }

    private void UpdatePathVisualization()
    {
        Vector3[] corners;

        // Если корабль ПЛЫВЕТ, рисуем путь до конца маршрута
        if (_agent.hasPath)
        {
            corners = _agent.path.corners;
        }
        // Если корабль СТОИТ, но мы ТКНУЛИ мышкой — рисуем предпросмотр
        else if (_hasPendingPath)
        {
            corners = _previewPath.corners;
        }
        else
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        // Рисуем линию
        _lineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            // Поднимаем каждую точку пути над водой, чтобы линия не "тонула" в мешах
            Vector3 point = corners[i];
            point.y += _lineYOffset;
            _lineRenderer.SetPosition(i, point);
        }
    }

    private void HandleCameraMovement()
    {
        // Камера на СКМ (среднюю кнопку) или зажать Alt+ЛКМ, 
        // чтобы не конфликтовать с выбором пути на ЛКМ.
        // Но оставим твою логику на ЛКМ, добавив проверку на "движение или клик"
        if (Input.GetMouseButtonDown(2)) _lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;
            Vector3 move = new Vector3(-delta.x * _dragSensitivity * 0.05f, 0, -delta.y * _dragSensitivity * 0.05f);

            _camera.transform.position += move;
            _camera.transform.position = new Vector3(_camera.transform.position.x, _fixedCameraY, _camera.transform.position.z);
            _lastMousePosition = Input.mousePosition;
        }
    }
}
