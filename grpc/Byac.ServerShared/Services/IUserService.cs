using MagicOnion;
using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePack.Formatters;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.Services
{
    [MessagePackFormatter(typeof(UlidMessagePackFormatter))]
    public interface IUserService : IService<IUserService>, IServiceMarker
    {
        UnaryResult<ServiceResult> EditUserNameAsync(string newName);
        UnaryResult<ServiceResult> EditUserBoardAsync(int newBoardId);
        UnaryResult<ServiceResult> EditUserAvatarAsync(int newAvatarId);
        UnaryResult<ServiceResult<UserHomeDataResponse>> GetUserHomeDataAsync();
        UnaryResult<ServiceResult<UserDataAndItemResponse>> GetUserDataAndFurnitureAsync();
        UnaryResult<ServiceResult<CurrentBattleResponse>> GetCurrentBattleAsync();
        UnaryResult<ServiceResult<UserItemResponse>> GetUserItemsAsync();
        UnaryResult<ServiceResult> ReceiveUserFurnitureAsync(int itemId);
        UnaryResult<ServiceResult<UserItemResponse>> GetUserFurnituresAsync();
        UnaryResult<ServiceResult<TutorialStep>> AdvanceTutorialStepAsync();
        UnaryResult<int> Debug(Ulid message);
    }
}
