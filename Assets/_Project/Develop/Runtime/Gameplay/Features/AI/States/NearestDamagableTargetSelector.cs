using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class NearestDamagableTargetSelector : ITargetSelector
    {
        private Entity _source;
        private Transform _sourceTransform;

        public NearestDamagableTargetSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Transform;
        }

        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            Debug.Log("Ищу цель среди: " + targets.Count() + " entities");

            string list = "";

            foreach (var target in targets)
            {
                if (target.Transform == null)
                    continue;

                list += target.Transform.gameObject.name + " ";
            }

            Debug.Log("Targets: " + list);

            IEnumerable<Entity> selectedTargets = targets.Where(target =>
            {
                bool result = target.HasComponent<TakeDamageRequest>();

                if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage))
                {
                    result = result && canApplyDamage.Evaluate();
                }

                result = result && EntitiesHelper.IsSameTeam(_source, target) == false;

                result = result && (target != _source);

                return result;
            });

            if (selectedTargets.Any() == false)
                return null;

            Entity closestTarget = selectedTargets.First();

            if (TryGetDistanceTo(closestTarget, out float minDistance) == false)
                return null;

            foreach (Entity target in selectedTargets)
            {
                if (TryGetDistanceTo(target, out float distance) == false)
                    continue;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private bool TryGetDistanceTo(Entity target, out float result)
        {
            if (target == null || target.Transform == null)
            {
                result = 0;
                return false;
            }

            result = (_sourceTransform.position - target.Transform.position).magnitude;

            return true;
        }
    }
}
