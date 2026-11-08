using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class MatchMakingJoinResponse
    {
        [Key(0)]
        public MatchMakingUserDataMpo[] GroupUsers { get; set; }
    }
}
