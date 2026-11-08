using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class MatchMakingEntryRequest
    {
        [Key(0)]
        public int DummyKindId;

    }
}
