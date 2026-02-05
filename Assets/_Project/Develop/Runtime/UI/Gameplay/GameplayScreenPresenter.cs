using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;

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
    }
}