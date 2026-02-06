using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
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

        private BallistaController _ballista;

        private Transform _projectileParent;

        private float _shootTimer = 0;
        private bool _isShootStarted = false;

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
            _container.Resolve<GameplayScreenPresenter>().SubscribeHealthView(_mainShip.CurrentHealth);

            _ballista = _mainShip.Transform.GetComponentInChildren<BallistaController>();
            _projectileParent = _ballista.ProjectileParent;
        }

        private void Update()
        {
            if (_isRunning == false)
                return;

            if (_input.IsAttackKeyReleased)
            {
                float power = _ballista.ChargeProgress < 0.5f ? 1f : _ballista.ChargeProgress * 2f;
                _entitiesFactory.CreateArrowProjectile(_projectileParent, _projectileParent.forward, 1, _mainShip, power);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _entitiesFactory.CreateShip(Teams.Enemies);
            }
        }
    }
}