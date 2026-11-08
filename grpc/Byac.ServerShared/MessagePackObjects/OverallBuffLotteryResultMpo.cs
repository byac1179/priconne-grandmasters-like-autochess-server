using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public struct OverallBuffLotteryResultMpo
    {
        [Key(0)]
        public int ViewerId { get; set; }

        [Key(1)]
        public List<int> OverallBuffList { get; set; }

    }
}
