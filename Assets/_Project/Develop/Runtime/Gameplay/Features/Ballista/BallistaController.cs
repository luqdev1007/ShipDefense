using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaController : MonoBehaviour
    {
        [field:SerializeField] public Transform ProjectileParent { get; private set; }

        [Header("Parts References")]
        [SerializeField] private Transform _horizontalPivot; 
        [SerializeField] private Transform _verticalPivot; 

        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 50f;
        [SerializeField] private Vector2 _verticalLimits = new Vector2(-20f, 45f);
        [SerializeField] private Vector2 _horizontalLimits = new Vector2(-60f, 60f); 

        private IInputService _inputService;
        private float _currentXRotation;
        private float _currentYRotation;

        public void Init(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Update()
        {
            if (_inputService == null || !_inputService.IsEnabled) 
                return;

            HandleRotation();
        }

        private void HandleRotation()
        {
            Vector2 input = _inputService.MoveDirection;

            _currentYRotation += input.x * _rotationSpeed * Time.deltaTime;
            _currentYRotation = Mathf.Clamp(_currentYRotation, _horizontalLimits.x, _horizontalLimits.y);

            _horizontalPivot.localRotation = Quaternion.Euler(0, _currentYRotation, 0);

            _currentXRotation -= input.y * _rotationSpeed * Time.deltaTime;
            _currentXRotation = Mathf.Clamp(_currentXRotation, _verticalLimits.x, _verticalLimits.y);

            _verticalPivot.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);
        }
    }
}