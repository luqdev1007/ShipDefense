using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class WinState : EndGameState, IUpdatableState
    {
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly WalletService _walletService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;

        public WinState(
            IInputService inputService,
            GameplayInputArgs gameplayInputArgs,
            PlayerDataProvider playerDataProvider,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer,
            WalletService walletService,
            GameplayScreenPresenter gameplayScreenPresenter) : base(inputService)
        {
            _gameplayInputArgs = gameplayInputArgs;
            _playerDataProvider = playerDataProvider;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _walletService = walletService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("VICTORY!");

            _walletService.Add(CurrencyTypes.Gold, _gameplayInputArgs.LevelConfig.BaseReward * _gameplayInputArgs.LevelConfig.Difficulty);
            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync()); // saves win reward

            _gameplayScreenPresenter.ShowAnnouncement("ПОБЕДА!\nНажмите 'Q' для перехода в главное меню", "здесь могла быть ваша реклама");
        }

        public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
            }
        }
    }
}
