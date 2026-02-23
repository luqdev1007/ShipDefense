using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class TestGameplay : MonoBehaviour
    {
        [SerializeField] private StageConfig _config;
        private StagesFactory _stagesFactory;
        private IStage _stage;

        private DIContainer _container;

        private GameplayScreenPresenter _gameplayScreenPresenter;
        private IInputService _input;

        // factories
        private EntitiesFactory _entitiesFactory;
        private ProjectilesFactory _projectilesFactory;
        private EnemiesFactory _enemiesFactory;
        private VehiclesFactory _vehiclesFactory;
        private BrainsFactory _brainsFactory;
        private MainHeroesFactory _mainHeroesFactory;

        // Main Entities
        private Entity _mainShip;
        private Entity _captain;
        private Entity _wizard;
        private Entity _engineer;
        private Entity _ballista;

        // tests
        private BallistaController _ballistaController;

        private bool _isRunning;

        public void Initialize(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _enemiesFactory = _container.Resolve<EnemiesFactory>();
            _vehiclesFactory = _container.Resolve<VehiclesFactory>();
            _mainHeroesFactory = _container.Resolve<MainHeroesFactory>();
            _projectilesFactory = _container.Resolve<ProjectilesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();

            _stagesFactory = _container.Resolve<StagesFactory>();

            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            _input = _container.Resolve<IInputService>();
        }

        public void Run()
        {
            _isRunning = true;

            // stage
            _stage = _stagesFactory.Create(_config);
            _stage.Completed.Subscribe(OnStageCompleted);
            _stage.Start();

            // main ship
            MainShipConfig config = _container.Resolve<ConfigsProviderService>()
                .GetConfig<MainShipConfig>(); // saved data provide

            _mainShip = _vehiclesFactory.Create(null, Teams.Allies, config);
            ShipPlace[] shipPlaces = _mainShip.Transform.GetComponentsInChildren<ShipPlace>();

            // ballista
            BallistaConfig ballistaConfig = _container.Resolve<ConfigsProviderService>().GetConfig<BallistaConfig>();
            Transform ballistaSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Ballista).transform;
            _ballista = _entitiesFactory.CreateBallista(ballistaSpawnPointParent, ballistaConfig, Teams.Allies);

            _ballistaController = _ballista.Transform.GetComponent<BallistaController>();
            _ballistaController.Init(_container.Resolve<IInputService>());

            _mainShip.Transform.GetComponentInChildren<CinemachineCamera>().Target.TrackingTarget = _ballistaController.CameraPivot;

            // captain
            Transform captainSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Driver).transform;
            _captain = _mainHeroesFactory.CreateCaptain(captainSpawnPointParent);

            // wizard
            Transform wizardSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Mast).transform;
            _wizard = _mainHeroesFactory.CreateWizard(wizardSpawnPointParent);

            // engineer
            Transform engineerSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Paluba).transform;
            _engineer = _mainHeroesFactory.CreateEngineer(engineerSpawnPointParent);
            _engineer.Transform.GetComponent<ConfigurableJoint>().connectedBody = _ballistaController.EngineerPivot;

            // UI
            _gameplayScreenPresenter.SubscribeHealthViewToEntity(_mainShip);
        }

        private void OnStageCompleted()
        {
            Debug.Log("Victory!");
            _stage.Cleanup();
        }

        private void Update()
        {
            if (_isRunning == false)
                return;

            _stage.Update(Time.deltaTime);

            if (_input.IsAttackKeyReleased)
            {
                BallistaAttack();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                _gameplayScreenPresenter.ShowAnnouncement();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ShowPrepTimer();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // CreateEnemySmallShip();
            }
        }

        private void BallistaAttack()
        {
            float ballistaPower = _ballistaController.ShootPower;
            float launchPowerMultiplier = _ballistaController.ChargeProgress < 0.5f ? 1f : _ballistaController.ChargeProgress * 2f;
            SimpleProjectileConfig config = _container.Resolve<ConfigsProviderService>().GetConfig<SimpleProjectileConfig>();
            config.GravityScale = 10;

            _projectilesFactory.Create(
                _ballistaController.ProjectileParent,
                new ProjectileCreationContext(_ballista,
                launchPower: ballistaPower * launchPowerMultiplier, 
                finalDamage: 2, 
                launchDelay: 0.25f, 
                shootDirection: _ballistaController.ProjectileParent.forward),
                config);
        }

        private void ShowPrepTimer()
        {
            float prepTime = 10;
            _gameplayScreenPresenter.ShowPreperationTimer(_container
                .Resolve<TimerServiceFactory>().Create(prepTime));
        }

        /*
        private void CreateEnemySmallShip()
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

                    case ShipPlaceType.MeleeSmall:
                        _enemiesFactory.Create(place.transform,
                            _container.Resolve<ConfigsProviderService>().GetConfig<SoldierConfig>());
                        break;

                    case ShipPlaceType.RangeSmall:
                        _enemiesFactory.Create(place.transform,
                            _container.Resolve<ConfigsProviderService>().GetConfig<ArcherConfig>());
                        break;
                }
            }
        }
        */
    }
}