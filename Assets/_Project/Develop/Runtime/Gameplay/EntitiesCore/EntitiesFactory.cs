using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Linq;
using UnityEngine;

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

        public Entity CreateTit(Vector3 position)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, "Entities/Tit");

            entity
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(10));

            entity
                .AddSystem(new RigidbodyMovementSystem());

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateMainShip()
        {
            Entity entity = CreateEmpty();


            _monoEntitiesFactory.Create(entity, Vector3.up * 10, "Entities/MainShip");

            entity
                .AddMaxHealth(new ReactiveVariable<float>(10))
                .AddCurrentHealth(new ReactiveVariable<float>(10))

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

        public Entity CreateShip()
        {
            Entity entity = CreateEmpty();

            Transform[] spawners = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
                .Where(i => i.gameObject.layer == LayersAPI.LayerSpawner).ToArray(); // game input args?

            Transform randomSpawner = spawners[Random.Range(0, spawners.Length)];

            _monoEntitiesFactory.Create(entity, randomSpawner, "Entities/SmallShip");

            entity
                .AddMoveSpeed(new ReactiveVariable<float>(1))
                .AddMoveDirection(new ReactiveVariable<Vector3>())

                .AddMaxHealth(new ReactiveVariable<float>(10))
                .AddCurrentHealth(new ReactiveVariable<float>(10))

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
                .AddSystem(new TransformMoveTowardsTargetSystem())
                .AddSystem(new TransformDirectionalRotatorSystem())

                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                .AddSystem(new ApplyDamageSystem())
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        private Entity CreateEmpty() => new Entity();
    }
}
