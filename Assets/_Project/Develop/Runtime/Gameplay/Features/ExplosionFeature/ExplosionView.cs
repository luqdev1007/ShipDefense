namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    using UnityEngine;

    public class ExplosionView : MonoBehaviour
    {
        public void Initialize(float range, float damage)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, range);

            foreach (var target in targets)
            {
                Debug.Log($"Взрыв задел: {target.name} на {damage} урона");
            }

            Destroy(gameObject, 2f);
        }
    }
}