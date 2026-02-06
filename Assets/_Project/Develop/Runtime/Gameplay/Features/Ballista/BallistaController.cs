using UnityEngine;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaController : MonoBehaviour
    {
        public event Action<float> OnFired;

        [field: SerializeField] public Transform ProjectileParent { get; private set; }

        [SerializeField] private Transform _horizontalPivot;
        [SerializeField] private Transform _verticalPivot;

        [SerializeField] private float _rotationSpeed = 50f;
        [SerializeField] private Vector2 _verticalLimits = new Vector2(-20f, 45f);
        [SerializeField] private Vector2 _horizontalLimits = new Vector2(-60f, 60f);

        private IInputService _inputService;
        private float _currentXRotation;
        private float _currentYRotation;

        public bool IsCharging { get; private set; }
        public float ChargeProgress { get; private set; }

        public void Init(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Update()
        {
            if (_inputService == null || !_inputService.IsEnabled)
                return;

            HandleRotation();
            HandleCharge();
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

        private void HandleCharge()
        {
            IsCharging = _inputService.IsAttackKeyHold;

            if (IsCharging)
            {
                ChargeProgress = Mathf.Clamp01(ChargeProgress + Time.deltaTime / 2f);
            }

            if (_inputService.IsAttackKeyReleased)
            {
                OnFired?.Invoke(ChargeProgress);
                ChargeProgress = 0;
            }
        }
    }
}