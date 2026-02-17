using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class TestGameplay : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _hittableLayers;

        private DIContainer _container;

        private EntitiesFactory _entitiesFactory;
        private BrainsFactory _brainsFactory;

        private IInputService _input;

        private bool _isRunning;

        // Entities
        private Entity _mainShip;
        private Entity _captain;
        private Entity _wizard;

        private BallistaController _ballista;

        private Transform _projectileParent;


        public void Initialize(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();

            _input = _container.Resolve<IInputService>();
        }

        public void Run()
        {
            _isRunning = true;

            _mainShip = _entitiesFactory.CreateMainShip();
            _container.Resolve<GameplayScreenPresenter>().SubscribeHealthView(_mainShip.CurrentHealth, _mainShip.MaxHealth);

            _ballista = _mainShip.Transform.GetComponentInChildren<BallistaController>();
            _projectileParent = _ballista.ProjectileParent;

            ShipPlace[] shipPlaces = _mainShip.Transform.GetComponentsInChildren<ShipPlace>();

            Transform captainSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Driver).transform;
            _captain = _entitiesFactory.CreateCaptain(captainSpawnPointParent);
            _brainsFactory.CreateCaptainBrain(_captain);

            Transform wizardSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Mast).transform;
            _wizard = _entitiesFactory.CreateWizard(wizardSpawnPointParent);
            _brainsFactory.CreateWizardBrain(_wizard, new NearestDamagableTargetSelector(_wizard));
        }

        private void Update()
        {
            if (_isRunning == false)
                return;

            if (_input.IsAttackKeyReleased)
            {
                float power = _ballista.ChargeProgress < 0.5f ? 1f : _ballista.ChargeProgress * 2f;
                _entitiesFactory.CreateArrowProjectile(_projectileParent, 1, _mainShip, power);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _entitiesFactory.CreateSmallShip(Teams.Enemies);
            }
        }
    }
}