using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class TransformMoveTowardsTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ICompositeCondition _mustDie;
        private Transform _transform;

        private Transform _target;

        public void OnInit(Entity entity)
        {
            // тут должен браться target в виде игрока
            _target = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
                .First(i => i.gameObject.name.ToLower().Contains("mainship"));

            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _transform = entity.Transform;
            _mustDie = entity.MustDie;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_target == null || _transform == null || _mustDie.Evaluate()) 
                return;

            if (Vector3.Distance(_transform.position, _target.position) < 10)
                return;

            Vector3 nextStep = Vector3.MoveTowards(
                _transform.position,
                _target.position,
                _moveSpeed.Value * deltaTime
            );

            nextStep.y = _transform.position.y;
            _transform.position = nextStep;

            Vector3 directionToTarget = (_target.position - _transform.position).normalized;
            directionToTarget.y = 0; 
            _moveDirection.Value = directionToTarget;
        }
    }
}
