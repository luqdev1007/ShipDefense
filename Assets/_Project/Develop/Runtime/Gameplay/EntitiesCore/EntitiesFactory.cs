using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.Meta.Features.ShipUpgrades;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesFactory
    {
        private readonly DIContainer _container;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly MonoEntitiesFactory _monoEntitiesFactory;

        private readonly CollidersRegistryService _collidersRegistryService;

        public EntitiesFactory(DIContainer container)
        {
            _container = container;
            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
        }

        public Entity CreateWizard(Transform parent)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, parent, "Entities/Wizard");

            entity
                .AddMoveDirection()
                .AddRotationDirection()
                .AddCurrentTarget()
                .AddMoveSpeed(new ReactiveVariable<float>(10));

            entity
                .AddSystem(new RigidbodyMovementSystem());

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateCaptain(Transform parent)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, parent, "Entities/Captain");

            entity
                .AddMoveDirection()
                .AddRotationDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(10));

            entity
                .AddSystem(new RigidbodyMovementSystem());

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateMainShip()
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, Vector3.up * 10, "Entities/MainShip")
                .GetComponentInChildren<BallistaController>().Init(_container.Resolve<IInputService>());

            PlayerMainShipDataProvider shipData = _container.Resolve<PlayerMainShipDataProvider>();

            entity
                .AddMaxHealth(new ReactiveVariable<float>(shipData.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(shipData.MaxHealth))

                .AddTeam(new ReactiveVariable<Teams>(Teams.Allies))

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(2))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(2))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                ;


            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                .AddSystem(new ApplyDamageSystem())
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateSoldier(Transform parent, Teams team)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, "Entities/Enemies/Soldier");

            entity
                .AddMaxHealth(new ReactiveVariable<float>(10))
                .AddCurrentHealth(new ReactiveVariable<float>(10))

                .AddTeam(new ReactiveVariable<Teams>(team))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(2))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(2))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));


            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateDriver(Transform parent, Teams team)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, "Entities/Enemies/Driver");

            entity
                .AddMaxHealth(new ReactiveVariable<float>(10))
                .AddCurrentHealth(new ReactiveVariable<float>(10))

                .AddTeam(new ReactiveVariable<Teams>(team))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(2))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(2))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));


            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateArcher(Transform parent, Teams team)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, "Entities/Enemies/Archer");

            entity
                .AddMaxHealth(new ReactiveVariable<float>(10))
                .AddCurrentHealth(new ReactiveVariable<float>(10))

                .AddTeam(new ReactiveVariable<Teams>(team))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(2))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(2))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));


            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateSmallShip(Teams team)
        {
            Entity entity = CreateEmpty();

            Transform[] spawners = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
                .Where(i => i.gameObject.layer == LayersAPI.LayerSpawner).ToArray(); // game input args?

            Vector3 randomOfsset = new Vector3(Random.Range(-30, 10), 0, Random.Range(-30, 10));
            Transform randomSpawner = spawners[Random.Range(0, spawners.Length)];

            MonoEntity mono = _monoEntitiesFactory.Create(entity, randomSpawner, "Entities/SmallShip");
            mono.transform.position += randomOfsset;
            mono.transform.SetParent(null);

            ShipPlace driverPlace = null;

            foreach (ShipPlace i in mono.GetComponentsInChildren<ShipPlace>())
            {
                if (i.PlaceType == ShipPlaceType.Driver)
                {
                    driverPlace = i;
                }
            }

            entity
                .AddMoveSpeed(new ReactiveVariable<float>(Random.Range(2, 4)))
                .AddMoveDirection(new ReactiveVariable<Vector3>())

                .AddMaxHealth(new ReactiveVariable<float>(5))
                .AddCurrentHealth(new ReactiveVariable<float>(5))

                .AddBodyContactDamage(new ReactiveVariable<float>(2))
                .AddContactsDetectingMask(LayersAPI.LayerMaskHittable)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddTeam(new ReactiveVariable<Teams>(team))
                .AddIsTouchAnotherTeam()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(2))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(2))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => driverPlace.transform.childCount > 0));


            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustExplode = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value == true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddCanMove(canMove)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustExplode(mustExplode)
                ;

            entity
                //.AddSystem(new RigidbodyDirectionalRotatorSystem())
                .AddSystem(new RigidbodyMoveTowardsTargetSystem(_entitiesLifeContext))

                .AddSystem(new ApplyDamageSystem())

                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())
                .AddSystem(new AnotherTeamTouchDetectorSystem())

                .AddSystem(new SelfExplodeSystem(_container.Resolve<ExplosionsFactory>()))
            ;

            _entitiesLifeContext.Add(entity);

            // unity logic
            ShipPlace[] places = mono.transform.GetComponentsInChildren<ShipPlace>();

            foreach (ShipPlace i in places)
            {
                switch (i.PlaceType)
                {
                    case ShipPlaceType.MeleeSmall:
                        CreateSoldier(i.transform, team);
                        break;

                    case ShipPlaceType.RangeSmall:
                        CreateArcher(i.transform, team);
                        break;

                    case ShipPlaceType.Driver:
                        CreateDriver(i.transform, team);
                        break;
                }
            }

            return entity;
        }

        public Entity CreateArrowProjectile(Transform parent, float damage, Entity owner, float tintPower)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, "Entities/ArrowProjectile");
            Vector3 shootDirection = parent.forward;

            entity
                .AddPushDirection(new ReactiveVariable<Vector3>(shootDirection))
                .AddPushForce(new ReactiveVariable<float>(25 * tintPower))
                .AddGravityScale(new ReactiveVariable<float>(10))

                .AddIsDead()
                .AddContactsDetectingMask(LayersAPI.LayerMaskWater)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))
                .AddBodyContactDamage(new ReactiveVariable<float>(damage))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()
                .AddIsTouchAnotherTeam()
                .AddTeam(new ReactiveVariable<Teams>(owner.Team.Value))
                ;

            ICompositeCondition mustDie = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value == true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true));

            entity
                .AddMustDie(mustDie)
                .AddMustExplode(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                  .AddSystem(new RigidbodyGravityApplySystem())      
                  .AddSystem(new AddDelayedForceSystem(0.2f, _container.Resolve<ICoroutinesPerformer>())) 
                  .AddSystem(new SlowApearEntityViewSystem(0.25f, _container.Resolve<ICoroutinesPerformer>()))
                  .AddSystem(new TransformRotateWithLinearVelocitySystem()) 

                  .AddSystem(new DeathSystem())
                  .AddSystem(new DisableCollidersOnDeathSystem())
                  .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                  .AddSystem(new BodyContactDetectingSystem())
                  .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                  .AddSystem(new DealDamageOnContactSystem())
                  .AddSystem(new DeathMaskTouchDetectorSystem())
                  .AddSystem(new AnotherTeamTouchDetectorSystem())

                  .AddSystem(new SelfExplodeSystem(_container.Resolve<ExplosionsFactory>()))
                  ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        private Entity CreateEmpty() => new Entity();
    }
}
