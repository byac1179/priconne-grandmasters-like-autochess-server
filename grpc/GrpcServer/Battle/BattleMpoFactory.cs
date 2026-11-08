using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.Services.Battle
{

    public static class BattleMpoFactory
    {
        private static readonly DateTimeOffset EpochZero = DateTimeOffset.FromUnixTimeSeconds(0);

        public static BattleUnitDataMpo MakeUnit(int uniqueKey, int unitId, int x, int y, UnitState state, int index, bool isNpc = false)
        {
            var info = BattleMaster.GetUnit(unitId);
            return new BattleUnitDataMpo
            {
                UniqueKey = uniqueKey,
                UnitId = unitId,
                Rank = 1,
                EquipmentList = new List<BattleEquipmentDataMpo>(),
                State = state,
                PositionX = x,
                PositionY = y,
                Index = index,
                IsPurchased = false,
                FieldDeployAt = EpochZero,
                IsToken = info != null && info.IsToken,
                IsNpc = isNpc,
                IsHelp = false,
                OriginPlayerId = null,
                MirroringRoleType = ImmutableHashSet<UnitData.ROLE_TYPE>.Empty,
                MirroringTeamType = ImmutableHashSet<UnitData.TEAM_TYPE>.Empty,
                TotalSkillCount = 0,
            };
        }

        public static BattleEquipmentDataMpo MakeEquipment(int uniqueKey, int equipmentId)
        {
            var info = BattleMaster.GetEquipment(equipmentId);
            return new BattleEquipmentDataMpo
            {
                UniqueKey = uniqueKey,
                Id = equipmentId,
                Name = info?.Name ?? "",
                Type = (EquipmentType)(info?.Type ?? 0),
                Life = info?.Life ?? 0,
                Atk = info?.Atk ?? 0,
                MagicAtk = info?.MagicAtk ?? 0,
                Defence = info?.Defence ?? 0,
                MagicDefence = info?.MagicDefence ?? 0,
                TechnicalPointRate = info?.TechnicalPointRate ?? 0f,
                AttackRecastTimeTickRate = info?.AttackRecastTimeTickRate ?? 0,
                MoveRecastTimeTickRate = info?.MoveRecastTimeTickRate ?? 0,
                AttackRange = info?.AttackRange ?? 0f,
                AttackType = (AttackType)(info?.AttackType ?? 0),
                AfterSynthesisIds = info?.AfterSynthesisIds ?? Array.Empty<int>(),
                Rank = info?.Rank ?? 0,
                RoleType = (UnitData.ROLE_TYPE)(info?.RoleType ?? 0),
                TeamType = (UnitData.TEAM_TYPE)(info?.TeamType ?? 0),
                SelectableEquipmentIds = info?.SelectableEquipmentIds ?? Array.Empty<int>(),
                Description = info?.Description ?? "",
                SkillDamageScale = info?.SkillDamageScale ?? 0,
            };
        }

        public static BoardUnitStatusMpo MakeBoardStatus(int unitId, int rank = 1,
            IEnumerable<BattleEquipmentDataMpo> equipment = null)
        {
            var info = BattleMaster.GetUnit(unitId);

            var mult = BattleMaster.UnitRankMultiplier(unitId, rank);
            static int Scale(int v, float m)
            {
                var r = (int)Math.Floor(v * m);
                return r < 0 ? 0 : r;
            }
            var life = Scale(info?.Life ?? 1, mult);
            var atk = Scale(info?.Atk ?? 0, mult);
            var matk = Scale(info?.MagicAtk ?? 0, mult);
            var def = Scale(info?.Defence ?? 0, mult);
            var mdef = Scale(info?.MagicDefence ?? 0, mult);

            if (equipment != null)
            {
                foreach (var g in equipment)
                {
                    var ge = g != null ? BattleMaster.GetEquipment(g.Id) : null;
                    if (ge == null) continue;
                    life += ge.Life;
                    atk += ge.Atk;
                    matk += ge.MagicAtk;
                    def += ge.Defence;
                    mdef += ge.MagicDefence;
                }
            }
            return new BoardUnitStatusMpo
            {
                Attack = atk,
                MagicAttack = matk,
                Life = life < 1 ? 1 : life,
                Defense = def,
                MagicDefense = mdef,

                AttackRecastTime = info?.AttackRecastTime ?? 1.0f,
                MoveRecastTime = info?.MoveRecastTime ?? 1.0f,
                RemainAttackRecastTime = 0f,
                RemainMoveRecastTime = 0f,
                AttackRecastTimeTickRate = 1f,
                MoveRecastTimeTickRate = 1f,
                TechnicalPoint = 0,
                MaxTechnicalPoint = info?.TechnicalPoint ?? 0,
                TechnicalPointRate = 0f,
                AttackRange = info?.AttackRange ?? 1f,
                SpecialState = BoardUnitStatusSpecialState.None,
                HateUniqueKeys = new HashSet<int>(),
                SkillUsageCount = 0,
            };
        }

        public static BattleUnitCalculatedStatusMpo MakeCalculated(int uniqueKey, int unitId, int rank = 1,
            IEnumerable<BattleEquipmentDataMpo> equipment = null)
        {
            var status = MakeBoardStatus(unitId, rank, equipment);
            return new BattleUnitCalculatedStatusMpo
            {
                UniqueKey = uniqueKey,
                Current = status,
                Adjusted = CloneStatus(status),
                Base = CloneStatus(status),
            };
        }

        public static BoardUnitStatusMpo CloneStatus(BoardUnitStatusMpo s)
        {
            return new BoardUnitStatusMpo
            {
                Attack = s.Attack,
                MagicAttack = s.MagicAttack,
                Life = s.Life,
                Defense = s.Defense,
                MagicDefense = s.MagicDefense,
                AttackRecastTime = s.AttackRecastTime,
                MoveRecastTime = s.MoveRecastTime,
                RemainAttackRecastTime = s.RemainAttackRecastTime,
                RemainMoveRecastTime = s.RemainMoveRecastTime,
                AttackRecastTimeTickRate = s.AttackRecastTimeTickRate,
                MoveRecastTimeTickRate = s.MoveRecastTimeTickRate,
                TechnicalPoint = s.TechnicalPoint,
                MaxTechnicalPoint = s.MaxTechnicalPoint,
                TechnicalPointRate = s.TechnicalPointRate,
                AttackRange = s.AttackRange,
                SpecialState = s.SpecialState,
                HateUniqueKeys = new HashSet<int>(s.HateUniqueKeys ?? new HashSet<int>()),
                SkillUsageCount = s.SkillUsageCount,
            };
        }

        public static BattleUnitCalculatedStatusSetMpo MakeStatusSet(BattleUnitCalculatedStatusMpo[] statuses, BattleSynergyMpo[] synergies = null)
        {
            return new BattleUnitCalculatedStatusSetMpo
            {
                Synergies = synergies ?? new BattleSynergyMpo[0],
                UnitStatuses = statuses ?? new BattleUnitCalculatedStatusMpo[0],
            };
        }

        public static BattleUnitCalculatedStatusSetMpo MakeStatusSetForUnits(IEnumerable<BattleUnitDataMpo> units,
            BattleSynergyMpo[] synergies = null, IEnumerable<BattleUnitDataMpo> synergyFieldUnits = null,
            IReadOnlyDictionary<int, BattleGuildBuffEffectInfo> guildBuffEffects = null)
        {
            IReadOnlyList<ActiveSynergyEffect> active = null;
            HashSet<int> fieldKeys = null;
            if (synergyFieldUnits != null)
            {
                var keys = new HashSet<int>();
                var field = new List<BattleUnitDataMpo>();
                foreach (var u in synergyFieldUnits)
                {
                    if (u == null) continue;
                    field.Add(u);
                    keys.Add(u.UniqueKey);
                }
                if (field.Count > 0)
                {
                    active = BattleMaster.EvaluateActiveSynergyEffects(field);
                    fieldKeys = keys;
                }
            }
            var list = new List<BattleUnitCalculatedStatusMpo>();
            foreach (var u in units)
            {
                var status = MakeBoardStatus(u.UnitId, u.Rank, u.EquipmentList);
                var guild = guildBuffEffects != null && guildBuffEffects.TryGetValue(u.UnitId, out var gb)
                    ? gb : null;

                bool foldSynergy = active != null && fieldKeys.Contains(u.UniqueKey);
                FoldPanelSynergies(status, u, foldSynergy ? active : null, guild);
                list.Add(new BattleUnitCalculatedStatusMpo
                {
                    UniqueKey = u.UniqueKey,
                    Current = status,
                    Adjusted = CloneStatus(status),
                    Base = CloneStatus(status),
                });
            }
            return MakeStatusSet(list.ToArray(), synergies);
        }

        private static void FoldPanelSynergies(BoardUnitStatusMpo s, BattleUnitDataMpo u,
            IReadOnlyList<ActiveSynergyEffect> effects, BattleGuildBuffEffectInfo guild)
        {
            if (s == null || u == null) return;
            var roleSet = new HashSet<int>();
            var teamSet = new HashSet<int>();
            bool needMembers = effects != null && effects.Count > 0;
            if (needMembers)
            {
                var info = BattleMaster.GetUnit(u.UnitId);
                foreach (var r in info?.RoleTypes ?? Array.Empty<int>()) roleSet.Add(r);
                foreach (var tp in info?.TeamTypes ?? Array.Empty<int>()) teamSet.Add(tp);
                if (u.MirroringRoleType != null)
                    foreach (var r in u.MirroringRoleType) roleSet.Add((int)r);
                if (u.MirroringTeamType != null)
                    foreach (var tp in u.MirroringTeamType) teamSet.Add((int)tp);
                if (u.EquipmentList != null)
                    foreach (var g in u.EquipmentList)
                    {
                        var ge = g != null ? BattleMaster.GetEquipment(g.Id) : null;
                        if (ge == null) continue;
                        if (ge.RoleType > 0) roleSet.Add(ge.RoleType);
                        if (ge.TeamType > 0) teamSet.Add(ge.TeamType);
                    }
            }
            int life = guild?.Life ?? 0;
            int attack = guild?.Atk ?? 0;
            int defense = guild?.Defence ?? 0;
            int magicDefense = guild?.MagicDefence ?? 0;
            float attackPercent = 0f, magicDefPercent = 0f, speedTickRate = 0f, moveTickRate = 0f;

            if (u.EquipmentList != null)
            {
                foreach (var g in u.EquipmentList)
                {
                    var ge = g != null ? BattleMaster.GetEquipment(g.Id) : null;
                    if (ge == null) continue;
                    speedTickRate += ge.AttackRecastTimeTickRate;
                    moveTickRate += ge.MoveRecastTimeTickRate;
                }
            }
            speedTickRate += guild?.AttackRecastTimeTickRate ?? 0;
            foreach (var syn in effects ?? Array.Empty<ActiveSynergyEffect>())
            {
                if (syn == null) continue;
                var v = syn.Values;
                bool isMember = syn.SynergyType == 1 ? roleSet.Contains(syn.Id) : teamSet.Contains(syn.Id);
                switch ((syn.SynergyType, syn.Id))
                {
                    case (0, 201):
                        if (isMember && v.Length >= 1) life += (int)v[0];
                        break;
                    case (1, 101):
                        if (isMember && v.Length >= 1) defense += (int)v[0];
                        break;
                    case (1, 110):

                        if (v.Length >= 1) magicDefense += (int)v[0];
                        break;
                    case (1, 114):
                        if (isMember && v.Length >= 3)
                        {
                            int seals = Math.Min(9, Math.Max(0, u.TotalSkillCount));
                            if (seals > 0)
                            {
                                life += (int)(v[0] * seals);
                                attack += (int)(v[1] * seals);
                                defense += (int)(v[2] * seals);
                            }
                        }
                        break;
                    case (0, 204):
                    case (0, 210):
                        if (v.Length >= 1) attackPercent += v[0];
                        break;
                    case (0, 207):
                        if (v.Length >= 1) magicDefPercent += v[0];
                        break;
                    case (0, 216):
                        if (v.Length >= 4 && v[3] > 0f) speedTickRate += v[3] - 100f;
                        break;
                }
            }

            if (attack != 0) s.Attack += attack;
            if (magicDefense != 0) s.MagicDefense += magicDefense;
            if (defense != 0) s.Defense += defense;
            if (life != 0) s.Life = Math.Max(1, s.Life + life);
            if (attackPercent != 0f)
                s.Attack = (int)Math.Round(s.Attack * (1f + attackPercent / 100f));
            if (magicDefPercent != 0f)
                s.MagicDefense = (int)Math.Round(s.MagicDefense * (1f + magicDefPercent / 100f));

            if (speedTickRate != 0f)
            {
                s.AttackRecastTime =
                    s.AttackRecastTime / Math.Max(0.01f, 1f + speedTickRate / 100f);
                s.AttackRecastTimeTickRate = Math.Max(0.01f, 1f + speedTickRate / 100f);
            }

            if (moveTickRate != 0f)
            {
                s.MoveRecastTime =
                    Math.Max(0.2f, s.MoveRecastTime / Math.Max(0.01f, 1f + moveTickRate / 100f));
                s.MoveRecastTimeTickRate = Math.Max(0.01f, 1f + moveTickRate / 100f);
            }
        }
    }
}
