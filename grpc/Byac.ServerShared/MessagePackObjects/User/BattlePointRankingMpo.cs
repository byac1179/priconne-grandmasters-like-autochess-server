using System.Collections.Generic;
using MessagePack;

namespace Byac.ServerShared.MessagePackObjects.User
{

    [MessagePackObject]
    public class BattlePointRankingUserMpo
    {
        [Key(0)]
        public int Ranking;

        [Key(1)]
        public int ViewerId;

        [Key(2)]
        public long BattlePoint;

        public BattlePointRankingUserMpo()
        {
        }
    }

    [MessagePackObject]
    public class BattlePointTotalRankingMpo
    {
        [Key(0)]
        public BattlePointRankingUserMpo[] TopRankings;

        [Key(1)]
        public BattlePointRankingUserMpo OwnRanking;

        [Key(2)]
        public bool IsOwnOutsideTheRanking;

        public BattlePointTotalRankingMpo()
        {
        }
    }

    [MessagePackObject]
    public class BattlePointDailyRankingMpo
    {
        [Key(0)]
        public int DateKey;

        [Key(1)]
        public BattlePointRankingUserMpo[] TopRankings;

        [Key(2)]
        public BattlePointRankingUserMpo OwnRanking;

        [Key(3)]
        public bool IsOwnOutsideTheRanking;

        public BattlePointDailyRankingMpo()
        {
        }
    }

    [MessagePackObject]
    public class BattlePointRankingResponse
    {
        [Key(0)]
        public UserResponse OwnUser;

        [Key(1)]
        public Dictionary<int, UserResponse> Users;

        [Key(2)]
        public BattlePointTotalRankingMpo TotalRanking;

        [Key(3)]
        public BattlePointDailyRankingMpo[] DailyRankings;

        public BattlePointRankingResponse()
        {
        }
    }
}
