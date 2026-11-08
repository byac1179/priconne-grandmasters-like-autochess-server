using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects.User
{
    [MessagePackObject]
    public class CurrentBattleResponse
    {
        [MessagePackFormatter(typeof(UlidMessagePackFormatter))]
        [Key(0)]
        public Ulid BattleId { get; set; }

        [Key(1)]
        public string RouteToken { get; set; }

        [Key(2)]
        public BattleRule Rule { get; set; }
    }
}
