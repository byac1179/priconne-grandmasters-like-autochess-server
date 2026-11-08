using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{

    [MessagePackObject]
    public class BossBattleResultResponse
    {
        [Key(0)]
        public int BossUnitId { get; set; }

        [Key(1)]
        public int BossMaxHp { get; set; }

        [Key(2)]
        public int BossRemainHp { get; set; }

        [Key(3)]
        public int TotalGivedDamage { get; set; }
    }
}
