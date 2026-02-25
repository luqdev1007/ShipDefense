using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Timer;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;

        private List<IDisposable> _disposables = new();

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _view.Init();
            
            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _disposables.Clear();
        }

        public void HideUI()
        {
            _view.HealthView.Hide();
        }

        public void SubscribeHealthViewToEntity(Entity entity)
        {
            _view.HealthView.Init(entity.CurrentHealth, entity.MaxHealth);
            _view.HealthView.Show();
        }

        public void ShowAnnouncement(string header, string subheader = "")
        {
            _view.AnouncementView.SetText(header, subheader);
            _view.AnouncementView.Show();
        }

        public void ShowPreperationTimer(TimerService timerService)
        {
            _view.PrepTimerView.Show();

            timerService.CurrentTime.Subscribe(OnTimerChanged);
            timerService.CooldownEnded.Subscribe(OnTimerEnded);

            timerService.Restart();
        }

        private void OnTimerEnded()
        {
            _view.PrepTimerView.Hide();
        }

        private void OnTimerChanged(float arg1, float timeLeft)
        {
            _view.TimerText.text = ((int)timeLeft).ToString();
        }
    }
}