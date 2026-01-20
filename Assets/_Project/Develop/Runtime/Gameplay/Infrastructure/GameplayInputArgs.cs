namespace Assets._Project.Develop.Runtime.Utilites.SceneManagement
{
    public class GameplayInputArgs : IInputSceneArgs
    {
        public GameplayInputArgs(int levelNumber)
        {
            LevelNumber = levelNumber;
        }

        public int LevelNumber { get; private set; }
    }
}
