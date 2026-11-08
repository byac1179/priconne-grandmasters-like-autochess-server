using System;
using System.Collections.Generic;
using System.Text;
using MagicOnion;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.Services
{
    public interface IUserAccountService : IService<IUserAccountService>, IServiceMarker
    {
        UnaryResult<ServiceResult<MasterDataResponse, MasterCacheResult>> GetMasterDataAsync(string hash);
        UnaryResult<ServiceResult<InitialDataResponse>> GetInitialDataAsync();
        UnaryResult<ServiceResult<CreateUserResponse>> CreateUserAsync(string userName, int? previousViewerId);
        UnaryResult<ServiceResult<GetAccessTokenResponse, GetAccessTokenResult>> GetAccessTokenAsync(int viewerId, string authenticationToken);
    }
}
