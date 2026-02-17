using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyRotationSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _direction;
        private ReactiveVariable<float> _rotationSpeed;
        private Rigidbody _rigidbody;

        private ICompositeCondition _canRotate;

        public void OnInit(Entity entity)
        {
            _direction = entity.RotationDirection;
            _rigidbody = entity.Rigidbody;
            _rotationSpeed = entity.RotationSpeed;

            _canRotate = entity.CanRotate;

            if (_direction.Value != Vector3.zero)
                _rigidbody.transform.rotation = Quaternion.LookRotation(_direction.Value.normalized);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canRotate.Evaluate() == false)
                return;

            Vector3 direction = _direction.Value;

            direction.y = 0;

            if (direction == Vector3.zero)
                return;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            float step = _rotationSpeed.Value * deltaTime;
            Quaternion rotation = Quaternion.RotateTowards(_rigidbody.rotation, lookRotation, step);

            _rigidbody.MoveRotation(rotation);
        }
    }
}
