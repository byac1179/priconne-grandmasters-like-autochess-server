using MagicOnion;
using Byac.ServerShared.MessagePackObjects.Prize;
using Byac.ServerShared.Primitives;
using System;
using System.Collections.Generic;

namespace Byac.ServerShared.Services
{
    public interface IPrizeService : IService<IPrizeService>, IServiceMarker
    {
        UnaryResult<ServiceResult<PrizeLotteryResultSetResponse>> GetLotteryResultByBattleAsync(Ulid battleId);
        UnaryResult<ServiceResult<PrizeLotteryResultResponse[]>> GetLotteryResultsByDateAsync(int year, int month, int day);
        UnaryResult<ServiceResult<PrizeScheduleResponse>> GetCurrentScheduleAsync();
    }
}
