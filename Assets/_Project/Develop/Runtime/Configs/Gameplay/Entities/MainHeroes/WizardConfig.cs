using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "WizardConfig", order = 54, menuName = "Configs/Gameplay/Entities/New Wizard Config")]
    public class WizardConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHeroes/Wizard";
        [field: SerializeField, Min(0)] public float RotationSpeed { get; private set; } = 90;
    }
}
