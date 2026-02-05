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
            }

        }
    }
}