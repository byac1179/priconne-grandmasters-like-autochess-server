using System;
using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using MagicOnion.Server;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class BattlePointRankingService : ServiceBase<IBattlePointRankingService>, IBattlePointRankingService
    {

        private const int TopLimit = 100;

        private const int DailyDays = 7;

        public async UnaryResult<ServiceResult<BattlePointRankingResponse>> GetBattlePointRankingsAsync()
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var fromKey = AccountDb.DateKeyOf(DateTime.Now.AddDays(-(DailyDays - 1)));
            var resp = BuildResponse(AccountStore.SnapshotAll(), AccountStore.LoadDailyBp(fromKey), viewerId);
            return new ServiceResult<BattlePointRankingResponse>
            {
                Succeeded = true,
                Result = resp,
                ErrorDetail = new GeneralError(),
            };
        }

        public static BattlePointRankingResponse BuildResponse(
            IReadOnlyList<AccountRecord> all,
            List<(int DateKey, int ViewerId, int Gain)> dailyAll,
            int viewerId)
        {
            var byId = all.ToDictionary(a => a.ViewerId);
            byId.TryGetValue(viewerId, out var me);

            if (me == null) me = new AccountRecord { ViewerId = viewerId, Name = "" };

            var resp = new BattlePointRankingResponse
            {
                Users = new Dictionary<int, UserResponse>(),
                DailyRankings = Array.Empty<BattlePointDailyRankingMpo>(),
            };

            var ordered = all
                .OrderByDescending(a => a.BattlePoint)
                .ThenBy(a => a.ViewerId)
                .ToList();
            var top = ordered.Take(TopLimit).ToArray();
            var topRows = new BattlePointRankingUserMpo[top.Length];
            for (var i = 0; i < top.Length; i++)
            {
                topRows[i] = new BattlePointRankingUserMpo
                {
                    Ranking = i + 1,
                    ViewerId = top[i].ViewerId,
                    BattlePoint = top[i].BattlePoint,
                };
                AddUser(resp, top[i]);
            }
            var myRank = ordered.FindIndex(a => a.ViewerId == viewerId) + 1;
            resp.TotalRanking = new BattlePointTotalRankingMpo
            {
                TopRankings = topRows,
                OwnRanking = new BattlePointRankingUserMpo
                {
                    Ranking = myRank,
                    ViewerId = viewerId,
                    BattlePoint = me.BattlePoint,
                },

                IsOwnOutsideTheRanking = myRank <= 0 || myRank > topRows.Length,
            };

            if (dailyAll.Count > 0)
            {
                var daily = new List<BattlePointDailyRankingMpo>();

                foreach (var day in dailyAll.GroupBy(r => r.DateKey).OrderByDescending(g => g.Key))
                {
                    var rows = day.ToList();
                    var topRowsD = new BattlePointRankingUserMpo[Math.Min(rows.Count, TopLimit)];
                    for (var i = 0; i < topRowsD.Length; i++)
                    {
                        topRowsD[i] = new BattlePointRankingUserMpo
                        {
                            Ranking = i + 1,
                            ViewerId = rows[i].ViewerId,
                            BattlePoint = rows[i].Gain,
                        };
                        if (byId.TryGetValue(rows[i].ViewerId, out var a)) AddUser(resp, a);
                    }
                    var ownIdx = rows.FindIndex(r => r.ViewerId == viewerId);
                    daily.Add(new BattlePointDailyRankingMpo
                    {
                        DateKey = day.Key,
                        TopRankings = topRowsD,
                        OwnRanking = new BattlePointRankingUserMpo
                        {
                            Ranking = ownIdx >= 0 ? ownIdx + 1 : 0,
                            ViewerId = viewerId,
                            BattlePoint = ownIdx >= 0 ? rows[ownIdx].Gain : 0,
                        },
                        IsOwnOutsideTheRanking = ownIdx < 0 || ownIdx >= TopLimit,
                    });
                }
                resp.DailyRankings = daily.ToArray();
            }

            resp.OwnUser = ToUser(me);
            resp.Users[viewerId] = resp.OwnUser;

            return resp;
        }

        private static void AddUser(BattlePointRankingResponse resp, AccountRecord a)
        {
            if (a == null || resp.Users.ContainsKey(a.ViewerId)) return;
            resp.Users[a.ViewerId] = ToUser(a);
        }

        private static UserResponse ToUser(AccountRecord a) => new UserResponse
        {
            ViewerId = a.ViewerId,
            Name = a.Name,
            AvatarId = a.AvatarId,
            BoardId = a.BoardId,
            TutorialStep = (TutorialStep)a.TutorialStep,
            CompletedBattleCount = a.CompletedBattleCount,
            Grade = BattleMaster.GradeOf(a.BattlePoint),
            BattlePoint = a.BattlePoint,

            LastLoggedInAt = new DateTimeOffset(DateTime.SpecifyKind(
                a.LastLoggedInAtUtc == default ? DateTime.UnixEpoch : a.LastLoggedInAtUtc,
                DateTimeKind.Utc)),
        };
    }
}
