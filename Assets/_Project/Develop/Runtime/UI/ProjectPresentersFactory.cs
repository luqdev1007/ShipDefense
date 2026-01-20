using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.TestPopup;
using Assets._Project.Develop.Runtime.UI.LevelsMenuPopup;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;

namespace Assets._Project.Develop.Runtime.UI
{
    public class ProjectPresentersFactory
    {
        private DIContainer _container;

        public ProjectPresentersFactory(DIContainer container)
        {
            _container = container;
        }


        public LevelsMenuPopupPresenter CreateLevelsMenuPopupPresenter(LevelsMenuPopupView view)
        {
            return new LevelsMenuPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<ConfigsProviderService>(),
                this,
                _container.Resolve<ViewsFactory>(),
                view
                );
        }

        public ConfirmPopupPresenter CreateConfirmPopupPresenter(ConfirmPopupView view, Action onConfirmButtonClicked, string header)
        {
            return new ConfirmPopupPresenter(view, _container.Resolve<ICoroutinesPerformer>(), onConfirmButtonClicked, header);
        }
    }
}
