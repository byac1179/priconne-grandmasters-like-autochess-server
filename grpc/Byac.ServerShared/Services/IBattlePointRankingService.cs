using MagicOnion;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.Services
{

    public interface IBattlePointRankingService : IService<IBattlePointRankingService>, IServiceMarker
    {
        UnaryResult<ServiceResult<BattlePointRankingResponse>> GetBattlePointRankingsAsync();
    }
}
