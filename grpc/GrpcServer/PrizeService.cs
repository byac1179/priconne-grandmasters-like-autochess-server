using MagicOnion;
using MagicOnion.Server;
using Byac.ServerShared.MessagePackObjects.Prize;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using System;
using System.Collections.Generic;

namespace Byac.Server
{
    public class PrizeService : ServiceBase<IPrizeService>, IPrizeService
    {
        public async UnaryResult<ServiceResult<PrizeLotteryResultSetResponse>> GetLotteryResultByBattleAsync(Ulid battleId)
        {
            Console.WriteLine($"[PRIZE] GetLotteryResultByBattleAsync battleId={battleId}");
            var result = new ServiceResult<PrizeLotteryResultSetResponse>();
            result.Succeeded = true;

            result.Result = new PrizeLotteryResultSetResponse
            {
                PrizeFeature = PrizeFeatureToggle.Full,
                Results = new[]
                {
                    new PrizeLotteryResultResponse
                    {
                        BattleId = battleId,
                        CreatedAt = DateTimeOffset.Now,
                        PrizeLotteryResultId = 1,
                        IsWin = false,
                        PrizeId = 100010,
                        Url = "",
                        Rank = 1,
                    },
                },
            };
            return result;
        }

        public async UnaryResult<ServiceResult<PrizeLotteryResultResponse[]>> GetLotteryResultsByDateAsync(int year, int month, int day)
        {
            Console.WriteLine($"[PRIZE] GetLotteryResultsByDateAsync {year}-{month}-{day}");
            var result = new ServiceResult<PrizeLotteryResultResponse[]>();
            result.Succeeded = true;
            result.Result = Array.Empty<PrizeLotteryResultResponse>();
            return result;
        }

        public async UnaryResult<ServiceResult<PrizeScheduleResponse>> GetCurrentScheduleAsync()
        {
            Console.WriteLine("[PRIZE] GetCurrentScheduleAsync");
            var result = new ServiceResult<PrizeScheduleResponse>();
            result.Succeeded = true;
            result.Result = new PrizeScheduleResponse
            {
                Begin = new PrizeScheduleDateOnly { Year = 2026, Month = 1, Day = 1 },
                End = new PrizeScheduleDateOnly { Year = 2027, Month = 1, Day = 1 },
            };
            return result;
        }
    }
}
