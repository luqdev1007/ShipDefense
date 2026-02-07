using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class SlowApearEntityViewSystem : IInitializableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private WaitForSeconds _delay;
        private float _appearTime;
        private Animator _view;

        public SlowApearEntityViewSystem(float appearTime, ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _appearTime = appearTime;

            _delay = new WaitForSeconds(_appearTime);
        }

        public void OnInit(Entity entity)
        {
            _view = entity.Animator;

            _view.gameObject.SetActive(false);

            _coroutinesPerformer.StartPerform(AppearProcess());
        }

        private IEnumerator AppearProcess()
        {
            yield return _delay;

            _view.gameObject.SetActive(true);
        }
    }
}
