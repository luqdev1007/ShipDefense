using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class RagdollExplosionableMono : SimpleExplosionableMono
    {
        [Header("Ragdoll Settings")]
        [SerializeField] private Rigidbody[] _ragdollRigidbodies;
        [SerializeField] private Animator _animator;

        [Tooltip("Главная кость (обычно Pelvis/Hips). Если не задана, возьмет основной Rigidbody")]
        [SerializeField] private Rigidbody _mainBone;

        private void Awake()
        {
            if (_animator == null) TryGetComponent(out _animator);
            if (_ragdollRigidbodies == null || _ragdollRigidbodies.Length == 0)
                _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            // Если главная кость не назначена, пытаемся найти типичный Hips или берем первый из списка
            if (_mainBone == null && _ragdollRigidbodies.Length > 0) _mainBone = _ragdollRigidbodies[0];

            ToggleRagdoll(false);
        }

        public override void Explode(float power, Vector3 sourcePosition, IExplosion source)
        {
            // 1. Активируем регдолл
            ToggleRagdoll(true);

            // 2. ОТКЛЮЧАЕМ главный коллайдер и физику родителя
            // Чтобы они не "якорили" персонажа и не конфликтовали с костями
            if (TryGetComponent(out Collider parentCollider)) parentCollider.enabled = false;
            Rigidbody.isKinematic = true;

            // 3. Толкаем КАЖДУЮ кость (или только таз, но лучше каждую по чуть-чуть)
            // Используем VelocityChange, чтобы забить на массу 50
            foreach (var rb in _ragdollRigidbodies)
            {
                Vector3 dir = rb.position - sourcePosition;
                Vector3 lateralDir = new Vector3(dir.x, 0, dir.z).normalized;

                // Смешиваем направление: вбок + акцентированно ВВЕРХ
                Vector3 explosionVector = (lateralDir * OutwardForceMultiplier) + (Vector3.up * UpwardForceMultiplier);

                // Применяем VelocityChange — это гарантированный подлет
                rb.AddForce(explosionVector.normalized * power, ForceMode.VelocityChange);

                // Рандомная закрутка для каждой кости
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.VelocityChange);
            }
        }

        private void ToggleRagdoll(bool isActive)
        {
            if (_animator != null) _animator.enabled = !isActive;

            foreach (var rb in _ragdollRigidbodies)
            {
                rb.isKinematic = !isActive;
                if (isActive)
                {
                    rb.useGravity = true;
                    // Чтобы конечности болтались сочно, убери у них Drag в инспекторе (поставь 0.01)
                }
            }
        }
    }
}