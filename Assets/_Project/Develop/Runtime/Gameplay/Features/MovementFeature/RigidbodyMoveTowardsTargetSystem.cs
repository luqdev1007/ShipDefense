using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMoveTowardsTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ICompositeCondition _canMove;
        private Rigidbody _rigidbody; 

        private ITargetSelector _targetSelector;
        private EntitiesLifeContext _entitiesLifeContext;
        private ReactiveVariable<Entity> _target;

        public RigidbodyMoveTowardsTargetSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _target = entity.CurrentTarget;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _canMove = entity.CanMove;
            _rigidbody = entity.Rigidbody;

            _targetSelector = new NearestDamagableTargetSelector(entity);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_target.Value == null)
            {
                _target.Value = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
                _moveDirection.Value = Vector3.zero;
                return;
            }

            if (_rigidbody == null || _canMove.Evaluate() == false)
                return;

            Vector3 currentPos = _rigidbody.position;
            Vector3 targetPos = _target.Value.Transform.position;

            Vector3 nextStep = Vector3.MoveTowards(
                currentPos,
                targetPos,
                _moveSpeed.Value * deltaTime
            );

            nextStep.y = currentPos.y;

            _rigidbody.MovePosition(nextStep);

            Vector3 directionToTarget = (targetPos - currentPos).normalized;
            directionToTarget.y = 0;
            _moveDirection.Value = directionToTarget;
        }
    }
}