using Assets._Project.Develop.Runtime.Meta.Features.ShipUpgrades;
using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [SerializeField] private IconTextView _healthDisplay;

        public void Init()
        {
            Debug.Log("Inited");
        }
    }
}