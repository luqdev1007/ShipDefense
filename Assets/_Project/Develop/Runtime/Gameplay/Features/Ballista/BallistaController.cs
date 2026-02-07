using UnityEngine;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaController : MonoBehaviour
    {
        public event Action<float> OnFired;

        [field: SerializeField] public Transform ProjectileParent { get; private set; }

        [SerializeField] private Rigidbody _horizontalPivot;
        [SerializeField] private Rigidbody _verticalPivot;

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

            // Получаем текущие ЛОКАЛЬНЫЕ углы
            Vector3 localH = _horizontalPivot.transform.localEulerAngles;
            Vector3 localV = _verticalPivot.transform.localEulerAngles;

            // Функция для перевода из 0..360 в -180..180
            _currentYRotation = FixAngle(localH.y);
            _currentXRotation = FixAngle(localV.x);
        }

        private float FixAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            return angle;
        }

        private void Update()
        {
            if (_inputService == null || !_inputService.IsEnabled)
                return;

            HandleCharge();
        }

        private void FixedUpdate()
        {
            if (_inputService == null || !_inputService.IsEnabled)
                return;

            HandlePhysicsRotation();
        }

        private void HandlePhysicsRotation()
        {
            Vector2 input = _inputService.MoveDirection;

            // 1. Считаем дельту (изменение), а не абсолютное значение
            _currentYRotation += input.x * _rotationSpeed * Time.fixedDeltaTime;
            _currentYRotation = Mathf.Clamp(_currentYRotation, _horizontalLimits.x, _horizontalLimits.y);

            _currentXRotation += input.y * _rotationSpeed * Time.fixedDeltaTime;
            _currentXRotation = Mathf.Clamp(_currentXRotation, _verticalLimits.x, _verticalLimits.y);

            // 2. ВАЖНО: Используем вращение КОРНЯ (самой баллисты), а не пивотов друг друга
            // Это предотвращает накопление ошибок поворота
            Quaternion rootRotation = transform.rotation;

            // Горизонталь: поворот корня + наш Y
            Quaternion horizTarget = rootRotation * Quaternion.Euler(0, _currentYRotation, 0);
            _horizontalPivot.MoveRotation(horizTarget);

            // Вертикаль: поворот корня + наш Y + наш X
            // Мы складываем их последовательно относительно корня
            Quaternion vertTarget = rootRotation * Quaternion.Euler(_currentXRotation, _currentYRotation, 0);
            _verticalPivot.MoveRotation(vertTarget);
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