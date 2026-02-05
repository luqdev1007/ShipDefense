using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilites.RaycastManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class TestGameplay : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _hittableLayers;

        private DIContainer _container;
        private EntitiesFactory _entitiesFactory;

        private ExplosionsFactory _explosionsFactory;

        private SurfaceRaycaster _surfaceRaycaster;

        private IInputService _input;

        private bool _isRunning;

        private Entity _mainShip;

        public void Initialize(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _explosionsFactory = _container.Resolve<ExplosionsFactory>();
            _input = _container.Resolve<IInputService>();
            _surfaceRaycaster = _container.Resolve<SurfaceRaycaster>();
        }

        public void Run()
        {
            _isRunning = true;

            _mainShip = _entitiesFactory.CreateMainShip();
        }

        private void Update()
        {
            if (_isRunning == false)
                return;

            if (_input.IsAttackKeyPressed)
            {
                if (_surfaceRaycaster.TryGetHitInfo(_mainCamera, _hittableLayers, out RaycastHit hitInfo))
                {
                    _explosionsFactory.Create(ExplosionType.Large, hitInfo.point);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _entitiesFactory.CreateShip(Teams.Enemies);
            }

            /*
            if (Input.GetKeyDown(KeyCode.D))
            {
                _mainShip.TakeDamageRequest.Invoke(1);
            }
            */
        }
    }
}