using System;
using MessagePack;

namespace Byac.ServerShared.MessagePackObjects
{

    [MessagePackObject]
    public class ResumeResponse
    {
        [Key(0)]
        public TimeSpan? RemainTime { get; set; }
    }
}
