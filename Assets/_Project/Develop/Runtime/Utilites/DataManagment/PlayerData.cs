using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilites.DataManagment
{
    public class PlayerData : ISaveData
    {
        public int Wins;
        public int Losses;
        public List<int> CompletedLevels;
    }
}
