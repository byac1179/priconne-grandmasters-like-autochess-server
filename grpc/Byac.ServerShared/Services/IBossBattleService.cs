using MagicOnion;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.Services
{

    public interface IBossBattleService : IService<IBossBattleService>, IServiceMarker
    {
        UnaryResult<ServiceResult<BossBattleDeckListResponse>> GetDeckListAsync();

        UnaryResult<ServiceResult<BossBattleStartResponse>> BattleStartAsync(int bossBattleDeckId);

        UnaryResult<ServiceResult<BossBattleRemainHpResponse>> GetBossBattleRemainHpAsync();

        UnaryResult<ServiceResult<BossBattleResultResponse>> GetBossBattleResultAsync(Ulid battleId);
    }
}
