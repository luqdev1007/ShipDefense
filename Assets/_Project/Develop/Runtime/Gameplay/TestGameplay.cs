using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class TestGameplay : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawners;
        [SerializeField] private Camera _mainCamera;

        private DIContainer _container;

        private ProjectilesFactory _projectilesFactory;
        private EnemiesFactory _enemiesFactory;
        private VehiclesFactory _vehiclesFactory;
        private BrainsFactory _brainsFactory;
        private MainHeroesFactory _mainHeroesFactory;

        private GameplayScreenPresenter _gameplayScreenPresenter;

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

            _enemiesFactory = _container.Resolve<EnemiesFactory>();
            _vehiclesFactory = _container.Resolve<VehiclesFactory>();
            _mainHeroesFactory = _container.Resolve<MainHeroesFactory>();
            _projectilesFactory = _container.Resolve<ProjectilesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();

            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            _input = _container.Resolve<IInputService>();
        }

        public void Run()
        {
            _isRunning = true;

            // main ship
            MainShipConfig config = _container.Resolve<ConfigsProviderService>()
                .GetConfig<MainShipConfig>(); // saved data provide

            _mainShip = _vehiclesFactory.Create(null, Teams.Allies, config);
            ShipPlace[] shipPlaces = _mainShip.Transform.GetComponentsInChildren<ShipPlace>();

            // ballista
            _ballista = _mainShip.Transform.GetComponentInChildren<BallistaController>(); // entity?
            _projectileParent = _ballista.ProjectileParent;

            // captain
            Transform captainSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Driver).transform;
            _captain = _mainHeroesFactory.CreateCaptain(captainSpawnPointParent);

            // wizard
            Transform wizardSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Mast).transform;
            _wizard = _mainHeroesFactory.CreateWizard(wizardSpawnPointParent);

            // UI
            _gameplayScreenPresenter.SubscribeHealthViewToEntity(_mainShip);
        }

        private void Update()
        {
            if (_isRunning == false)
                return;

            if (_input.IsAttackKeyReleased)
            {
                float ballistaPower = 25;
                float launchPowerMultiplier = _ballista.ChargeProgress < 0.5f ? 1f : _ballista.ChargeProgress * 2f;

                _projectilesFactory.Create(
                    _projectileParent, 
                    new ProjectileCreationContext(_mainShip, 
                    launchPower: ballistaPower * launchPowerMultiplier, finalDamage: 2, launchDelay: 0.25f),
                    _container.Resolve<ConfigsProviderService>().GetConfig<SimpleProjectileConfig>());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                _gameplayScreenPresenter.ShowAnnouncement();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                float prepTime = 10;
                _gameplayScreenPresenter.ShowPreperationTimer(_container
                    .Resolve<TimerServiceFactory>().Create(prepTime));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Transform randomSpawner = _spawners[Random.Range(0, _spawners.Length)];

                Entity entity = _vehiclesFactory.Create(randomSpawner, 
                    Teams.Enemies, 
                    _container.Resolve<ConfigsProviderService>().GetConfig<SmallShipConfig>());

                ShipPlace[] places = entity.Transform.GetComponentsInChildren<ShipPlace>();

                foreach (ShipPlace place in places)
                {
                    switch (place.PlaceType)
                    {
                        case ShipPlaceType.Driver:
                            _enemiesFactory.Create(place.transform, 
                                _container.Resolve<ConfigsProviderService>().GetConfig<DriverConfig>());
                            break;
                    }
                }
            }
        }
    }
}