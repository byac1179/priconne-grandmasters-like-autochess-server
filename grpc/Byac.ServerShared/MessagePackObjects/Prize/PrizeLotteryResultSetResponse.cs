using MessagePack;
using System.Collections.Generic;

namespace Byac.ServerShared.MessagePackObjects.Prize
{
    [MessagePackObject]
    public class PrizeLotteryResultSetResponse
    {
        [Key(0)] public PrizeFeatureToggle PrizeFeature { get; set; }
        [Key(1)] public IReadOnlyList<PrizeLotteryResultResponse> Results { get; set; }
    }

    [MessagePackObject]
    public class PrizeScheduleDateOnly
    {
        [Key(0)] public int Year { get; set; }
        [Key(1)] public int Month { get; set; }
        [Key(2)] public int Day { get; set; }
    }

    [MessagePackObject]
    public class PrizeScheduleResponse
    {
        [Key(0)] public PrizeScheduleDateOnly Begin { get; set; }
        [Key(1)] public PrizeScheduleDateOnly End { get; set; }
    }
}
