using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class Explosion
    {
        private float _range;
        private float _power;

        public Explosion(float range, float power)
        {
            _range = range;
            _power = power;
        }

        public void Activate(Vector3 at)
        {
            Collider[] targets = Physics.OverlapSphere(at, _range);

            foreach (Collider target in targets)
            {
                IExplosionable actor = target.GetComponent<IExplosionable>();

                if (actor != null)
                {
                    actor.Explode(_power, at);
                }

                MonoEntity monoEntity = target.GetComponent<MonoEntity>();

                if (monoEntity != null)
                {
                    if (monoEntity.LinkedEntity.HasComponent<CurrentHealth>())
                    {
                        monoEntity.LinkedEntity.CurrentHealth.Value -= 1;
                        Debug.Log(monoEntity.gameObject.name + " health: " + monoEntity.LinkedEntity.CurrentHealth.Value);
                    }
                }
            }

        }
    }
}