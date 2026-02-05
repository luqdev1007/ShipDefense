using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features
{
    public interface ITargetSelector
    {
        Entity SelectTargetFrom(IEnumerable<Entity> targets);
    }
}
