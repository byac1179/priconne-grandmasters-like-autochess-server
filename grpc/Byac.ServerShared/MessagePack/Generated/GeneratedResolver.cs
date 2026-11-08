#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1312
#pragma warning disable SA1649

namespace MessagePack.Resolvers
{
    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(178)
            {
                { typeof((global::Byac.ServerShared.Enums.BattleResultType, int)), 0 },
                { typeof(global::Byac.ServerShared.Enums.BattlePhase?), 1 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse[]), 2 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]), 3 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo?), 4 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]), 5 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo[]), 6 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo[]), 7 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo[]), 8 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo[]), 9 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo[]), 10 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]), 11 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo[]), 12 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]), 13 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo[]), 14 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo[]), 15 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo[]), 16 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo[]), 17 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse[]), 18 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo[]), 19 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo[]), 20 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse[]), 21 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase[]), 22 },
                { typeof(global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo>), 23 },
                { typeof(global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>), 24 },
                { typeof(global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.RewardDataMpo>), 25 },
                { typeof(global::System.Collections.Generic.Dictionary<int, int>), 26 },
                { typeof(global::System.Collections.Generic.HashSet<int>), 27 },
                { typeof(global::System.Collections.Generic.IReadOnlyDictionary<int, int>), 28 },
                { typeof(global::System.Collections.Generic.IReadOnlyList<float>), 29 },
                { typeof(global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo>), 30 },
                { typeof(global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse>), 31 },
                { typeof(global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.User.ItemResponse>), 32 },
                { typeof(global::System.Collections.Generic.IReadOnlyList<int>), 33 },
                { typeof(global::System.Collections.Generic.List<float>), 34 },
                { typeof(global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>), 35 },
                { typeof(global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>), 36 },
                { typeof(global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>), 37 },
                { typeof(global::System.Collections.Generic.List<int>), 38 },
                { typeof(global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>), 39 },
                { typeof(global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>), 40 },
                { typeof(global::Byac.ServerShared.Enums.AttackType), 41 },
                { typeof(global::Byac.ServerShared.Enums.BattleOption), 42 },
                { typeof(global::Byac.ServerShared.Enums.BattlePhase), 43 },
                { typeof(global::Byac.ServerShared.Enums.BattleResultType), 44 },
                { typeof(global::Byac.ServerShared.Enums.BattleRule), 45 },
                { typeof(global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState), 46 },
                { typeof(global::Byac.ServerShared.Enums.DamageType), 47 },
                { typeof(global::Byac.ServerShared.Enums.EquipmentType), 48 },
                { typeof(global::Byac.ServerShared.Enums.FieldCellGimmickType), 49 },
                { typeof(global::Byac.ServerShared.Enums.GeneralErrorCode), 50 },
                { typeof(global::Byac.ServerShared.Enums.GetAccessTokenResult), 51 },
                { typeof(global::Byac.ServerShared.Enums.GuildId), 52 },
                { typeof(global::Byac.ServerShared.Enums.InformationType), 53 },
                { typeof(global::Byac.ServerShared.Enums.StatusEffectVfxType), 54 },
                { typeof(global::Byac.ServerShared.Enums.SynergyType), 55 },
                { typeof(global::Byac.ServerShared.Enums.TutorialStep), 56 },
                { typeof(global::Byac.ServerShared.Enums.UnitState), 57 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle), 58 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE), 59 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE), 60 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse), 61 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase), 62 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleAttachResponse), 63 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse), 64 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse), 65 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse), 66 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse), 67 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse), 68 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse), 69 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse), 70 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse), 71 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse), 72 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse), 73 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse), 74 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse), 75 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse), 76 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse), 77 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse), 78 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce), 79 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse), 80 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse), 81 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce), 82 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse), 83 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse), 84 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse), 85 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse), 86 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse), 87 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse), 88 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse), 89 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse), 90 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse), 91 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse), 92 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse), 93 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse), 94 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse), 95 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse), 96 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse), 97 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse), 98 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse), 99 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse), 100 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse), 101 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse), 102 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse), 103 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse), 104 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse), 105 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse), 106 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo), 107 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo), 108 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo), 109 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo), 110 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpo), 111 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpo), 112 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo), 113 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo), 114 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpo), 115 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo), 116 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo), 117 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo), 118 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpo), 119 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo), 120 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo), 121 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo), 122 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo), 123 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo), 124 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo), 125 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo), 126 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo), 127 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo), 128 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse), 129 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo), 130 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse), 131 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BossRoundStartResponse), 132 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpo), 133 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo), 134 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponse), 135 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo), 136 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse), 137 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo), 138 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.InformationListResponse), 139 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequest), 140 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponse), 141 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponse), 142 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequest), 143 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponse), 144 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponse), 145 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo), 146 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo), 147 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse), 148 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo), 149 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpo), 150 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpo), 151 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse), 152 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponse), 153 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly), 154 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponse), 155 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo), 156 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.RemainTimeResponse), 157 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.ResumeResponse), 158 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.RewardDataMpo), 159 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.SetupRoundResponse), 160 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UnitData), 161 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo), 162 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.CreateUserResponse), 163 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponse), 164 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponse), 165 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.InitialDataResponse), 166 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.ItemResponse), 167 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.MasterDataResponse), 168 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponse), 169 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponse), 170 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse), 171 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.User.UserResponse), 172 },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UserReconnectResponse), 173 },
                { typeof(global::Byac.ServerShared.Primitives.AuthenticationToken), 174 },
                { typeof(global::Byac.ServerShared.Primitives.GeneralError), 175 },
                { typeof(global::Byac.ServerShared.Primitives.ServiceResult), 176 },
                { typeof(global::Byac.ServerShared.Primitives.ServiceResultVoid), 177 },
            };
        }

        internal static object GetFormatter(global::System.Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ValueTupleFormatter<global::Byac.ServerShared.Enums.BattleResultType, int>();
                case 1: return new global::MessagePack.Formatters.NullableFormatter<global::Byac.ServerShared.Enums.BattlePhase>();
                case 2: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse>();
                case 3: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>();
                case 4: return new global::MessagePack.Formatters.NullableFormatter<global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo>();
                case 5: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>();
                case 6: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo>();
                case 7: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo>();
                case 8: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo>();
                case 9: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo>();
                case 10: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>();
                case 11: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo>();
                case 12: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo>();
                case 13: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>();
                case 14: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo>();
                case 15: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo>();
                case 16: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo>();
                case 17: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>();
                case 18: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse>();
                case 19: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo>();
                case 20: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo>();
                case 21: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse>();
                case 22: return new global::MessagePack.Formatters.ArrayFormatter<global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase>();
                case 23: return new global::MessagePack.Formatters.DictionaryFormatter<int, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo>();
                case 24: return new global::MessagePack.Formatters.DictionaryFormatter<int, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>();
                case 25: return new global::MessagePack.Formatters.DictionaryFormatter<int, global::Byac.ServerShared.MessagePackObjects.RewardDataMpo>();
                case 26: return new global::MessagePack.Formatters.DictionaryFormatter<int, int>();
                case 27: return new global::MessagePack.Formatters.HashSetFormatter<int>();
                case 28: return new global::MessagePack.Formatters.InterfaceReadOnlyDictionaryFormatter<int, int>();
                case 29: return new global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<float>();
                case 30: return new global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo>();
                case 31: return new global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse>();
                case 32: return new global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<global::Byac.ServerShared.MessagePackObjects.User.ItemResponse>();
                case 33: return new global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<int>();
                case 34: return new global::MessagePack.Formatters.ListFormatter<float>();
                case 35: return new global::MessagePack.Formatters.ListFormatter<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>();
                case 36: return new global::MessagePack.Formatters.ListFormatter<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>();
                case 37: return new global::MessagePack.Formatters.ListFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>();
                case 38: return new global::MessagePack.Formatters.ListFormatter<int>();
                case 39: return new global::MessagePack.ImmutableCollection.ImmutableHashSetFormatter<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>();
                case 40: return new global::MessagePack.ImmutableCollection.ImmutableHashSetFormatter<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>();
                case 41: return new MessagePack.Formatters.Byac.ServerShared.Enums.AttackTypeFormatter();
                case 42: return new MessagePack.Formatters.Byac.ServerShared.Enums.BattleOptionFormatter();
                case 43: return new MessagePack.Formatters.Byac.ServerShared.Enums.BattlePhaseFormatter();
                case 44: return new MessagePack.Formatters.Byac.ServerShared.Enums.BattleResultTypeFormatter();
                case 45: return new MessagePack.Formatters.Byac.ServerShared.Enums.BattleRuleFormatter();
                case 46: return new MessagePack.Formatters.Byac.ServerShared.Enums.BoardUnitStatusSpecialStateFormatter();
                case 47: return new MessagePack.Formatters.Byac.ServerShared.Enums.DamageTypeFormatter();
                case 48: return new MessagePack.Formatters.Byac.ServerShared.Enums.EquipmentTypeFormatter();
                case 49: return new MessagePack.Formatters.Byac.ServerShared.Enums.FieldCellGimmickTypeFormatter();
                case 50: return new MessagePack.Formatters.Byac.ServerShared.Enums.GeneralErrorCodeFormatter();
                case 51: return new MessagePack.Formatters.Byac.ServerShared.Enums.GetAccessTokenResultFormatter();
                case 52: return new MessagePack.Formatters.Byac.ServerShared.Enums.GuildIdFormatter();
                case 53: return new MessagePack.Formatters.Byac.ServerShared.Enums.InformationTypeFormatter();
                case 54: return new MessagePack.Formatters.Byac.ServerShared.Enums.StatusEffectVfxTypeFormatter();
                case 55: return new MessagePack.Formatters.Byac.ServerShared.Enums.SynergyTypeFormatter();
                case 56: return new MessagePack.Formatters.Byac.ServerShared.Enums.TutorialStepFormatter();
                case 57: return new MessagePack.Formatters.Byac.ServerShared.Enums.UnitStateFormatter();
                case 58: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggleFormatter();
                case 59: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.UnitData_ROLE_TYPEFormatter();
                case 60: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.UnitData_TEAM_TYPEFormatter();
                case 61: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardEventResponseFormatter();
                case 62: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBaseFormatter();
                case 63: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleAttachResponseFormatter();
                case 64: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponseFormatter();
                case 65: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponseFormatter();
                case 66: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponseFormatter();
                case 67: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponseFormatter();
                case 68: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponseFormatter();
                case 69: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponseFormatter();
                case 70: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponseFormatter();
                case 71: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponseFormatter();
                case 72: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponseFormatter();
                case 73: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponseFormatter();
                case 74: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponseFormatter();
                case 75: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponseFormatter();
                case 76: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponseFormatter();
                case 77: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponseFormatter();
                case 78: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponseFormatter();
                case 79: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponceFormatter();
                case 80: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponseFormatter();
                case 81: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponseFormatter();
                case 82: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponceFormatter();
                case 83: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponseFormatter();
                case 84: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponseFormatter();
                case 85: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponseFormatter();
                case 86: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponseFormatter();
                case 87: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponseFormatter();
                case 88: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponseFormatter();
                case 89: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponseFormatter();
                case 90: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponseFormatter();
                case 91: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponseFormatter();
                case 92: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponseFormatter();
                case 93: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponseFormatter();
                case 94: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse_SkillActionResultResponseFormatter();
                case 95: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponseFormatter();
                case 96: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponseFormatter();
                case 97: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponseFormatter();
                case 98: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponseFormatter();
                case 99: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponseFormatter();
                case 100: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponseFormatter();
                case 101: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponseFormatter();
                case 102: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponseFormatter();
                case 103: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponseFormatter();
                case 104: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponseFormatter();
                case 105: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponseFormatter();
                case 106: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponseFormatter();
                case 107: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpoFormatter();
                case 108: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpoFormatter();
                case 109: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpoFormatter();
                case 110: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpoFormatter();
                case 111: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpoFormatter();
                case 112: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpoFormatter();
                case 113: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpoFormatter();
                case 114: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleResultInfoMpoFormatter();
                case 115: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpoFormatter();
                case 116: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpoFormatter();
                case 117: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpoFormatter();
                case 118: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleRoundResultMpoFormatter();
                case 119: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpoFormatter();
                case 120: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleSynergyMpoFormatter();
                case 121: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleTeamDataMpoFormatter();
                case 122: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpoFormatter();
                case 123: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpoFormatter();
                case 124: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleUnitDataMpoFormatter();
                case 125: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpoFormatter();
                case 126: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpoFormatter();
                case 127: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpoFormatter();
                case 128: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpoFormatter();
                case 129: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponseFormatter();
                case 130: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpoFormatter();
                case 131: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponseFormatter();
                case 132: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.BossRoundStartResponseFormatter();
                case 133: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpoFormatter();
                case 134: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpoFormatter();
                case 135: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponseFormatter();
                case 136: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.FieldCellDataMpoFormatter();
                case 137: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponseFormatter();
                case 138: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.InformationListItemMpoFormatter();
                case 139: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.InformationListResponseFormatter();
                case 140: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequestFormatter();
                case 141: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponseFormatter();
                case 142: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponseFormatter();
                case 143: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequestFormatter();
                case 144: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponseFormatter();
                case 145: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponseFormatter();
                case 146: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpoFormatter();
                case 147: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpoFormatter();
                case 148: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.NpcRoundStartResponseFormatter();
                case 149: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpoFormatter();
                case 150: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpoFormatter();
                case 151: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpoFormatter();
                case 152: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponseFormatter();
                case 153: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponseFormatter();
                case 154: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnlyFormatter();
                case 155: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponseFormatter();
                case 156: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpoFormatter();
                case 157: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.RemainTimeResponseFormatter();
                case 158: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.ResumeResponseFormatter();
                case 159: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.RewardDataMpoFormatter();
                case 160: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.SetupRoundResponseFormatter();
                case 161: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.UnitDataFormatter();
                case 162: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpoFormatter();
                case 163: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.CreateUserResponseFormatter();
                case 164: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponseFormatter();
                case 165: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponseFormatter();
                case 166: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.InitialDataResponseFormatter();
                case 167: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.ItemResponseFormatter();
                case 168: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.MasterDataResponseFormatter();
                case 169: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponseFormatter();
                case 170: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponseFormatter();
                case 171: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.UserItemResponseFormatter();
                case 172: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User.UserResponseFormatter();
                case 173: return new MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.UserReconnectResponseFormatter();
                case 174: return new MessagePack.Formatters.Byac.ServerShared.Primitives.AuthenticationTokenFormatter();
                case 175: return new MessagePack.Formatters.Byac.ServerShared.Primitives.GeneralErrorFormatter();
                case 176: return new MessagePack.Formatters.Byac.ServerShared.Primitives.ServiceResultFormatter();
                case 177: return new MessagePack.Formatters.Byac.ServerShared.Primitives.ServiceResultVoidFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.Enums
{

    public sealed class AttackTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.AttackType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.AttackType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.AttackType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.AttackType)reader.ReadInt32();
        }
    }

    public sealed class BattleOptionFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.BattleOption>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.BattleOption value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.BattleOption Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.BattleOption)reader.ReadInt32();
        }
    }

    public sealed class BattlePhaseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.BattlePhase>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.BattlePhase value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.BattlePhase Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.BattlePhase)reader.ReadInt32();
        }
    }

    public sealed class BattleResultTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.BattleResultType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.BattleResultType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.BattleResultType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.BattleResultType)reader.ReadInt32();
        }
    }

    public sealed class BattleRuleFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.BattleRule>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.BattleRule value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.BattleRule Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.BattleRule)reader.ReadInt32();
        }
    }

    public sealed class BoardUnitStatusSpecialStateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState)reader.ReadInt32();
        }
    }

    public sealed class DamageTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.DamageType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.DamageType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.DamageType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.DamageType)reader.ReadInt32();
        }
    }

    public sealed class EquipmentTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.EquipmentType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.EquipmentType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.EquipmentType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.EquipmentType)reader.ReadInt32();
        }
    }

    public sealed class FieldCellGimmickTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.FieldCellGimmickType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.FieldCellGimmickType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.FieldCellGimmickType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.FieldCellGimmickType)reader.ReadInt32();
        }
    }

    public sealed class GeneralErrorCodeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.GeneralErrorCode>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.GeneralErrorCode value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.GeneralErrorCode Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.GeneralErrorCode)reader.ReadInt32();
        }
    }

    public sealed class GetAccessTokenResultFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.GetAccessTokenResult>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.GetAccessTokenResult value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.GetAccessTokenResult Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.GetAccessTokenResult)reader.ReadInt32();
        }
    }

    public sealed class GuildIdFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.GuildId>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.GuildId value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.GuildId Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.GuildId)reader.ReadInt32();
        }
    }

    public sealed class InformationTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.InformationType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.InformationType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.InformationType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.InformationType)reader.ReadInt32();
        }
    }

    public sealed class StatusEffectVfxTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.StatusEffectVfxType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.StatusEffectVfxType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.StatusEffectVfxType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.StatusEffectVfxType)reader.ReadInt32();
        }
    }

    public sealed class SynergyTypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.SynergyType>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.SynergyType value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.SynergyType Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.SynergyType)reader.ReadInt32();
        }
    }

    public sealed class TutorialStepFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.TutorialStep>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.TutorialStep value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.TutorialStep Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.TutorialStep)reader.ReadInt32();
        }
    }

    public sealed class UnitStateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Enums.UnitState>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Enums.UnitState value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.Enums.UnitState Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.Enums.UnitState)reader.ReadInt32();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize
{

    public sealed class PrizeFeatureToggleFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle)reader.ReadInt32();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects
{

    public sealed class UnitData_ROLE_TYPEFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE)reader.ReadInt32();
        }
    }

    public sealed class UnitData_TEAM_TYPEFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE)reader.ReadInt32();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects
{
    public sealed class BattleBoardEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse>
    {
        private readonly global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>> typeToKeyAndJumpMap;
        private readonly global::System.Collections.Generic.Dictionary<int, int> keyToJumpMap;

        public BattleBoardEventResponseFormatter()
        {
            this.typeToKeyAndJumpMap = new global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>>(42, global::MessagePack.Internal.RuntimeTypeHandleEqualityComparer.Default)
            {
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(0, 0) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(1, 1) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(2, 2) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(3, 3) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(4, 4) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(5, 5) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(6, 6) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(7, 7) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(8, 8) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(9, 9) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(10, 10) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(11, 11) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(12, 12) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(13, 13) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(14, 14) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(15, 15) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(16, 16) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(17, 17) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(18, 18) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(19, 19) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(20, 20) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(21, 21) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(22, 22) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(23, 23) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(24, 24) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(25, 25) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(26, 26) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(27, 27) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(28, 28) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(29, 29) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(30, 30) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(31, 31) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(32, 32) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(33, 33) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(34, 34) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(35, 35) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(36, 36) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(37, 37) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(38, 38) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(39, 39) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(40, 40) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(41, 41) },
            };
            this.keyToJumpMap = new global::System.Collections.Generic.Dictionary<int, int>(42)
            {
                { 0, 0 },
                { 1, 1 },
                { 2, 2 },
                { 3, 3 },
                { 4, 4 },
                { 5, 5 },
                { 6, 6 },
                { 7, 7 },
                { 8, 8 },
                { 9, 9 },
                { 10, 10 },
                { 11, 11 },
                { 12, 12 },
                { 13, 13 },
                { 14, 14 },
                { 15, 15 },
                { 16, 16 },
                { 17, 17 },
                { 18, 18 },
                { 19, 19 },
                { 20, 20 },
                { 21, 21 },
                { 22, 22 },
                { 23, 23 },
                { 24, 24 },
                { 25, 25 },
                { 26, 26 },
                { 27, 27 },
                { 28, 28 },
                { 29, 29 },
                { 30, 30 },
                { 31, 31 },
                { 32, 32 },
                { 33, 33 },
                { 34, 34 },
                { 35, 35 },
                { 36, 36 },
                { 37, 37 },
                { 38, 38 },
                { 39, 39 },
                { 40, 40 },
                { 41, 41 },
            };
        }

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::System.Collections.Generic.KeyValuePair<int, int> keyValuePair;
            if (value != null && this.typeToKeyAndJumpMap.TryGetValue(value.GetType().TypeHandle, out keyValuePair))
            {
                writer.WriteArrayHeader(2);
                writer.WriteInt32(keyValuePair.Key);
                switch (keyValuePair.Value)
                {
                    case 0:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse)value, options);
                        break;
                    case 1:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse)value, options);
                        break;
                    case 2:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse)value, options);
                        break;
                    case 3:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse)value, options);
                        break;
                    case 4:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse)value, options);
                        break;
                    case 5:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse)value, options);
                        break;
                    case 6:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse)value, options);
                        break;
                    case 7:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse)value, options);
                        break;
                    case 8:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse)value, options);
                        break;
                    case 9:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse)value, options);
                        break;
                    case 10:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse)value, options);
                        break;
                    case 11:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse)value, options);
                        break;
                    case 12:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse)value, options);
                        break;
                    case 13:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse)value, options);
                        break;
                    case 14:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse)value, options);
                        break;
                    case 15:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse)value, options);
                        break;
                    case 16:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse)value, options);
                        break;
                    case 17:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse)value, options);
                        break;
                    case 18:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse)value, options);
                        break;
                    case 19:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse)value, options);
                        break;
                    case 20:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse)value, options);
                        break;
                    case 21:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse)value, options);
                        break;
                    case 22:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse)value, options);
                        break;
                    case 23:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse)value, options);
                        break;
                    case 24:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse)value, options);
                        break;
                    case 25:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse)value, options);
                        break;
                    case 26:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse)value, options);
                        break;
                    case 27:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse)value, options);
                        break;
                    case 28:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse)value, options);
                        break;
                    case 29:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse)value, options);
                        break;
                    case 30:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse)value, options);
                        break;
                    case 31:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse)value, options);
                        break;
                    case 32:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce)value, options);
                        break;
                    case 33:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce)value, options);
                        break;
                    case 34:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse)value, options);
                        break;
                    case 35:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse)value, options);
                        break;
                    case 36:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse)value, options);
                        break;
                    case 37:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse)value, options);
                        break;
                    case 38:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse)value, options);
                        break;
                    case 39:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse)value, options);
                        break;
                    case 40:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse)value, options);
                        break;
                    case 41:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse)value, options);
                        break;
                    default:
                        break;
                }

                return;
            }

            writer.WriteNil();
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            if (reader.ReadArrayHeader() != 2)
            {
                throw new global::System.InvalidOperationException("Invalid Union data was detected. Type:global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse");
            }

            options.Security.DepthStep(ref reader);
            var key = reader.ReadInt32();

            if (!this.keyToJumpMap.TryGetValue(key, out key))
            {
                key = -1;
            }

            global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse result = null;
            switch (key)
            {
                case 0:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 1:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 2:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 3:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 4:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 5:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 6:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 7:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 8:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 9:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 10:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 11:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 12:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 13:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 14:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 15:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 16:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 17:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 18:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 19:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 20:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 21:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 22:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 23:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 24:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 25:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 26:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 27:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 28:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 29:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 30:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 31:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 32:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 33:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 34:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 35:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 36:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 37:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 38:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 39:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 40:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 41:
                    result = (global::Byac.ServerShared.MessagePackObjects.BattleBoardEventResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                default:
                    reader.Skip();
                    break;
            }

            reader.Depth--;
            return result;
        }
    }

    public sealed class PickupItemDataMpoBaseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase>
    {
        private readonly global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>> typeToKeyAndJumpMap;
        private readonly global::System.Collections.Generic.Dictionary<int, int> keyToJumpMap;

        public PickupItemDataMpoBaseFormatter()
        {
            this.typeToKeyAndJumpMap = new global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>>(4, global::MessagePack.Internal.RuntimeTypeHandleEqualityComparer.Default)
            {
                { typeof(global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(0, 0) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(1, 1) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(2, 2) },
                { typeof(global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(3, 3) },
            };
            this.keyToJumpMap = new global::System.Collections.Generic.Dictionary<int, int>(4)
            {
                { 0, 0 },
                { 1, 1 },
                { 2, 2 },
                { 3, 3 },
            };
        }

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::System.Collections.Generic.KeyValuePair<int, int> keyValuePair;
            if (value != null && this.typeToKeyAndJumpMap.TryGetValue(value.GetType().TypeHandle, out keyValuePair))
            {
                writer.WriteArrayHeader(2);
                writer.WriteInt32(keyValuePair.Key);
                switch (keyValuePair.Value)
                {
                    case 0:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo)value, options);
                        break;
                    case 1:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo)value, options);
                        break;
                    case 2:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo)value, options);
                        break;
                    case 3:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo>(options.Resolver).Serialize(ref writer, (global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo)value, options);
                        break;
                    default:
                        break;
                }

                return;
            }

            writer.WriteNil();
        }

        public global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            if (reader.ReadArrayHeader() != 2)
            {
                throw new global::System.InvalidOperationException("Invalid Union data was detected. Type:global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase");
            }

            options.Security.DepthStep(ref reader);
            var key = reader.ReadInt32();

            if (!this.keyToJumpMap.TryGetValue(key, out key))
            {
                key = -1;
            }

            global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase result = null;
            switch (key)
            {
                case 0:
                    result = (global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 1:
                    result = (global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 2:
                    result = (global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo>(options.Resolver).Deserialize(ref reader, options);
                    break;
                case 3:
                    result = (global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo>(options.Resolver).Deserialize(ref reader, options);
                    break;
                default:
                    reader.Skip();
                    break;
            }

            reader.Depth--;
            return result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1129
#pragma warning disable SA1309
#pragma warning disable SA1312
#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects
{
    public sealed class BattleAttachResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleAttachResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleAttachResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.Write(value.AttachedViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Serialize(ref writer, value.Players, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Serialize(ref writer, value.Teams, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo?>(formatterResolver).Serialize(ref writer, value.GuildBuffData, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattlePhase?>(formatterResolver).Serialize(ref writer, value.CurrentPhase, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleAttachResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleAttachResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.AttachedViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Players = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Teams = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.GuildBuffData = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.CurrentPhase = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattlePhase?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardAddFieldUnitEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.AddUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardAddFieldUnitEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.AddUnits = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.CalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardAttachBarrierEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.SkillActionId);
            writer.Write(value.IsBarrierEnd);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardAttachBarrierEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.SkillActionId = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.IsBarrierEnd = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardAttackOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(11);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Damage);
            writer.Write(value.Life);
            writer.Write(value.IsCritical);
            writer.Write(value.HavePassiveSkillEffect);
            writer.Write(value.IsDead);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Serialize(ref writer, value.DamageType, options);
            writer.Write(value.PreviousLife);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardAttackOperateEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.Damage = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.Life = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.IsCritical = reader.ReadBoolean();
                        break;
                    case 7:
                        ____result.HavePassiveSkillEffect = reader.ReadBoolean();
                        break;
                    case 8:
                        ____result.IsDead = reader.ReadBoolean();
                        break;
                    case 9:
                        ____result.DamageType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 10:
                        ____result.PreviousLife = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardBarrierDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.MaxBufferedDamage);
            writer.Write(value.BufferedDamage);
            writer.Write(value.Damage);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Serialize(ref writer, value.DamageType, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierDamageEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.OperatorUniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.MaxBufferedDamage = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.BufferedDamage = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.Damage = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.DamageType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardBarrierEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(5);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.MaxBufferedDamage);
            writer.Write(value.BufferedDamage);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardBarrierEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.MaxBufferedDamage = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.BufferedDamage = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardBlowAwayOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.PreviousX);
            writer.Write(value.PreviousY);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardBlowAwayOperateEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.X = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.Y = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.PreviousX = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.PreviousY = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardBoxDropEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo[]>(formatterResolver).Serialize(ref writer, value.Boxes, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __Boxes__ = default(global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo[]);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __Boxes__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardBoxDropEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __Boxes__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardChangeTechnicalPointEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TechnicalPoint);
            writer.Write(value.Tp);
            writer.Write(value.IsSynergy);
            writer.Write(value.PreviousTp);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __TechnicalPoint__ = default(int);
            var __Tp__ = default(int);
            var __IsSynergy__ = default(bool);
            var __PreviousTp__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TechnicalPoint__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Tp__ = reader.ReadInt32();
                        break;
                    case 5:
                        __IsSynergy__ = reader.ReadBoolean();
                        break;
                    case 6:
                        __PreviousTp__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardChangeTechnicalPointEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __TechnicalPoint__, __Tp__, __IsSynergy__, __PreviousTp__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardCompleteEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(10);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.LastMatchInRound);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Serialize(ref writer, value.PrimaryResult, options);
            writer.Write(value.PrimaryDamage);
            writer.Write(value.PrimaryLife);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Serialize(ref writer, value.SecondaryResult, options);
            writer.Write(value.SecondaryDamage);
            writer.Write(value.SecondaryLife);
            writer.Write(value.TimedOut);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardCompleteEventResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RaisedAt = reader.ReadSingle();
                        break;
                    case 2:
                        ____result.LastMatchInRound = reader.ReadBoolean();
                        break;
                    case 3:
                        ____result.PrimaryResult = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.PrimaryDamage = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.PrimaryLife = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.SecondaryResult = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        ____result.SecondaryDamage = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.SecondaryLife = reader.ReadInt32();
                        break;
                    case 9:
                        ____result.TimedOut = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardCounterSkillDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(12);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Damage);
            writer.Write(value.CounterBeforeMove);
            writer.Write(value.SkillId);
            writer.Write(value.IsDead);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Serialize(ref writer, value.DamageType, options);
            writer.Write(value.PreviousLife);
            writer.Write(value.IsCounterDamage);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Damage__ = default(int);
            var __CounterBeforeMove__ = default(bool);
            var __SkillId__ = default(int);
            var __IsDead__ = default(bool);
            var __DamageType__ = default(global::Byac.ServerShared.Enums.DamageType);
            var __PreviousLife__ = default(int);
            var __IsCounterDamage__ = default(bool);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 6:
                        __CounterBeforeMove__ = reader.ReadBoolean();
                        break;
                    case 7:
                        __SkillId__ = reader.ReadInt32();
                        break;
                    case 8:
                        __IsDead__ = reader.ReadBoolean();
                        break;
                    case 9:
                        __DamageType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 10:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    case 11:
                        __IsCounterDamage__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Life__, __Damage__, __CounterBeforeMove__, __SkillId__, __IsDead__, __DamageType__, __PreviousLife__, __IsCounterDamage__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardCounterSkillOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.SkillActionId);
            writer.Write(value.IsEnd);
            writer.Write(value.IsCancel);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __SkillActionId__ = default(int);
            var __IsEnd__ = default(bool);
            var __IsCancel__ = default(bool);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    case 4:
                        __IsEnd__ = reader.ReadBoolean();
                        break;
                    case 5:
                        __IsCancel__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardCounterSkillOperateEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __SkillActionId__, __IsEnd__, __IsCancel__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardDebugMessageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Message, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __Message__ = default(string);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __Message__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardDebugMessageEventResponse(__MatchId__, __RaisedAt__, __Message__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardEmptySkillOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.SkillActionId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __SkillActionId__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardEmptySkillOperateEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __SkillActionId__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardEquipEquipmentHasPassiveSkillEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUnitUniqueKey);
            writer.Write(value.EquipmentUniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUnitUniqueKey__ = default(int);
            var __EquipmentUniqueKey__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUnitUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __EquipmentUniqueKey__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipEquipmentHasPassiveSkillEventResponse(__MatchId__, __RaisedAt__, __OperatorUnitUniqueKey__, __EquipmentUniqueKey__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardEquipmentSkillDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(11);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUnitUniqueKey);
            writer.Write(value.EquipmentUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Damage);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Serialize(ref writer, value.DamageType, options);
            writer.Write(value.SkillId);
            writer.Write(value.IsDead);
            writer.Write(value.PreviousLife);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __OperatorUnitUniqueKey__ = default(int);
            var __EquipmentUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Damage__ = default(int);
            var __DamageType__ = default(global::Byac.ServerShared.Enums.DamageType);
            var __SkillId__ = default(int);
            var __IsDead__ = default(bool);
            var __PreviousLife__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUnitUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __EquipmentUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 6:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 7:
                        __DamageType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 8:
                        __SkillId__ = reader.ReadInt32();
                        break;
                    case 9:
                        __IsDead__ = reader.ReadBoolean();
                        break;
                    case 10:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardEquipmentSkillDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUnitUniqueKey__, __EquipmentUniqueKey__, __TargetUniqueKey__, __Life__, __Damage__, __DamageType__, __SkillId__, __IsDead__, __PreviousLife__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardGoOutGimmickFieldCellEventResponceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>(formatterResolver).Serialize(ref writer, value.FieldCellDataMpo, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __FieldCellDataMpo__ = default(global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __FieldCellDataMpo__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardGoOutGimmickFieldCellEventResponce(__MatchId__, __RaisedAt__, __UniqueKey__, __FieldCellDataMpo__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardHpDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(9);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Damage);
            writer.Write(value.IsDead);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.AttackType>(formatterResolver).Serialize(ref writer, value.AttackType, options);
            writer.Write(value.PreviousLife);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Damage__ = default(int);
            var __IsDead__ = default(bool);
            var __AttackType__ = default(global::Byac.ServerShared.Enums.AttackType);
            var __PreviousLife__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 6:
                        __IsDead__ = reader.ReadBoolean();
                        break;
                    case 7:
                        __AttackType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.AttackType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 8:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardHpDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Life__, __Damage__, __IsDead__, __AttackType__, __PreviousLife__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardHpHealEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(9);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Heal);
            writer.Write(value.PreviousLife);
            writer.Write(value.ForceHeal);
            writer.Write(value.EmitOnDead);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Heal__ = default(int);
            var __PreviousLife__ = default(int);
            var __ForceHeal__ = default(bool);
            var __EmitOnDead__ = default(bool);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Heal__ = reader.ReadInt32();
                        break;
                    case 6:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    case 7:
                        __ForceHeal__ = reader.ReadBoolean();
                        break;
                    case 8:
                        __EmitOnDead__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardHpHealEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __TargetUniqueKey__, __Life__, __Heal__, __PreviousLife__, __ForceHeal__, __EmitOnDead__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardIntoGimmickFieldCellEventResponceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>(formatterResolver).Serialize(ref writer, value.FieldCellDataMpo, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __FieldCellDataMpo__ = default(global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __FieldCellDataMpo__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardIntoGimmickFieldCellEventResponce(__MatchId__, __RaisedAt__, __UniqueKey__, __FieldCellDataMpo__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardMoveFieldEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.IsJump);
            writer.Write(value.PreviousX);
            writer.Write(value.PreviousY);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __X__ = default(int);
            var __Y__ = default(int);
            var __IsJump__ = default(bool);
            var __PreviousX__ = default(int);
            var __PreviousY__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __X__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Y__ = reader.ReadInt32();
                        break;
                    case 5:
                        __IsJump__ = reader.ReadBoolean();
                        break;
                    case 6:
                        __PreviousX__ = reader.ReadInt32();
                        break;
                    case 7:
                        __PreviousY__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardMoveFieldEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __X__, __Y__, __IsJump__, __PreviousX__, __PreviousY__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardNopEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardNopEventResponse(__MatchId__, __RaisedAt__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardReceiveFieldUnitEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.SendPlayerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.ReceivePlayerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.SendUnitUniqueKeys, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.ReceiveUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __SendPlayerId__ = default(int?);
            var __ReceivePlayerId__ = default(int?);
            var __SendUnitUniqueKeys__ = default(int[]);
            var __ReceiveUnits__ = default(global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]);
            var __CalculatedStatus__ = default(global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __SendPlayerId__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        __ReceivePlayerId__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        __SendUnitUniqueKeys__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        __ReceiveUnits__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        __CalculatedStatus__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardReceiveFieldUnitEventResponse(__MatchId__, __RaisedAt__, __SendPlayerId__, __ReceivePlayerId__, __SendUnitUniqueKeys__, __ReceiveUnits__, __CalculatedStatus__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardResetRecastTimeEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardResetRecastTimeEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardResetSkillRecastTimeEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardResetSkillRecastTimeEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardRushOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.BackToCurrentPosition);
            writer.Write(value.PreviousX);
            writer.Write(value.PreviousY);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __X__ = default(int);
            var __Y__ = default(int);
            var __BackToCurrentPosition__ = default(bool);
            var __PreviousX__ = default(int);
            var __PreviousY__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __X__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Y__ = reader.ReadInt32();
                        break;
                    case 5:
                        __BackToCurrentPosition__ = reader.ReadBoolean();
                        break;
                    case 6:
                        __PreviousX__ = reader.ReadInt32();
                        break;
                    case 7:
                        __PreviousY__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardRushOperateEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __X__, __Y__, __BackToCurrentPosition__, __PreviousX__, __PreviousY__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSendFieldUnitEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.SendPlayerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.SendUnitUniqueKeys, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __SendPlayerId__ = default(int?);
            var __SendUnitUniqueKeys__ = default(int[]);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __SendPlayerId__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        __SendUnitUniqueKeys__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSendFieldUnitEventResponse(__MatchId__, __RaisedAt__, __SendPlayerId__, __SendUnitUniqueKeys__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(12);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Damage);
            writer.Write(value.SkillId);
            writer.Write(value.IsDead);
            writer.Write(value.SwitchMainEffect);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Serialize(ref writer, value.DamageType, options);
            writer.Write(value.PreviousLife);
            writer.Write(value.IsCounterDamage);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Damage__ = default(int);
            var __SkillId__ = default(int);
            var __IsDead__ = default(bool);
            var __SwitchMainEffect__ = default(bool);
            var __DamageType__ = default(global::Byac.ServerShared.Enums.DamageType);
            var __PreviousLife__ = default(int);
            var __IsCounterDamage__ = default(bool);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 6:
                        __SkillId__ = reader.ReadInt32();
                        break;
                    case 7:
                        __IsDead__ = reader.ReadBoolean();
                        break;
                    case 8:
                        __SwitchMainEffect__ = reader.ReadBoolean();
                        break;
                    case 9:
                        __DamageType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.DamageType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 10:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    case 11:
                        __IsCounterDamage__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Life__, __Damage__, __SkillId__, __IsDead__, __SwitchMainEffect__, __DamageType__, __PreviousLife__, __IsCounterDamage__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillHpHealEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Life);
            writer.Write(value.Heal);
            writer.Write(value.SkillActionId);
            writer.Write(value.PreviousLife);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Life__ = default(int);
            var __Heal__ = default(int);
            var __SkillActionId__ = default(int);
            var __PreviousLife__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Life__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Heal__ = reader.ReadInt32();
                        break;
                    case 6:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    case 7:
                        __PreviousLife__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillHpHealEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Life__, __Heal__, __SkillActionId__, __PreviousLife__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillMoveEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(9);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.IsJump);
            writer.Write(value.PreviousX);
            writer.Write(value.PreviousY);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __X__ = default(int);
            var __Y__ = default(int);
            var __IsJump__ = default(bool);
            var __PreviousX__ = default(int);
            var __PreviousY__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __X__ = reader.ReadInt32();
                        break;
                    case 5:
                        __Y__ = reader.ReadInt32();
                        break;
                    case 6:
                        __IsJump__ = reader.ReadBoolean();
                        break;
                    case 7:
                        __PreviousX__ = reader.ReadInt32();
                        break;
                    case 8:
                        __PreviousY__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillMoveEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __TargetUniqueKey__, __X__, __Y__, __IsJump__, __PreviousX__, __PreviousY__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillOperateEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse[]>(formatterResolver).Serialize(ref writer, value.SkillActionResults, options);
            writer.Write(value.SkillId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>>(formatterResolver).Serialize(ref writer, value.PreviousTargetStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __SkillActionResults__ = default(global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse[]);
            var __SkillId__ = default(int);
            var __PreviousTargetStatus__ = default(global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __SkillActionResults__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        __SkillId__ = reader.ReadInt32();
                        break;
                    case 5:
                        __PreviousTargetStatus__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __SkillActionResults__, __SkillId__, __PreviousTargetStatus__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillOperateEventResponse_SkillActionResultResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.SkillActionId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, int>>(formatterResolver).Serialize(ref writer, value.Targets, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillOperateEventResponse.SkillActionResultResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.SkillActionId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Targets = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillSummonUnitEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.AddUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
            writer.Write(value.SkillActionId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __AddUnits__ = default(global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]);
            var __CalculatedStatus__ = default(global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo);
            var __SkillActionId__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __AddUnits__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        __CalculatedStatus__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillSummonUnitEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __AddUnits__, __CalculatedStatus__, __SkillActionId__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillTPDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Damage);
            writer.Write(value.TechnicalPoint);
            writer.Write(value.SkillActionId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __SkillActionId__ = default(int);
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Damage__ = default(int);
            var __TechnicalPoint__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 5:
                        __TechnicalPoint__ = reader.ReadInt32();
                        break;
                    case 6:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Damage__, __TechnicalPoint__);
            if (length <= 6)
            {
                goto MEMBER_ASSIGNMENT_END;
            }

            ____result.SkillActionId = __SkillActionId__;

        MEMBER_ASSIGNMENT_END:
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardSkillTPHealEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(8);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Heal);
            writer.Write(value.TechnicalPoint);
            writer.Write(value.PreviousTp);
            writer.Write(value.SkillActionId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __SkillActionId__ = default(int);
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Heal__ = default(int);
            var __TechnicalPoint__ = default(int);
            var __PreviousTp__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Heal__ = reader.ReadInt32();
                        break;
                    case 5:
                        __TechnicalPoint__ = reader.ReadInt32();
                        break;
                    case 6:
                        __PreviousTp__ = reader.ReadInt32();
                        break;
                    case 7:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardSkillTPHealEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Heal__, __TechnicalPoint__, __PreviousTp__);
            if (length <= 7)
            {
                goto MEMBER_ASSIGNMENT_END;
            }

            ____result.SkillActionId = __SkillActionId__;

        MEMBER_ASSIGNMENT_END:
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardStatusEffectAddedEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.SkillActionId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.StatusEffectVfxType>(formatterResolver).Serialize(ref writer, value.StatusEffectVfxType, options);
            writer.Write(value.SynergyId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __SkillActionId__ = default(int);
            var __StatusEffectVfxType__ = default(global::Byac.ServerShared.Enums.StatusEffectVfxType);
            var __SynergyId__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    case 5:
                        __StatusEffectVfxType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.StatusEffectVfxType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        __SynergyId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectAddedEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __TargetUniqueKey__, __SkillActionId__, __StatusEffectVfxType__);
            if (length <= 6)
            {
                goto MEMBER_ASSIGNMENT_END;
            }

            ____result.SynergyId = __SynergyId__;

        MEMBER_ASSIGNMENT_END:
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardStatusEffectRemovedEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.SkillActionId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.StatusEffectVfxType>(formatterResolver).Serialize(ref writer, value.StatusEffectVfxType, options);
            writer.Write(value.SynergyId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __SkillActionId__ = default(int);
            var __StatusEffectVfxType__ = default(global::Byac.ServerShared.Enums.StatusEffectVfxType);
            var __SynergyId__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    case 4:
                        __StatusEffectVfxType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.StatusEffectVfxType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        __SynergyId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardStatusEffectRemovedEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __SkillActionId__, __StatusEffectVfxType__);
            if (length <= 5)
            {
                goto MEMBER_ASSIGNMENT_END;
            }

            ____result.SynergyId = __SynergyId__;

        MEMBER_ASSIGNMENT_END:
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardStunEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Duration);
            writer.Write(value.HideTargetUnit);
            writer.Write(value.SkillActionId);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Duration__ = default(float);
            var __HideTargetUnit__ = default(bool);
            var __SkillActionId__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Duration__ = reader.ReadSingle();
                        break;
                    case 5:
                        __HideTargetUnit__ = reader.ReadBoolean();
                        break;
                    case 6:
                        __SkillActionId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardStunEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Duration__, __HideTargetUnit__, __SkillActionId__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardTargetDeadEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardTargetDeadEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardTeleportEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.PreviousX);
            writer.Write(value.PreviousY);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __X__ = default(int);
            var __Y__ = default(int);
            var __PreviousX__ = default(int);
            var __PreviousY__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __X__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Y__ = reader.ReadInt32();
                        break;
                    case 5:
                        __PreviousX__ = reader.ReadInt32();
                        break;
                    case 6:
                        __PreviousY__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardTeleportEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __X__, __Y__, __PreviousX__, __PreviousY__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardTPDamageEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(6);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Damage);
            writer.Write(value.TechnicalPoint);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Damage__ = default(int);
            var __TechnicalPoint__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Damage__ = reader.ReadInt32();
                        break;
                    case 5:
                        __TechnicalPoint__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardTPDamageEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Damage__, __TechnicalPoint__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardTPHealEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(7);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
            writer.Write(value.TargetUniqueKey);
            writer.Write(value.Heal);
            writer.Write(value.TechnicalPoint);
            writer.Write(value.PreviousTp);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __TargetUniqueKey__ = default(int);
            var __Heal__ = default(int);
            var __TechnicalPoint__ = default(int);
            var __PreviousTp__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __TargetUniqueKey__ = reader.ReadInt32();
                        break;
                    case 4:
                        __Heal__ = reader.ReadInt32();
                        break;
                    case 5:
                        __TechnicalPoint__ = reader.ReadInt32();
                        break;
                    case 6:
                        __PreviousTp__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardTPHealEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__, __TargetUniqueKey__, __Heal__, __TechnicalPoint__, __PreviousTp__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardUnitStatusSyncEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo>(formatterResolver).Serialize(ref writer, value.UnitStatus, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo>(formatterResolver).Serialize(ref writer, value.PreviousUnitStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UniqueKey__ = default(int);
            var __UnitStatus__ = default(global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo);
            var __PreviousUnitStatus__ = default(global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __UniqueKey__ = reader.ReadInt32();
                        break;
                    case 3:
                        __UnitStatus__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        __PreviousUnitStatus__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardUnitStatusSyncEventResponse(__MatchId__, __RaisedAt__, __UniqueKey__, __UnitStatus__, __PreviousUnitStatus__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleBoardWeaponChangeEventResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.MatchId);
            writer.Write(value.RaisedAt);
            writer.Write(value.OperatorUniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __OperatorUniqueKey__ = default(int);
            var __MatchId__ = default(int);
            var __RaisedAt__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __MatchId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __RaisedAt__ = reader.ReadSingle();
                        break;
                    case 2:
                        __OperatorUniqueKey__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleBoardWeaponChangeEventResponse(__MatchId__, __RaisedAt__, __OperatorUniqueKey__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleEquipmentDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(21);
            writer.Write(value.UniqueKey);
            writer.Write(value.Id);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Name, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.EquipmentType>(formatterResolver).Serialize(ref writer, value.Type, options);
            writer.Write(value.Life);
            writer.Write(value.Atk);
            writer.Write(value.MagicAtk);
            writer.Write(value.Defence);
            writer.Write(value.MagicDefence);
            writer.Write(value.TechnicalPointRate);
            writer.Write(value.AttackRecastTimeTickRate);
            writer.Write(value.MoveRecastTimeTickRate);
            writer.Write(value.AttackRange);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.AttackType>(formatterResolver).Serialize(ref writer, value.AttackType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Serialize(ref writer, value.AfterSynthesisIds, options);
            writer.Write(value.Rank);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>(formatterResolver).Serialize(ref writer, value.RoleType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>(formatterResolver).Serialize(ref writer, value.TeamType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Serialize(ref writer, value.SelectableEquipmentIds, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Description, options);
            writer.Write(value.SkillDamageScale);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Id = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.Name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Type = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.EquipmentType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.Life = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.Atk = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.MagicAtk = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.Defence = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.MagicDefence = reader.ReadInt32();
                        break;
                    case 9:
                        ____result.TechnicalPointRate = reader.ReadSingle();
                        break;
                    case 10:
                        ____result.AttackRecastTimeTickRate = reader.ReadInt32();
                        break;
                    case 11:
                        ____result.MoveRecastTimeTickRate = reader.ReadInt32();
                        break;
                    case 12:
                        ____result.AttackRange = reader.ReadSingle();
                        break;
                    case 13:
                        ____result.AttackType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.AttackType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 14:
                        ____result.AfterSynthesisIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 15:
                        ____result.Rank = reader.ReadInt32();
                        break;
                    case 16:
                        ____result.RoleType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 17:
                        ____result.TeamType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 18:
                        ____result.SelectableEquipmentIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 19:
                        ____result.Description = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 20:
                        ____result.SkillDamageScale = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleGuildBuffDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GuildId>(formatterResolver).Serialize(ref writer, value.GuildId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Serialize(ref writer, value.GuildBuffIds, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Serialize(ref writer, value.DrawnGuildIds, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Serialize(ref writer, value.TargetUnitIds, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleGuildBuffDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.GuildId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GuildId>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.GuildBuffIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.DrawnGuildIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.TargetUnitIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleLogRoundPlayerResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(10);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.ViewerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Name, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.Party, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>>(formatterResolver).Serialize(ref writer, value.Synergies, options);
            writer.Write(value.IsNpc);
            writer.Write(value.WinningStreakNum);
            writer.Write(value.PrevWinningStreakNum);
            writer.Write(value.LosingStreakNum);
            writer.Write(value.PrevLosingStreakNum);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Serialize(ref writer, value.JudgeResult, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Party = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Synergies = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.IsNpc = reader.ReadBoolean();
                        break;
                    case 5:
                        ____result.WinningStreakNum = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.PrevWinningStreakNum = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.LosingStreakNum = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.PrevLosingStreakNum = reader.ReadInt32();
                        break;
                    case 9:
                        ____result.JudgeResult = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleLogRoundResultsHistoryElementMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(8);
            writer.Write(value.Round);
            writer.Write(value.IsNpcRound);
            writer.Write(value.RoundCycleCount);
            writer.Write(value.CycleRoundNumber);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.WinnerViewerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo>(formatterResolver).Serialize(ref writer, value.Primary, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo>(formatterResolver).Serialize(ref writer, value.Secondary, options);
            writer.Write(value.IsFinished);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Round = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.IsNpcRound = reader.ReadBoolean();
                        break;
                    case 2:
                        ____result.RoundCycleCount = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.CycleRoundNumber = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.WinnerViewerId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.Primary = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        ____result.Secondary = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundPlayerResultMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        ____result.IsFinished = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleLogRoundResultsHistoryMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo>>(formatterResolver).Serialize(ref writer, value.History, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.History = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.BattleLogRoundResultsHistoryElementMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleMatchResultDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.DefeatAt, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleMatchResultDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.DefeatAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattlePlayerDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(34);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Name, options);
            writer.Write(value.TeamId);
            writer.Write(value.AvatarId);
            writer.Write(value.Grade);
            writer.Write(value.BoardId);
            writer.Write(value.Index);
            writer.Write(value.Level);
            writer.Write(value.Exp);
            writer.Write(value.MaxLife);
            writer.Write(value.Life);
            writer.Write(value.Alive);
            writer.Write(value.Gold);
            writer.Write(value.RankUpToken);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Serialize(ref writer, value.EquipmentList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.FieldUnitList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.DeckUnitList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.ShopUnitList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.InitialDistributeUnitList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Serialize(ref writer, value.TeamTransferUnit, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Serialize(ref writer, value.TeamTakeOverUnit, options);
            writer.Write(value.WinsCount);
            writer.Write(value.LosesCount);
            writer.Write(value.IsShopLock);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
            writer.Write(value.RecommendPartyDataId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<float>>(formatterResolver).Serialize(ref writer, value.ShopUnitLotteryRates, options);
            writer.Write(value.TotalWinsCount);
            writer.Write(value.TotalLoseCount);
            writer.Write(value.TotalGold);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo[]>(formatterResolver).Serialize(ref writer, value.UnitMirroringPairs, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<int>>(formatterResolver).Serialize(ref writer, value.OverallBuffList, options);
            writer.Write(value.PartyPower);
            writer.Write(value.IsShopPurchasable);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.TeamId = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.AvatarId = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.Grade = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.BoardId = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.Index = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.Level = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.Exp = reader.ReadInt32();
                        break;
                    case 9:
                        ____result.MaxLife = reader.ReadInt32();
                        break;
                    case 10:
                        ____result.Life = reader.ReadInt32();
                        break;
                    case 11:
                        ____result.Alive = reader.ReadBoolean();
                        break;
                    case 12:
                        ____result.Gold = reader.ReadInt32();
                        break;
                    case 13:
                        ____result.RankUpToken = reader.ReadInt32();
                        break;
                    case 14:
                        ____result.EquipmentList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 15:
                        ____result.FieldUnitList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 16:
                        ____result.DeckUnitList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 17:
                        ____result.ShopUnitList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 18:
                        ____result.InitialDistributeUnitList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 19:
                        ____result.TeamTransferUnit = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 20:
                        ____result.TeamTakeOverUnit = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 21:
                        ____result.WinsCount = reader.ReadInt32();
                        break;
                    case 22:
                        ____result.LosesCount = reader.ReadInt32();
                        break;
                    case 23:
                        ____result.IsShopLock = reader.ReadBoolean();
                        break;
                    case 24:
                        ____result.CalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 25:
                        ____result.RecommendPartyDataId = reader.ReadInt32();
                        break;
                    case 26:
                        ____result.ShopUnitLotteryRates = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<float>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 27:
                        ____result.TotalWinsCount = reader.ReadInt32();
                        break;
                    case 28:
                        ____result.TotalLoseCount = reader.ReadInt32();
                        break;
                    case 29:
                        ____result.TotalGold = reader.ReadInt32();
                        break;
                    case 30:
                        ____result.UnitMirroringPairs = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 31:
                        ____result.OverallBuffList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 32:
                        ____result.PartyPower = reader.ReadInt32();
                        break;
                    case 33:
                        ____result.IsShopPurchasable = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleResultInfoMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(18);
            writer.Write(value.Ranking);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.PlayerName, options);
            writer.Write(value.TeamId);
            writer.Write(value.WinCount);
            writer.Write(value.LoseCount);
            writer.Write(value.LivedRoundCount);
            writer.Write(value.KillCount);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan>(formatterResolver).Serialize(ref writer, value.TimeOfSurvival, options);
            writer.Write(value.TakenDamageCount);
            writer.Write(value.Gold);
            writer.Write(value.Rank);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Serialize(ref writer, value.FieldUnitList, options);
            writer.Write(value.IsWin);
            writer.Write(value.TeamRanking);
            writer.Write(value.TotalGold);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo[]>(formatterResolver).Serialize(ref writer, value.Synergies, options);
            writer.Write(value.IsAbandoned);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Ranking = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.PlayerName = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.TeamId = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.WinCount = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.LoseCount = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.LivedRoundCount = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.KillCount = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.TimeOfSurvival = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 9:
                        ____result.TakenDamageCount = reader.ReadInt32();
                        break;
                    case 10:
                        ____result.Gold = reader.ReadInt32();
                        break;
                    case 11:
                        ____result.Rank = reader.ReadInt32();
                        break;
                    case 12:
                        ____result.FieldUnitList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 13:
                        ____result.IsWin = reader.ReadBoolean();
                        break;
                    case 14:
                        ____result.TeamRanking = reader.ReadInt32();
                        break;
                    case 15:
                        ____result.TotalGold = reader.ReadInt32();
                        break;
                    case 16:
                        ____result.Synergies = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 17:
                        ____result.IsAbandoned = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleRoundBeginDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Serialize(ref writer, value.Players, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo[]>(formatterResolver).Serialize(ref writer, value.Matches, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Serialize(ref writer, value.Teams, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleRoundBeginDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Players = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Matches = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Teams = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleRoundLotteryResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Serialize(ref writer, value.Equipments, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Equipments = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleRoundMatchMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(8);
            writer.Write(value.Id);
            writer.Write(value.PrimaryViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.PrimaryUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.SecondaryViewerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.SecondaryUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.ShadowViewerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.PrimaryFieldUnitCalculatedStatus, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.SecondaryFieldUnitCalculatedStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleRoundMatchMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Id = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.PrimaryViewerId = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.PrimaryUnits = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.SecondaryViewerId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.SecondaryUnits = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.ShadowViewerId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        ____result.PrimaryFieldUnitCalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        ____result.SecondaryFieldUnitCalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleRoundResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.MatchId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<(global::Byac.ServerShared.Enums.BattleResultType, int)>(formatterResolver).Serialize(ref writer, value.PrimaryResult, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<(global::Byac.ServerShared.Enums.BattleResultType, int)>(formatterResolver).Serialize(ref writer, value.SecondaryResult, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyDictionary<int, int>>(formatterResolver).Serialize(ref writer, value.KillCounts, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MatchId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.PrimaryResult = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<(global::Byac.ServerShared.Enums.BattleResultType, int)>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.SecondaryResult = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<(global::Byac.ServerShared.Enums.BattleResultType, int)>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.KillCounts = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyDictionary<int, int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleRoundResultsMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(7);
            writer.Write(value.Round);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo[]>(formatterResolver).Serialize(ref writer, value.Results, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Serialize(ref writer, value.Players, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Serialize(ref writer, value.Teams, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo[]>(formatterResolver).Serialize(ref writer, value.LotteryEquipments, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.RewardDataMpo>>(formatterResolver).Serialize(ref writer, value.Rewards, options);
            writer.Write(value.IsFinished);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleRoundResultsMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Round = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Results = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundResultMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Players = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Teams = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.LotteryEquipments = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleRoundLotteryResultMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.Rewards = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Byac.ServerShared.MessagePackObjects.RewardDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        ____result.IsFinished = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleSynergyMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.SynergyType>(formatterResolver).Serialize(ref writer, value.SynergyType, options);
            writer.Write(value.Id);
            writer.Write(value.CurrentFieldCount);
            writer.Write(value.NextCount);
            writer.Write(value.IsConfirmed);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.SynergyType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.SynergyType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Id = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.CurrentFieldCount = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.NextCount = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.IsConfirmed = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleTeamDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            writer.Write(value.TeamId);
            writer.Write(value.Life);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.DefeatAt, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.TeamId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Life = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.DefeatAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleUnitCalculatedStatusMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.UniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Serialize(ref writer, value.Current, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Serialize(ref writer, value.Adjusted, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Serialize(ref writer, value.Base, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Current = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Adjusted = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Base = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleUnitCalculatedStatusSetMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo[]>(formatterResolver).Serialize(ref writer, value.Synergies, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo[]>(formatterResolver).Serialize(ref writer, value.UnitStatuses, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Synergies = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleSynergyMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.UnitStatuses = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleUnitDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(17);
            writer.Write(value.UniqueKey);
            writer.Write(value.UnitId);
            writer.Write(value.Rank);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Serialize(ref writer, value.EquipmentList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.UnitState>(formatterResolver).Serialize(ref writer, value.State, options);
            writer.Write(value.PositionX);
            writer.Write(value.PositionY);
            writer.Write(value.Index);
            writer.Write(value.IsPurchased);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.FieldDeployAt, options);
            writer.Write(value.IsToken);
            writer.Write(value.IsNpc);
            writer.Write(value.IsHelp);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.OriginPlayerId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>>(formatterResolver).Serialize(ref writer, value.MirroringRoleType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>>(formatterResolver).Serialize(ref writer, value.MirroringTeamType, options);
            writer.Write(value.TotalSkillCount);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.UnitId = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.Rank = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.EquipmentList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.State = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.UnitState>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.PositionX = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.PositionY = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.Index = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.IsPurchased = reader.ReadBoolean();
                        break;
                    case 9:
                        ____result.FieldDeployAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 10:
                        ____result.IsToken = reader.ReadBoolean();
                        break;
                    case 11:
                        ____result.IsNpc = reader.ReadBoolean();
                        break;
                    case 12:
                        ____result.IsHelp = reader.ReadBoolean();
                        break;
                    case 13:
                        ____result.OriginPlayerId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 14:
                        ____result.MirroringRoleType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.ROLE_TYPE>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 15:
                        ____result.MirroringTeamType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Immutable.ImmutableHashSet<global::Byac.ServerShared.MessagePackObjects.UnitData.TEAM_TYPE>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 16:
                        ____result.TotalSkillCount = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BattleUnitMirroringPairDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.Write(value.MainUniqueKey);
            writer.Write(value.TargetUniqueKey);
        }

        public global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BattleUnitMirroringPairDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.MainUniqueKey = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.TargetUniqueKey = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BoardUnitStatusMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(18);
            writer.Write(value.Attack);
            writer.Write(value.MagicAttack);
            writer.Write(value.Life);
            writer.Write(value.Defense);
            writer.Write(value.MagicDefense);
            writer.Write(value.AttackRecastTime);
            writer.Write(value.MoveRecastTime);
            writer.Write(value.RemainAttackRecastTime);
            writer.Write(value.RemainMoveRecastTime);
            writer.Write(value.AttackRecastTimeTickRate);
            writer.Write(value.MoveRecastTimeTickRate);
            writer.Write(value.TechnicalPoint);
            writer.Write(value.MaxTechnicalPoint);
            writer.Write(value.TechnicalPointRate);
            writer.Write(value.AttackRange);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState>(formatterResolver).Serialize(ref writer, value.SpecialState, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.HashSet<int>>(formatterResolver).Serialize(ref writer, value.HateUniqueKeys, options);
            writer.Write(value.SkillUsageCount);
        }

        public global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BoardUnitStatusMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Attack = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.MagicAttack = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.Life = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.Defense = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.MagicDefense = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.AttackRecastTime = reader.ReadSingle();
                        break;
                    case 6:
                        ____result.MoveRecastTime = reader.ReadSingle();
                        break;
                    case 7:
                        ____result.RemainAttackRecastTime = reader.ReadSingle();
                        break;
                    case 8:
                        ____result.RemainMoveRecastTime = reader.ReadSingle();
                        break;
                    case 9:
                        ____result.AttackRecastTimeTickRate = reader.ReadSingle();
                        break;
                    case 10:
                        ____result.MoveRecastTimeTickRate = reader.ReadSingle();
                        break;
                    case 11:
                        ____result.TechnicalPoint = reader.ReadInt32();
                        break;
                    case 12:
                        ____result.MaxTechnicalPoint = reader.ReadInt32();
                        break;
                    case 13:
                        ____result.TechnicalPointRate = reader.ReadSingle();
                        break;
                    case 14:
                        ____result.AttackRange = reader.ReadSingle();
                        break;
                    case 15:
                        ____result.SpecialState = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BoardUnitStatusSpecialState>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 16:
                        ____result.HateUniqueKeys = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.HashSet<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 17:
                        ____result.SkillUsageCount = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BoardUnitSyncStatusMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(9);
            writer.Write(value.Attack);
            writer.Write(value.MagicAttack);
            writer.Write(value.Defense);
            writer.Write(value.MagicDefense);
            writer.Write(value.AttackRecastTime);
            writer.Write(value.MoveRecastTime);
            writer.Write(value.AttackRecastTimeTickRate);
            writer.Write(value.MoveRecastTimeTickRate);
            writer.Write(value.MaxTechnicalPoint);
        }

        public global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BoardUnitSyncStatusMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Attack = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.MagicAttack = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.Defense = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.MagicDefense = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.AttackRecastTime = reader.ReadSingle();
                        break;
                    case 5:
                        ____result.MoveRecastTime = reader.ReadSingle();
                        break;
                    case 6:
                        ____result.AttackRecastTimeTickRate = reader.ReadSingle();
                        break;
                    case 7:
                        ____result.MoveRecastTimeTickRate = reader.ReadSingle();
                        break;
                    case 8:
                        ____result.MaxTechnicalPoint = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BossBattleDeckDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(10);
            writer.Write(value.DeckId);
            writer.Write(value.Ranking);
            writer.Write(value.PartyPower);
            writer.Write(value.TotalGiveDamage);
            writer.Write(value.PlayerLevel);
            writer.Write(value.AvatarId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.DisplayTime, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.Units, options);
            writer.Write(value.IsAvailable);
            writer.Write(value.DeckNumber);
        }

        public global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.DeckId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Ranking = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.PartyPower = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.TotalGiveDamage = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.PlayerLevel = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.AvatarId = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.DisplayTime = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        ____result.Units = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 8:
                        ____result.IsAvailable = reader.ReadBoolean();
                        break;
                    case 9:
                        ____result.DeckNumber = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BossBattleDeckListResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo[]>(formatterResolver).Serialize(ref writer, value.Decks, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Decks = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BossBattleDeckUnitDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(10);
            writer.Write(value.Seq);
            writer.Write(value.UniqueKey);
            writer.Write(value.UnitId);
            writer.Write(value.Rank);
            writer.Write(value.PositionX);
            writer.Write(value.PositionY);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.EquipmentIds, options);
            writer.Write(value.FieldDeployOrder);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.MirroredUnitKey, options);
            writer.Write(value.TotalSkillCount);
        }

        public global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BossBattleDeckUnitDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Seq = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.UnitId = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.Rank = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.PositionX = reader.ReadInt32();
                        break;
                    case 5:
                        ____result.PositionY = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.EquipmentIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        ____result.FieldDeployOrder = reader.ReadInt32();
                        break;
                    case 8:
                        ____result.MirroredUnitKey = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 9:
                        ____result.TotalSkillCount = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BossBattleRemainHpResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.BossUnitId);
            writer.Write(value.RemainHp);
        }

        public global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.BossUnitId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.RemainHp = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class BossRoundStartResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.BossRoundStartResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.BossRoundStartResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Serialize(ref writer, value.Player, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.NpcUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
            writer.Write(value.BossUnitUniqueKey);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.ZakoUnitUniqueKeys, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.BossRoundStartResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.BossRoundStartResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Player = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.NpcUnits = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.CalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.BossUnitUniqueKey = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.ZakoUnitUniqueKeys = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class CycleRewardLotteryResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Serialize(ref writer, value.User, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.Gold, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.Units, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Serialize(ref writer, value.Equipments, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.RankUpToken, options);
            writer.Write(value.BoxGrade);
        }

        public global::Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.CycleRewardLotteryResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.User = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Gold = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Units = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Equipments = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.RankUpToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.BoxGrade = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class EquipmentPickupItemDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.Id);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.UniqueKeys, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.EquipmentPickupItemDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Id = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.UniqueKeys = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class EquipmentToUnitResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Serialize(ref writer, value.PlayerData, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Serialize(ref writer, value.UnitData, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.EquipmentToUnitResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.PlayerData = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.UnitData = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class FieldCellDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.FieldCellGimmickType>(formatterResolver).Serialize(ref writer, value.GimmickType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<float>>(formatterResolver).Serialize(ref writer, value.Values, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.GimmickType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.FieldCellGimmickType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Values = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<float>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class FreeShopRefreshResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleOption>(formatterResolver).Serialize(ref writer, value.InvokerOption, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.InvokerOption = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleOption>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class InformationListItemMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.Write(value.InformationId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.InformationType>(formatterResolver).Serialize(ref writer, value.InformationType, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Title, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Body, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.PublishStartAt, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.InformationId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.InformationType = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.InformationType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Title = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Body = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.PublishStartAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class InformationListResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.InformationListResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.InformationListResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo>>(formatterResolver).Serialize(ref writer, value.Informations, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.InformationListResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.InformationListResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Informations = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.InformationListItemMpo>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingCancelRequestFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequest>
    {
        private readonly global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter __EntryIdCustomFormatter__ = new global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequest value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(1);
            this.__EntryIdCustomFormatter__.Serialize(ref writer, value.EntryId, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequest Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelRequest();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.EntryId = this.__EntryIdCustomFormatter__.Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingCancelResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponse>
    {
        private readonly global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter __CanceledEntryIdCustomFormatter__ = new global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            this.__CanceledEntryIdCustomFormatter__.Serialize(ref writer, value.CanceledEntryId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo[]>(formatterResolver).Serialize(ref writer, value.GroupUsers, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingCancelResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.CanceledEntryId = this.__CanceledEntryIdCustomFormatter__.Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.GroupUsers = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingCompleteResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponse>
    {
        private readonly global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter __BattleIdCustomFormatter__ = new global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            this.__BattleIdCustomFormatter__.Serialize(ref writer, value.BattleId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.RouteToken, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingCompleteResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.BattleId = this.__BattleIdCustomFormatter__.Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.RouteToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingEntryRequestFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequest>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequest value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(1);
            writer.Write(value.DummyKindId);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequest Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryRequest();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.DummyKindId = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingEntryResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponse>
    {
        private readonly global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter __EntryIdCustomFormatter__ = new global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            this.__EntryIdCustomFormatter__.Serialize(ref writer, value.EntryId, options);
            writer.Write(value.DummyKindId);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingEntryResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.EntryId = this.__EntryIdCustomFormatter__.Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.DummyKindId = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingJoinResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo[]>(formatterResolver).Serialize(ref writer, value.GroupUsers, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingJoinResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.GroupUsers = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MatchMakingUserDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.UserName, options);
            writer.Write(value.AvatarId);
            writer.Write(value.GradeId);
            writer.Write(value.ViewerId);
        }

        public global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UserName__ = default(string);
            var __AvatarId__ = default(int);
            var __GradeId__ = default(int);
            var __ViewerId__ = default(int);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __UserName__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        __AvatarId__ = reader.ReadInt32();
                        break;
                    case 2:
                        __GradeId__ = reader.ReadInt32();
                        break;
                    case 3:
                        __ViewerId__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.MatchMakingUserDataMpo(__UserName__, __AvatarId__, __GradeId__, __ViewerId__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NpcBoxDropResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.Gold, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.Units, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Serialize(ref writer, value.Equipments, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.RankUpToken, options);
            writer.Write(value.BoxGrade);
        }

        public global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.NpcBoxDropResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Gold = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Units = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Equipments = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.RankUpToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.BoxGrade = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NpcRoundStartResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Serialize(ref writer, value.Player, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.NpcUnits, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Serialize(ref writer, value.CalculatedStatus, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Player = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.NpcUnits = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.CalculatedStatus = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitCalculatedStatusSetMpo>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NullPickupItemDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(0);
        }

        public global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            reader.Skip();
            return new global::Byac.ServerShared.MessagePackObjects.NullPickupItemDataMpo();
        }
    }

    public sealed class OverallBuffLotteryResultMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<int>>(formatterResolver).Serialize(ref writer, value.OverallBuffList, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.OverallBuffLotteryResultMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.OverallBuffList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<int>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PickupItemPackDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.Index);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase[]>(formatterResolver).Serialize(ref writer, value.ItemDatas, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.PickupItemPackDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Index = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.ItemDatas = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.PickupItemDataMpoBase[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class RankupTokenPickupItemMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(1);
            writer.Write(value.Count);
        }

        public global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.RankupTokenPickupItemMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Count = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class RemainTimeResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.RemainTimeResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.RemainTimeResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.RemainTime, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.RemainTimeResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.RemainTimeResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.RemainTime = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ResumeResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.ResumeResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.ResumeResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan?>(formatterResolver).Serialize(ref writer, value.RemainTime, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.ResumeResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.ResumeResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.RemainTime = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class RewardDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.RewardDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.RewardDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(8);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Serialize(ref writer, value.ResultType, options);
            writer.Write(value.BaseGold);
            writer.Write(value.DistributeGold);
            writer.Write(value.BonusGold);
            writer.Write(value.ResultRewardGold);
            writer.Write(value.InterestGold);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Serialize(ref writer, value.Units, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Serialize(ref writer, value.Equipments, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.RewardDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __ResultType__ = default(global::Byac.ServerShared.Enums.BattleResultType);
            var __BaseGold__ = default(int);
            var __DistributeGold__ = default(int);
            var __BonusGold__ = default(int);
            var __ResultRewardGold__ = default(int);
            var __InterestGold__ = default(int);
            var __Units__ = default(global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]);
            var __Equipments__ = default(global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __ResultType__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleResultType>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        __BaseGold__ = reader.ReadInt32();
                        break;
                    case 2:
                        __DistributeGold__ = reader.ReadInt32();
                        break;
                    case 3:
                        __BonusGold__ = reader.ReadInt32();
                        break;
                    case 4:
                        __ResultRewardGold__ = reader.ReadInt32();
                        break;
                    case 5:
                        __InterestGold__ = reader.ReadInt32();
                        break;
                    case 6:
                        __Units__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 7:
                        __Equipments__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleEquipmentDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.RewardDataMpo(__ResultType__, __BaseGold__, __DistributeGold__, __BonusGold__, __ResultRewardGold__, __InterestGold__, __Units__, __Equipments__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class SetupRoundResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.SetupRoundResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.SetupRoundResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            writer.Write(value.Round);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Serialize(ref writer, value.Players, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse[]>(formatterResolver).Serialize(ref writer, value.FreeShopRefreshInfo, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleOption>(formatterResolver).Serialize(ref writer, value.BattleOption, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo[]>(formatterResolver).Serialize(ref writer, value.FieldCellDatas, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Serialize(ref writer, value.RewardPlayerIds, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.SetupRoundResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.SetupRoundResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Round = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Players = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.FreeShopRefreshInfo = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FreeShopRefreshResponse[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.BattleOption = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleOption>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.FieldCellDatas = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.FieldCellDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.RewardPlayerIds = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UnitDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.UnitData>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.UnitData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(0);
        }

        public global::Byac.ServerShared.MessagePackObjects.UnitData Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            reader.Skip();
            return new global::Byac.ServerShared.MessagePackObjects.UnitData();
        }
    }

    public sealed class UnitPickupItemDataMpoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.UniqueKey);
            writer.Write(value.UnitId);
        }

        public global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.UnitPickupItemDataMpo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.UniqueKey = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.UnitId = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UserReconnectResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.UserReconnectResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.UserReconnectResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(7);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattlePhase>(formatterResolver).Serialize(ref writer, value.Phase, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Serialize(ref writer, value.Players, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse[]>(formatterResolver).Serialize(ref writer, value.NpcRoundStartResponse, options);
            writer.Write(value.Round);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan?>(formatterResolver).Serialize(ref writer, value.RemainTime, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo[]>(formatterResolver).Serialize(ref writer, value.BattleResults, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Serialize(ref writer, value.Teams, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.UserReconnectResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.UserReconnectResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Phase = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattlePhase>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Players = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattlePlayerDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.NpcRoundStartResponse = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.NpcRoundStartResponse[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Round = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.RemainTime = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.TimeSpan?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.BattleResults = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleResultInfoMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        ____result.Teams = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BattleTeamDataMpo[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129
#pragma warning restore SA1309
#pragma warning restore SA1312
#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1129
#pragma warning disable SA1309
#pragma warning disable SA1312
#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.Prize
{
    public sealed class PrizeLotteryResultResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(7);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Ulid>(formatterResolver).Serialize(ref writer, value.BattleId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.CreatedAt, options);
            writer.Write(value.PrizeLotteryResultId);
            writer.Write(value.IsWin);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Serialize(ref writer, value.PrizeId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Url, options);
            writer.Write(value.Rank);
        }

        public global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.BattleId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Ulid>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.CreatedAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.PrizeLotteryResultId = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.IsWin = reader.ReadBoolean();
                        break;
                    case 4:
                        ____result.PrizeId = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<int?>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.Url = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 6:
                        ____result.Rank = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PrizeLotteryResultSetResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle>(formatterResolver).Serialize(ref writer, value.PrizeFeature, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse>>(formatterResolver).Serialize(ref writer, value.Results, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultSetResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.PrizeFeature = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeFeatureToggle>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Results = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeLotteryResultResponse>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PrizeScheduleDateOnlyFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.Year);
            writer.Write(value.Month);
            writer.Write(value.Day);
        }

        public global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Year = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Month = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.Day = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PrizeScheduleResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly>(formatterResolver).Serialize(ref writer, value.Begin, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly>(formatterResolver).Serialize(ref writer, value.End, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Begin = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.End = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.Prize.PrizeScheduleDateOnly>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129
#pragma warning restore SA1309
#pragma warning restore SA1312
#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1129
#pragma warning disable SA1309
#pragma warning disable SA1312
#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.MessagePackObjects.User
{
    public sealed class CreateUserResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.CreateUserResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.CreateUserResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.WriteNil();
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.AuthenticationToken, options);
            writer.Write(value.ViewerId);
            writer.Write(value.DeviceIdentifier);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Name, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.CreateUserResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.CreateUserResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 1:
                        ____result.AuthenticationToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.DeviceIdentifier = global::MessagePack.Internal.CodeGenHelpers.GetArrayFromNullableSequence(reader.ReadBytes());
                        break;
                    case 4:
                        ____result.Name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class CurrentBattleResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponse>
    {
        private readonly global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter __BattleIdCustomFormatter__ = new global::Byac.ServerShared.MessagePack.Formatters.UlidMessagePackFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            this.__BattleIdCustomFormatter__.Serialize(ref writer, value.BattleId, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.RouteToken, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleRule>(formatterResolver).Serialize(ref writer, value.Rule, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.CurrentBattleResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.BattleId = this.__BattleIdCustomFormatter__.Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.RouteToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Rule = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.BattleRule>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class GetAccessTokenResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GetAccessTokenResult>(formatterResolver).Serialize(ref writer, value.Result, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.AccessToken, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.Expiration, options);
            writer.Write(value.ViewerId);
            writer.Write(value.DeviceIdentifier);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.GetAccessTokenResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GetAccessTokenResult>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.AccessToken = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.Expiration = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.DeviceIdentifier = global::MessagePack.Internal.CodeGenHelpers.GetArrayFromNullableSequence(reader.ReadBytes());
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class InitialDataResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.InitialDataResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.InitialDataResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Serialize(ref writer, value.User, options);
            writer.Write(value.PrizeFeature);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.InitialDataResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.InitialDataResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.User = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.PrizeFeature = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ItemResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.ItemResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.ItemResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.ItemId);
            writer.Write(value.PossessCount);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.ItemResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __ItemId__ = default(int);
            var __PossessCount__ = default(int);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __ItemId__ = reader.ReadInt32();
                        break;
                    case 1:
                        __PossessCount__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.ItemResponse(__ItemId__, __PossessCount__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class MasterDataResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.MasterDataResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.MasterDataResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.Data);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Hash, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.MasterDataResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.MasterDataResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Data = global::MessagePack.Internal.CodeGenHelpers.GetArrayFromNullableSequence(reader.ReadBytes());
                        break;
                    case 1:
                        ____result.Hash = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UserDataAndItemResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Serialize(ref writer, value.UserData, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse>(formatterResolver).Serialize(ref writer, value.Items, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.UserDataAndItemResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.UserData = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Items = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UserHomeDataResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(6);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Serialize(ref writer, value.User, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse>(formatterResolver).Serialize(ref writer, value.Furnitures, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse>(formatterResolver).Serialize(ref writer, value.BossRemainHp, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse>(formatterResolver).Serialize(ref writer, value.DeckList, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.InformationListResponse>(formatterResolver).Serialize(ref writer, value.ForceDisplayInformations, options);
            writer.Write(value.RequestedAt);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.UserHomeDataResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.User = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Furnitures = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.BossRemainHp = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleRemainHpResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.DeckList = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.BossBattleDeckListResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.ForceDisplayInformations = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.MessagePackObjects.InformationListResponse>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.RequestedAt = reader.ReadInt64();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UserItemResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.User.ItemResponse>>(formatterResolver).Serialize(ref writer, value.Items, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.UserItemResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Items = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.IReadOnlyList<global::Byac.ServerShared.MessagePackObjects.User.ItemResponse>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class UserResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.MessagePackObjects.User.UserResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.MessagePackObjects.User.UserResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(9);
            writer.Write(value.ViewerId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Name, options);
            writer.Write(value.AvatarId);
            writer.Write(value.BoardId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.TutorialStep>(formatterResolver).Serialize(ref writer, value.TutorialStep, options);
            writer.Write(value.CompletedBattleCount);
            writer.Write(value.Grade);
            writer.Write(value.BattlePoint);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Serialize(ref writer, value.LastLoggedInAt, options);
        }

        public global::Byac.ServerShared.MessagePackObjects.User.UserResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.MessagePackObjects.User.UserResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ViewerId = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.Name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.AvatarId = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.BoardId = reader.ReadInt32();
                        break;
                    case 4:
                        ____result.TutorialStep = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.TutorialStep>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 5:
                        ____result.CompletedBattleCount = reader.ReadInt32();
                        break;
                    case 6:
                        ____result.Grade = reader.ReadInt32();
                        break;
                    case 7:
                        ____result.BattlePoint = reader.ReadInt64();
                        break;
                    case 8:
                        ____result.LastLoggedInAt = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.DateTimeOffset>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129
#pragma warning restore SA1309
#pragma warning restore SA1312
#pragma warning restore SA1403
#pragma warning restore SA1649

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591

#pragma warning disable SA1129
#pragma warning disable SA1309
#pragma warning disable SA1312
#pragma warning disable SA1403
#pragma warning disable SA1649

namespace MessagePack.Formatters.Byac.ServerShared.Primitives
{
    public sealed class AuthenticationTokenFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.AuthenticationToken>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.AuthenticationToken value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value._value, options);
        }

        public global::Byac.ServerShared.Primitives.AuthenticationToken Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ___value__ = default(string);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ___value__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Byac.ServerShared.Primitives.AuthenticationToken(___value__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class GeneralErrorFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.GeneralError>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.GeneralError value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GeneralErrorCode>(formatterResolver).Serialize(ref writer, value.Code, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.Reason, options);
        }

        public global::Byac.ServerShared.Primitives.GeneralError Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.Primitives.GeneralError();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Code = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Enums.GeneralErrorCode>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Reason = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ServiceResultFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.ServiceResult>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.ServiceResult value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.ServiceResultVoid>(formatterResolver).Serialize(ref writer, value.Result, options);
            writer.Write(value.Succeeded);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.GeneralError>(formatterResolver).Serialize(ref writer, value.ErrorDetail, options);
        }

        public global::Byac.ServerShared.Primitives.ServiceResult Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.Primitives.ServiceResult();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Result = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.ServiceResultVoid>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Succeeded = reader.ReadBoolean();
                        break;
                    case 2:
                        ____result.ErrorDetail = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.GeneralError>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ServiceResultFormatter<T, TError> : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.ServiceResult<T, TError>>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.ServiceResult<T, TError> value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<T>(formatterResolver).Serialize(ref writer, value.Result, options);
            writer.Write(value.Succeeded);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<TError>(formatterResolver).Serialize(ref writer, value.ErrorDetail, options);
        }

        public global::Byac.ServerShared.Primitives.ServiceResult<T, TError> Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.Primitives.ServiceResult<T, TError>();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Result = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<T>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Succeeded = reader.ReadBoolean();
                        break;
                    case 2:
                        ____result.ErrorDetail = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<TError>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ServiceResultFormatter<T> : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.ServiceResult<T>>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.ServiceResult<T> value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<T>(formatterResolver).Serialize(ref writer, value.Result, options);
            writer.Write(value.Succeeded);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.GeneralError>(formatterResolver).Serialize(ref writer, value.ErrorDetail, options);
        }

        public global::Byac.ServerShared.Primitives.ServiceResult<T> Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::Byac.ServerShared.Primitives.ServiceResult<T>();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Result = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<T>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.Succeeded = reader.ReadBoolean();
                        break;
                    case 2:
                        ____result.ErrorDetail = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Byac.ServerShared.Primitives.GeneralError>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class ServiceResultVoidFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Byac.ServerShared.Primitives.ServiceResultVoid>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Byac.ServerShared.Primitives.ServiceResultVoid value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(0);
        }

        public global::Byac.ServerShared.Primitives.ServiceResultVoid Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            reader.Skip();
            return new global::Byac.ServerShared.Primitives.ServiceResultVoid();
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129
#pragma warning restore SA1309
#pragma warning restore SA1312
#pragma warning restore SA1403
#pragma warning restore SA1649
