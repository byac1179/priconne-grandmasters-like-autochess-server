using MessagePack;
using Byac.ServerShared.MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class MatchMakingCompleteResponse
    {
        [MessagePackFormatter(typeof(UlidMessagePackFormatter))]
        [Key(0)]
        public Ulid BattleId { get; set; }

        [Key(1)]
        public string RouteToken { get; set; }

    }
}
