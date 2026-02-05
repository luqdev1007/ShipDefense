using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class TransformMoveTowardsTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ICompositeCondition _canMove;
        private Transform _transform;

        private ITargetSelector _targetSelector;
        private EntitiesLifeContext _entitiesLifeContext;
        private ReactiveVariable<Entity> _target;

        public TransformMoveTowardsTargetSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _target = entity.CurrentTarget;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _canMove = entity.CanMove;
            _transform = entity.Transform;

            _targetSelector = new NearestDamagableTargetSelector(entity);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_target.Value == null)
            {
                _target.Value = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
                return;
            }

            if (_target == null || _transform == null || _canMove.Evaluate() == false) 
                return;


            Vector3 nextStep = Vector3.MoveTowards(
                _transform.position,
                _target.Value.Transform.position,
                _moveSpeed.Value * deltaTime
            );

            nextStep.y = _transform.position.y;
            _transform.position = nextStep;

            Vector3 directionToTarget = (_target.Value.Transform.position - _transform.position).normalized;
            directionToTarget.y = 0; 
            _moveDirection.Value = directionToTarget;
        }
    }
}
