using System;
using System.IO;
using System.Linq;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Server;
using MessagePack;
using Byac.ServerShared;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class UserAccountService : ServiceBase<IUserAccountService>, IUserAccountService
    {

        public async UnaryResult<ServiceResult<MasterDataResponse, MasterCacheResult>> GetMasterDataAsync(string hash)
        {
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            var result = new ServiceResult<MasterDataResponse, MasterCacheResult>();

            var masterPath = Environment.GetEnvironmentVariable("GSERVER_MASTERDATA")
                ?? Path.Combine(Environment.CurrentDirectory, "masterdata.bytes");

            var masterBytes = AndroidFileBridge.ReadBytes(masterPath);
            if (masterBytes == null)
            {
                Console.WriteLine($"[UserAccount] masterdata missing at {masterPath}; returning NotModified (hash={hash})");
                result.Succeeded = true;
                result.ErrorDetail = MasterCacheResult.NotModified;
                return result;
            }

            try
            {
                MasterDataResponse resp = new MasterDataResponse();
                resp.Data = masterBytes;
                resp.Hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(resp.Data)).ToLowerInvariant();

                result.Result = resp;
                result.Succeeded = true;
                result.ErrorDetail = MasterCacheResult.Success;

                Console.WriteLine($"Received:{hash} master={masterPath} len={resp.Data.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MASTER dump-fail] {ex}");
                result.Succeeded = false;
                result.ErrorDetail = MasterCacheResult.NotModified;
            }

            return result;
        }

        private bool ViaProxy()
        {
            if (Byac.Server.Security.PublicGuardOptions.TlsPort > 0) return true;
            if (!Byac.Server.Security.PublicGuardOptions.TrustProxy) return false;
            foreach (var h in Context.CallContext.RequestHeaders)
                if (string.Equals(h.Key, "x-forwarded-for", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(h.Value)) return true;
            return false;
        }

        public async UnaryResult<ServiceResult<CreateUserResponse>> CreateUserAsync(string userName, int? previousViewerId)
        {
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            Console.WriteLine($"Received:{userName} prev={previousViewerId}");

            var account = await AccountStore.CreateAsync(userName, previousViewerId,
                AccountStore.PeerAddressOf(Context.CallContext.Peer), ViaProxy());
            Console.WriteLine($"[ACC-DONE id={account.ViewerId}]");

            CreateUserResponse createUserResponse = new CreateUserResponse();
            createUserResponse.Name = account.Name;
            createUserResponse.AuthenticationToken = account.AuthenticationToken;
            createUserResponse.ViewerId = account.ViewerId;
            createUserResponse.DeviceIdentifier = account.DeviceIdentifier;

            var result = new ServiceResult<CreateUserResponse>();
            result.Succeeded = true;
            result.Result = createUserResponse;
            result.ErrorDetail = new GeneralError();
            Console.WriteLine("[RESP-RETURN]");
            return result;
        }

        public async UnaryResult<ServiceResult<GetAccessTokenResponse, GetAccessTokenResult>> GetAccessTokenAsync(int viewerId, string authenticationToken)
        {

            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            Console.WriteLine($"Received: Viewer ID {viewerId}, authenticationToken {LogSafe.Secret(authenticationToken)}");

            if (!AccountStore.TryAuthenticate(viewerId, authenticationToken, out var account))
            {
                if (viewerId > 0 && !string.IsNullOrEmpty(authenticationToken))
                {

                    account = AccountStore.AcceptReturningClient(viewerId, authenticationToken,
                        AccountStore.PeerAddressOf(Context.CallContext.Peer));
                    Console.WriteLine($"[UserAccount] GetAccessToken ACCEPT-UNKNOWN viewerId={viewerId} (re-seed)");
                }
                else
                {
                    Console.WriteLine($"[UserAccount] GetAccessToken REJECT viewerId={viewerId}");
                    var fail = new ServiceResult<GetAccessTokenResponse, GetAccessTokenResult>();
                    fail.Succeeded = false;
                    fail.Result = null;
                    fail.ErrorDetail = GetAccessTokenResult.NotFoundOrInvalidToken;
                    return fail;
                }
            }

            GetAccessTokenResponse getAccessTokenResponse = new GetAccessTokenResponse();

            getAccessTokenResponse.AccessToken = account.AuthenticationToken;
            getAccessTokenResponse.Expiration = DateTimeOffset.UtcNow.AddDays(30);
            getAccessTokenResponse.ViewerId = account.ViewerId;
            getAccessTokenResponse.DeviceIdentifier = account.DeviceIdentifier;

            var result = new ServiceResult<GetAccessTokenResponse, GetAccessTokenResult>();
            result.Succeeded = true;
            result.Result = getAccessTokenResponse;
            result.ErrorDetail = GetAccessTokenResult.Success;
            return result;

        }

        public async UnaryResult<ServiceResult<InitialDataResponse>> GetInitialDataAsync()
        {
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            InitialDataResponse initialDataResponse = new InitialDataResponse();
            initialDataResponse.User = new UserResponse();

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var account = AccountStore.GetOrDefault(viewerId);
            initialDataResponse.User.AvatarId = account.AvatarId;
            initialDataResponse.User.ViewerId = account.ViewerId;
            initialDataResponse.User.Name = account.Name;
            initialDataResponse.User.BoardId = account.BoardId;
            initialDataResponse.User.TutorialStep = (TutorialStep)account.TutorialStep;
            initialDataResponse.User.CompletedBattleCount = BattleMaster.MaxAvatarAvailableIndex;
            initialDataResponse.User.BattlePoint = account.BattlePoint;
            initialDataResponse.User.LastLoggedInAt = AccountStore.TouchLogin(account.ViewerId);
            initialDataResponse.User.Grade = BattleMaster.GradeOf(account.BattlePoint);

            initialDataResponse.PrizeFeature = 0;

            var result = new ServiceResult<InitialDataResponse>();
            result.Succeeded = true;
            result.ErrorDetail = new GeneralError();
            result.Result = initialDataResponse;

            Console.WriteLine("INITIAL-DATA-READY");

            return result;
        }
    }
}
