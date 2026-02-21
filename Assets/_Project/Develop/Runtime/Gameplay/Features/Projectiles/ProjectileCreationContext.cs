using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class ProjectileCreationContext
    {
        public Entity Owner { get; private set; }
        public float LaunchPower { get; private set; }
        public float FinalDamage { get; private set; }
        public float LaunchDelay { get; private set; }

        public ProjectileCreationContext(
            Entity owner, 
            float launchPower, 
            float finalDamage, 
            float launchDelay)
        {
            Owner = owner;
            LaunchPower = launchPower;
            FinalDamage = finalDamage;
            LaunchDelay = launchDelay;
        }
    }
}
