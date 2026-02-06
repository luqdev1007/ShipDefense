using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics
{
    public class AddInstantPushPowerSystem : IInitializableSystem
    {
        public void OnInit(Entity entity)
        {
            entity.Rigidbody.linearVelocity = entity.PushDirection.Value * entity.PushForce.Value;
        }
    }
}