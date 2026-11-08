using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class BattleBoardBarrierDamageEventResponse : BattleBoardEventResponse
    {
        [Key(2)]
        public int OperatorUniqueKey { get; set; }

        [Key(3)]
        public int TargetUniqueKey { get; set; }

        [Key(4)]
        public int MaxBufferedDamage { get; set; }

        [Key(5)]
        public int BufferedDamage { get; set; }

        [Key(6)]
        public int Damage { get; set; }

        [Key(7)]
        public DamageType DamageType { get; set; }

    }
}
