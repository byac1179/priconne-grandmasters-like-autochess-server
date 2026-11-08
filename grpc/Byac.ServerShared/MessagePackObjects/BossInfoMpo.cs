using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{

    [MessagePackObject]
    public class BossInfoMpo
    {
        [Key(0)]
        public int BossUnitId { get; set; }

        [Key(1)]
        public int MaxHp { get; set; }
    }
}
