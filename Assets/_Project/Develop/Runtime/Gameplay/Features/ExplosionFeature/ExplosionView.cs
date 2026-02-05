namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    using UnityEngine;

    public class ExplosionView : MonoBehaviour
    {
        public void Initialize(Explosion effect)
        {
            effect.Activate(transform.position);

            Destroy(gameObject, 2f);
        }
    }
}