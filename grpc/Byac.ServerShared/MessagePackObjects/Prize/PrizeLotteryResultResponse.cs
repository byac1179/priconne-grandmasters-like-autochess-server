using MessagePack;
using System;

namespace Byac.ServerShared.MessagePackObjects.Prize
{
    [MessagePackObject]
    public class PrizeLotteryResultResponse
    {
        [Key(0)] public Ulid BattleId { get; set; }
        [Key(1)] public DateTimeOffset CreatedAt { get; set; }
        [Key(2)] public int PrizeLotteryResultId { get; set; }
        [Key(3)] public bool IsWin { get; set; }
        [Key(4)] public int? PrizeId { get; set; }
        [Key(5)] public string Url { get; set; }
        [Key(6)] public int Rank { get; set; }
    }
}
