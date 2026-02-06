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
   
            if (_input.IsAttackKeyPressed)
            {
                _shootTimer = 0;
                _isShootStarted = true;
            }

            if (_isShootStarted && _shootTimer < 2)
            {
                _shootTimer += Time.deltaTime;
            }

            if (_input.IsAttackKeyReleased)
            {
                _shootTimer = _shootTimer < 1 ? 1 : _shootTimer;
                _isShootStarted = false;
                _entitiesFactory.CreateArrowProjectile(_projectileParent, _projectileParent.forward, 1, _mainShip, _shootTimer);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _entitiesFactory.CreateShip(Teams.Enemies);
            }
        }
    }
}