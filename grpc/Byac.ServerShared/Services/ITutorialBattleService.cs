using MagicOnion;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.ServerShared.Services
{
    public interface ITutorialBattleService : IService<ITutorialBattleService>, IServiceMarker
    {
        UnaryResult<BattleBoardEventResponse[]> GetBattleResponseAsync(TutorialBattleRequest request);
    }
}
