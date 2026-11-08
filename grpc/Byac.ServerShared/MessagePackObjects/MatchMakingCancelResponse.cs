using MessagePack;
using Byac.ServerShared.MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class MatchMakingCancelResponse
    {
        [MessagePackFormatter(typeof(UlidMessagePackFormatter))]
        [Key(0)]
        public Ulid CanceledEntryId;

        [Key(1)]
        public MatchMakingUserDataMpo[] GroupUsers;
    }
}
