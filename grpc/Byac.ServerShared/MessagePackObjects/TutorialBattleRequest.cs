using MessagePack;

namespace Byac.ServerShared.MessagePackObjects
{

    [MessagePackObject(true)]
    public class TutorialBattleRequest
    {
        public int MatchId { get; set; }
        public int PrimaryViewerId { get; set; }
        public int SecondaryViewerId { get; set; }
        public Unit[] PrimaryUnits { get; set; }
        public Unit[] SecondaryUnits { get; set; }

        [MessagePackObject(true)]
        public class Unit
        {
            public int UnitId { get; set; }
            public int UniqueKey { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Rank { get; set; }
            public int[] Equipments { get; set; }
        }
    }
}
