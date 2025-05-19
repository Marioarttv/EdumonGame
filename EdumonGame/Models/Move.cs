using EdumonGame.Services;

namespace EdumonGame.Models
{
    public class Move
    {
        public MoveBase Base { get; set; }

        public Move(MoveBase pBase)
        {
            Base = pBase;
        }

        public Move(MoveSaveData saveData)
        {
            Base = MoveDB.GetObjectByName(saveData.name);
        }

        public MoveSaveData GetSaveData()
        {
            var saveData = new MoveSaveData()
            {
                name = Base.Name
            };
            return saveData;
        }
    }

    public class MoveSaveData
    {
        public string name;
    }
}