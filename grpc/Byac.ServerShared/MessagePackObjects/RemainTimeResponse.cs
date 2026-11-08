using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class RemainTimeResponse
    {
        [Key(0)]
        public int? RemainTime { get; set; }

    }
}
