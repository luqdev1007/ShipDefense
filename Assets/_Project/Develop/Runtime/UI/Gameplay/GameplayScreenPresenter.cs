using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

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


        public void SubscribeHealthView(IReadOnlyVariable<float> currentHealth, IReadOnlyVariable<float> maxHealth)
        {
            _view.ProgressFilledImageView.Init(currentHealth, maxHealth);
        }
    }
}