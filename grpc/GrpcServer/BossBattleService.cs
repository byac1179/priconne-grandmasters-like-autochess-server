using System;
using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using MagicOnion.Server;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class BossBattleService : ServiceBase<IBossBattleService>, IBossBattleService
    {
        public async UnaryResult<ServiceResult<BossBattleDeckListResponse>> GetDeckListAsync()
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<BossBattleDeckListResponse>
            {
                Succeeded = true,
                ErrorDetail = new GeneralError(),
                Result = new BossBattleDeckListResponse
                {

                    Decks = DeckList(viewerId).ToArray(),
                },
            };
            return result;
        }

        public async UnaryResult<ServiceResult<BossBattleStartResponse>> BattleStartAsync(int bossBattleDeckId)
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<BossBattleStartResponse>();
            var deck = BossBattleStore.DeckList(viewerId).FirstOrDefault(d => d.Id == bossBattleDeckId);
            if (deck == null)
            {
                result.Succeeded = false;
                result.Result = null;
                var err = new GeneralError { Code = GeneralErrorCode.NotFound, Reason = "Boss battle deck not found" };
                result.ErrorDetail = err;
                Console.WriteLine($"[BossBattle] start REJECT viewer={viewerId} deck={bossBattleDeckId} (unknown deck)");
                return result;
            }
            var human = AccountStore.GetOrDefault(viewerId);
            var session = BattleRegistry.CreateBossBattle(human.ViewerId, human.Name, human.AvatarId, deck);
            result.Succeeded = true;
            result.ErrorDetail = new GeneralError();
            result.Result = new BossBattleStartResponse
            {
                BattleId = session.BattleId,
                RouteToken = session.RouteToken,

                BossInfo = new BossInfoMpo { BossUnitId = BossBattleStore.BossUnitId, MaxHp = BossBattleStore.MaxHp() },
            };
            Console.WriteLine($"[BossBattle] start viewer={viewerId} deck={deck.Id} battle={session.BattleId} pool={BossBattleStore.RemainHp(viewerId)}/{BossBattleStore.MaxHp()}");
            return result;
        }

        public async UnaryResult<ServiceResult<BossBattleRemainHpResponse>> GetBossBattleRemainHpAsync()
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var result = new ServiceResult<BossBattleRemainHpResponse>
            {
                Succeeded = true,
                ErrorDetail = new GeneralError(),
                Result = new BossBattleRemainHpResponse
                {
                    BossUnitId = BossBattleStore.BossUnitId,
                    RemainHp = BossBattleStore.RemainHp(viewerId),
                },
            };
            return result;
        }

        public async UnaryResult<ServiceResult<BossBattleResultResponse>> GetBossBattleResultAsync(Ulid battleId)
        {
            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);

            var rec = BossBattleStore.GetResult(viewerId, battleId.ToString());
            var remain = BossBattleStore.RemainHp(viewerId);
            var result = new ServiceResult<BossBattleResultResponse>
            {
                Succeeded = true,
                ErrorDetail = new GeneralError(),
                Result = new BossBattleResultResponse
                {
                    BossUnitId = BossBattleStore.BossUnitId,
                    BossMaxHp = rec?.Max ?? BossBattleStore.MaxHp(),
                    BossRemainHp = rec?.Remain ?? remain,
                    TotalGivedDamage = rec?.Dmg ?? 0,
                },
            };
            Console.WriteLine($"[BossBattle] result viewer={viewerId} battle={battleId} max={result.Result.BossMaxHp} remain={result.Result.BossRemainHp} dmg={result.Result.TotalGivedDamage} (recorded={rec != null})");
            return result;
        }

        public static BossBattleDeckDataMpo[] DeckList(int viewerId)
        {
            var decks = BossBattleStore.DeckList(viewerId);
            var list = new List<BossBattleDeckDataMpo>(decks.Count);
            for (int i = 0; i < decks.Count; i++)
            {
                var d = decks[i];
                DateTimeOffset display;
                try { display = DateTimeOffset.Parse(d.Utc, global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind); }
                catch { display = DateTimeOffset.UtcNow; }
                list.Add(new BossBattleDeckDataMpo
                {
                    DeckId = d.Id,
                    Ranking = d.Rank,
                    PartyPower = d.Power,
                    TotalGiveDamage = d.Damage,
                    PlayerLevel = d.Level,
                    AvatarId = d.Avatar,
                    DisplayTime = display,
                    RoleSynergies = d.Roles.Select(r => ValueTuple.Create(r.Id, r.Count)).ToList(),
                    TeamSynergies = d.Teams.Select(r => ValueTuple.Create(r.Id, r.Count)).ToList(),
                    Units = d.Units.Select((u, k) => new BossBattleDeckUnitDataMpo
                    {
                        Seq = k + 1,
                        UniqueKey = u.Key,
                        UnitId = u.UnitId,
                        Rank = u.Rank,
                        PositionX = u.X,
                        PositionY = u.Y,
                        EquipmentIds = u.Equips ?? Array.Empty<int>(),
                        FieldDeployOrder = k + 1,
                        MirroredUnitKey = null,
                        TotalSkillCount = 0,
                    }).ToArray(),
                    IsAvailable = true,
                    DeckNumber = i + 1,
                });
            }
            return list.ToArray();
        }
    }
}
