using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private Rigidbody _rigidbody;

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            float currentYVelocity = _rigidbody.linearVelocity.y;

            Vector3 baseVelocity = _moveDirection.Value.normalized * _moveSpeed.Value;

            // ВАРИАНТ А: Если мы хотим, чтобы MoveSpeed задавал только "толчок вперед"
            // а гравитация была единственной силой по Y:
            // Vector3 horizontalMove = new Vector3(baseVelocity.x, 0, baseVelocity.z);
            // _rigidbody.linearVelocity = horizontalMove + Vector3.up * currentYVelocity;

            // ВАРИАНТ Б (Твой случай): Стрела летит туда, куда смотрит баллиста
            _rigidbody.linearVelocity = baseVelocity + Vector3.up * (currentYVelocity - baseVelocity.y);
        }
    }
}
