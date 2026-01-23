using Assets._Project.Develop.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            Debug.Log("Process registrations on main menu scene");

            /*
            container.RegisterAsSingle(CreateMainMenuUIRoot).NonLazy();
            container.RegisterAsSingle(CreateMainMenuPresentersFactory);
            container.RegisterAsSingle(CreateMainMenuScreenPresenter).NonLazy();
            container.RegisterAsSingle(CreateMainMenuPopupService);
            */

            /*
            private static MainMenuPopupService CreateMainMenuPopupService(DIContainer container)
            {
                return new MainMenuPopupService(
                    container.Resolve<ViewsFactory>(),
                    container.Resolve<ProjectPresentersFactory>(),
                    container.Resolve<MainMenuUIRoot>()
                    );
            }
            */
        }
    }
}