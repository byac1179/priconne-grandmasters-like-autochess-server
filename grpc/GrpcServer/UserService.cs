using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Server;
using Byac.ServerShared;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class UserService : ServiceBase<IUserService>, IUserService
    {
        public async UnaryResult<ServiceResult<CurrentBattleResponse>> GetCurrentBattleAsync()
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);

            var session = viewerId != 0 ? BattleRegistry.FindActiveHumanSession(viewerId) : null;
            var result = new ServiceResult<CurrentBattleResponse>();
            if (session != null)
            {
                result.Succeeded = true;
                result.Result = new CurrentBattleResponse
                {
                    BattleId = session.BattleId,
                    RouteToken = session.RouteToken,
                    Rule = session.Rule
                };
                result.ErrorDetail = new GeneralError();
                Console.WriteLine($"[User] GetCurrentBattle -> active battle={session.BattleId} phase={session.Phase} round={session.Round}");
                return result;
            }

            result.Succeeded = false;
            result.Result = null;
            var err = new GeneralError();
            err.Code = GeneralErrorCode.NotFound;
            err.Reason = "No active battle";
            result.ErrorDetail = err;
            return result;
        }

        public async UnaryResult<ServiceResult<UserHomeDataResponse>> GetUserHomeDataAsync()
        {
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            var resp = new UserHomeDataResponse();

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            resp.User = BuildUser(viewerId);

            var furniture = new UserItemResponse();
            furniture.ViewerId = viewerId;

            furniture.Items = new List<ItemResponse>();
            resp.Furnitures = furniture;

            resp.BossRemainHp = new BossBattleRemainHpResponse
            {
                BossUnitId = BossBattleStore.BossUnitId,
                RemainHp = BossBattleStore.RemainHp(viewerId),
            };
            resp.DeckList = new BossBattleDeckListResponse
            {
                Decks = BossBattleService.DeckList(viewerId),
            };

            resp.ForceDisplayInformations = new InformationListResponse { Informations = InformationStore.Build() };

            resp.RequestedAt = DateTimeOffset.Now.ToUnixTimeSeconds();

            var result = new ServiceResult<UserHomeDataResponse>();
            result.Succeeded = true;
            result.Result = resp;
            result.ErrorDetail = new GeneralError();

            return result;

        }

        public async UnaryResult<ServiceResult> EditUserNameAsync(string newName)
        {
            Console.WriteLine($"[User] EditUserName newName={newName}");

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            if (!AccountStore.TryEditName(viewerId, newName, out _))
            {
                var fail = new ServiceResult { ErrorDetail = new GeneralError() };
                return fail;
            }
            return ServiceResult.Success();
        }

        public async UnaryResult<ServiceResult> EditUserBoardAsync(int newBoardId)
        {
            Console.WriteLine($"[User] EditUserBoard newBoardId={newBoardId}");

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            if (!AccountStore.TryEditBoard(viewerId, newBoardId))
            {
                var fail = new ServiceResult { ErrorDetail = new GeneralError() };
                return fail;
            }
            return ServiceResult.Success();
        }

        public async UnaryResult<ServiceResult> EditUserAvatarAsync(int newAvatarId)
        {
            Console.WriteLine($"[User] EditUserAvatar newAvatarId={newAvatarId}");

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            if (!AccountStore.TryEditAvatar(viewerId, newAvatarId))
            {
                var fail = new ServiceResult { ErrorDetail = new GeneralError() };
                return fail;
            }
            return ServiceResult.Success();
        }

        private UserResponse BuildUser(int viewerId)
        {
            var account = AccountStore.GetOrDefault(viewerId);
            var user = new UserResponse();

            user.AvatarId = account.AvatarId;
            user.ViewerId = account.ViewerId;
            user.Name = account.Name;
            user.BoardId = account.BoardId;
            user.TutorialStep = (TutorialStep)account.TutorialStep;
            user.CompletedBattleCount = account.CompletedBattleCount;
            user.BattlePoint = account.BattlePoint;
            user.LastLoggedInAt = DateTime.Now;
            user.Grade = BattleMaster.GradeOf(account.BattlePoint);
            return user;
        }

        private UserItemResponse BuildItems(int viewerId)
        {

            var resp = new UserItemResponse();
            resp.ViewerId = viewerId;
            resp.Items = new List<ItemResponse>();
            return resp;
        }

        public async UnaryResult<ServiceResult<UserDataAndItemResponse>> GetUserDataAndFurnitureAsync()
        {
            Console.WriteLine("[User] GetUserDataAndFurniture");
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var resp = new UserDataAndItemResponse
            {
                UserData = BuildUser(viewerId),
                Items = BuildItems(viewerId)
            };
            var result = new ServiceResult<UserDataAndItemResponse>();
            result.Succeeded = true;
            result.Result = resp;
            result.ErrorDetail = new GeneralError();
            return result;
        }

        public async UnaryResult<ServiceResult<UserItemResponse>> GetUserItemsAsync()
        {
            Console.WriteLine("[User] GetUserItems");
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<UserItemResponse>();
            result.Succeeded = true;
            result.Result = BuildItems(viewerId);
            result.ErrorDetail = new GeneralError();
            return result;
        }

        public async UnaryResult<ServiceResult<UserItemResponse>> GetUserFurnituresAsync()
        {
            Console.WriteLine("[User] GetUserFurnitures");
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<UserItemResponse>();
            result.Succeeded = true;
            result.Result = BuildItems(viewerId);
            result.ErrorDetail = new GeneralError();
            return result;
        }

        public async UnaryResult<ServiceResult> ReceiveUserFurnitureAsync(int itemId)
        {
            Console.WriteLine($"[User] ReceiveUserFurniture itemId={itemId}");
            return ServiceResult.Success();
        }

        public async UnaryResult<ServiceResult<TutorialStep>> AdvanceTutorialStepAsync()
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<TutorialStep>();

            if (AccountStore.TryAdvanceTutorialStep(viewerId, out var step))
            {
                result.Succeeded = true;
                result.Result = (TutorialStep)step;
                Console.WriteLine($"[User] AdvanceTutorialStep viewer={viewerId} -> {result.Result}");
            }
            else
            {
                result.Succeeded = false;
                result.Result = default;
                result.ErrorDetail = new GeneralError();
                Console.WriteLine($"[User] AdvanceTutorialStep viewer={viewerId} REJECT (dirty TutorialStep)");
            }
            return result;
        }

        public async UnaryResult<int> Debug(Ulid message)
        {
            Console.WriteLine(message.ToString());
            return 0;
        }
    }
}
