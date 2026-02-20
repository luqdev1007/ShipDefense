using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using UnityEngine;

public class ShipStartCombatManager : MonoBehaviour
{
    private SceneSwitcherService _sceneSwitcherService;
    private ICoroutinesPerformer _coroutinesPerformer;
    private bool _isTriggered = false;

    public void Init(SceneSwitcherService sceneSwitcherService, ICoroutinesPerformer coroutinesPerformer)
    {
        _sceneSwitcherService = sceneSwitcherService;
        _coroutinesPerformer = coroutinesPerformer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyShip enemyShip) && _isTriggered == false)
        {
            _isTriggered = true;
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, new GameplayInputArgs(1)));
        }
    }
}