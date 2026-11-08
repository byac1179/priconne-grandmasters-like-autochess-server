using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.Services.Battle
{

    public static class SimpleTickSimulator
    {
        public const float StepSeconds = 0.1f;

        public const float AttackCadencePadSeconds = 0.4f;

        public const float MaxSeconds = 60f;
        public const float AtStartSeconds = 0.6f;
        public const float MinFrameGapSeconds = 0.05f;
        public const int MaxEventsPerFrame = 16;

        public const int AttackTpGain = 10;

        public const float DamageTpScale = 0.8f;

        public const float CountToSeconds = 1f;

        private const string StatAttack = "attack";
        private const string StatDefense = "defense";
        private const string StatMagicDefense = "magic_defense";
        private const string StatAttackRecast = "attack_recast_time";
        private const string StatMoveRecast = "move_recast_time";
        private const string StatAttackRange = "attack_range";
        private const string StatTpRate = "tp_rate";
        private const string StatLifesteal = "basic_attack_lifesteal_percent";
        private const int BoardMaxX = 5;
        private const int BoardMaxY = 7;

        private const float UnderAttackRetargetSeconds = 0.2f;

        private const int RetargetStepTolerance = 2;

        private const int DetourMaxSteps = 8;
        private const int TargetProbeSteps = 10;

        private static readonly float DamageScale =
            ReadScale(Environment.GetEnvironmentVariable("BYAC_TICK_DAMAGE_SCALE"));

        public static readonly bool VerboseSimLog =
            Environment.GetEnvironmentVariable("BYAC_SIM_LOG") == "1";

        public sealed class SimResult
        {
            public List<(float at, BattleBoardEventResponse ev)> Events { get; } = new List<(float, BattleBoardEventResponse)>();
            public bool PrimaryWin;
            public bool IsDraw;
            public bool TimedOut;
            public int PrimaryRemainingLife;
            public int SecondaryRemainingLife;
            public int PrimaryAlive;
            public int SecondaryAlive;

            public Dictionary<int, int> ChargeDeltas { get; } = new Dictionary<int, int>();

            public Dictionary<int, int> FinalLives { get; } = new Dictionary<int, int>();

            public List<(int UniqueKey, int Amount)> PendingDamageTp { get; } = new List<(int, int)>();

            public List<(int OperatorKey, int Back, int BackRaw, float Radius, int ActionId, bool OperatorIsPrimary)> PendingReflects { get; } = new List<(int, int, int, float, int, bool)>();
            public bool Reflecting;
        }

        private sealed class CritSource
        {
            public float ChancePercent;
            public float GrowthPercent;
            public int Stacks;
            public float NextAt;
            public float Multiplier = 1f;
        }

        private sealed class SimUnit
        {
            public int UniqueKey;
            public int UnitId;
            public int X, Y;
            public int Life;
            public int MaxLife;

            public int BaseLife;
            public int AttackBase;
            public int DefenseBase;
            public float AttackRange;

            public sealed class StatMod
            {
                public string Stat;
                public bool Percent;
                public float Magnitude;
                public float ExpiresAt;
                public StatusEffectVfxType Vfx;

                public int AuraHolderKey;
                public int AuraActionId;
            }
            public readonly List<StatMod> StatMods = new List<StatMod>();
            public int Attack => FoldStat(AttackBase, StatAttack);
            public int Defense => FoldStat(DefenseBase, StatDefense);
            private int FoldStat(int baseValue, string stat)
            {
                if (StatMods.Count == 0) return baseValue;
                float flat = 0f, pct = 0f;
                foreach (var m in StatMods)
                {
                    if (m.Stat != stat) continue;
                    if (m.Percent) pct += m.Magnitude; else flat += m.Magnitude;
                }
                return Math.Max(0, (int)Math.Round((baseValue + flat) * (1f + pct / 100f)));
            }

            private float StatModProduct(string stat)
            {
                float p = 1f;
                foreach (var m in StatMods)
                    if (m.Stat == stat && m.Percent) p *= 1f + m.Magnitude / 100f;
                return p;
            }
            private float StatModFlatSum(string stat)
            {
                float s = 0f;
                foreach (var m in StatMods)
                    if (m.Stat == stat && !m.Percent) s += m.Magnitude;
                return s;
            }
            private float FoldLive(float baseValue, string stat)
                => Math.Max(0f, (baseValue + StatModFlatSum(stat)) * StatModProduct(stat));

            public float EffectiveAttackRecast => FoldLive(AttackRecastTime, StatAttackRecast) * SufferedAuraSlowMult;

            public float EffectiveAttackPeriod => EffectiveAttackRecast + AttackCadencePadSeconds;
            public float EffectiveMoveRecast => Math.Max(0.1f, FoldLive(MoveRecastTime, StatMoveRecast));
            public float EffectiveAttackRange => FoldLive(AttackRange, StatAttackRange);
            public float EffectiveTpGainMult => FoldLive(TpGainMult, StatTpRate);
            public float EffectiveLifestealPercent
            {
                get
                {
                    float v = LifestealPercent;
                    foreach (var m in StatMods)
                        if (m.Stat == StatLifesteal) v += m.Percent ? LifestealPercent * m.Magnitude / 100f : m.Magnitude;
                    return v;
                }
            }

            public int Shield;
            public int ShieldCapacity;
            public float ShieldExpiresAt;
            public float InvincibleUntil;
            public float MagicInvincibleUntil;
            public float ReflectInvincibleUntil;
            public int ReflectWindowHits;
            public float ReflectTpRefundPercent;
            public int ReflectActionId;

            public float BarrierReflectUntil;
            public float BarrierReflectRadius;
            public int BarrierReflectActionId;

            public bool AttackSplashInstalled;
            public float AttackSplashValue;
            public float AttackSplashRadius;
            public int AttackSplashActionId;
            public float CounterStanceUntil;
            public float CounterStanceSeconds;
            public float CounterDamageFlat;
            public int CounterSkillActionId;

            public float CharmUntil;
            public float TauntUntil;
            public int TauntSourceUniqueKey;

            public sealed class PeriodicEffect
            {
                public int ActionId;
                public int SourceKey;
                public bool IsHeal;
                public bool MaxLifePercent;
                public bool Aura;
                public bool NoDefence;
                public bool IsMagical;

                public float Magnitude;
                public float Interval;
                public float AuraRadius;
                public float NextTickAt;
                public float ExpiresAt;

                public bool StatAura;
                public string AuraStat;
                public bool AuraPercent;
                public float AuraMagnitude;
                public StatusEffectVfxType AuraVfx;
            }
            public readonly List<PeriodicEffect> Periodics = new List<PeriodicEffect>();
            public float AttackRecastTime;

            public float AttackClipSeconds;
            public float MoveRecastTime;
            public int CurrentTp;
            public int MaxTp;
            public int[] SkillActionIds = Array.Empty<int>();
            public int Rank;
            public bool IsPrimary;
            public int StunTicksRemaining;
            public int[] RoleTypes = Array.Empty<int>();
            public int[] TeamTypes = Array.Empty<int>();

            public int[] SynergyRoleTypes = Array.Empty<int>();
            public int[] SynergyTeamTypes = Array.Empty<int>();

            public int ChargeLevel;
            public int ChargeInitial;
            public float[] ChargeSealValues;

            public float UntargetableUntil;
            public float AttackDisabledUntil;
            public float SilenceUntil;
            public float RootUntil;
            public bool BossResistance;
            public float PaAttackStunChance;
            public float PaAttackStunSeconds;
            public float PaSpeedStackPercent;
            public int PaSpeedStackMax;
            public int PaSpeedStacks;
            public float PaTpDownOnHit;
            public int SelfDestructEvery;
            public bool SelfDestructUsed;
            public bool WeaponChanged;
            public bool PaWeaponChange;
            public bool DeadEmitted;
            public readonly List<(int x, int y)> RecentCells = new();
            public BoardUnitSyncStatusMpo LastSyncStatus;
            public float AttackRecastTickRate => StatModProduct(StatAttackRecast);
            public StatusEffectVfxType InvincibleVfx;

            public int InvincibleSynergyId;

            public float LethalGuardUntil;
            public StatusEffectVfxType MagicInvincibleVfx;
            public StatusEffectVfxType ReflectInvincibleVfx;
            public bool Alive => Life > 0;
            public float NextAttackAt;
            public float NextMoveAt;

            public SimUnit WindupTarget;
            public float WindupSince;
            public float WindupImpactAt;
            public float MoveBlockedUntil;
            public float UbWindupSince = -1f;
            public float UbImpactAt;
            public float UbLockedUntil;
            public List<(int actionId, BattleSkillActionInfo action, List<SimUnit> targets)> UbWindupSchedule;
            public SimUnit UbApproachTarget;
            public float UbApproachReach;

            public int TargetUniqueKey;

            public float LastDamagedAt = float.NegativeInfinity;
            public int OutOfRangeSteps;

            public int LastAttackerKey;

            public SimUnit RetaliateTarget;

            public float SkillDamageMult = 1f;
            public float TpGainMult = 1f;

            public int AttackCount;
            public int HunterEvery;
            public int HunterTargets;
            public int HunterDamage;
            public int LightningEvery;
            public int LightningTargets;
            public int LightningDamage;
            public float ExtraDamagePercent;
            public float StunChancePercent;
            public float StunSeconds;
            public int TpDownPerHit;
            public float OldHunterChancePercent;
            public float OldHunterIntervalSeconds;
            public float OldHunterSpeedPercent;
            public float OldHunterDurationSeconds;
            public float OldHunterNextAt;
            public float OldHunterBuffUntil;
            public float AttackRecastBase;

            public List<CritSource> CritSources = new List<CritSource>();
            public float EvasionChancePercent;

            public int MagicDefenceBase;
            public int MagicDefence => FoldStat(MagicDefenceBase, StatMagicDefense);

            public bool GuaranteedHit;
            public bool CriticalEvasion;
            public float LifestealPercent;
            public float MaxLifeDotPercent;
            public float ReflectPercent;
            public int ReflectPassiveId;
            public int NthAttackEvery;
            public float NthAttackMult;
            public int NthAttackTargets;
            public float AuraEnemySlowMult = 1f;
            public float AuraEnemySlowRadius;
            public float SufferedAuraSlowMult = 1f;
            public float ReportedAuraSlowMult = 1f;
            public float SkillDamageTakenMult = 1f;

            public float GearSkillNukeDamage;
            public float GearSkillNukeRadius;
            public int GearSkillNukeTargets;
            public int GearSkillNukePassiveId;
            public int GearSkillStunMaxFires;
            public float GearSkillStunSeconds;
            public int GearSkillStunFires;
            public int GearReviveHpPercent;
            public int GearReviveMaxFires;
            public int GearReviveFires;
            public float GearAttackSealChance;
            public float GearAttackSealSeconds;
            public float BasicAttackSealUntil;
            public int GearLowHpHealBelow;
            public int GearLowHpHealAmount;
            public float GearLowHpHealDelay;
            public float GearLowHpHealRadius;
            public bool GearLowHpHealUsed;
            public float GearLowHpHealQueueAt;
            public float GearSkillEndTpChance;
            public int GearSkillEndTpAmount;
            public float GearSkillDotPercent;
            public float GearSkillDotSeconds;
            public float GearSkillDotHealCut;
            public float HealCutPercent;
            public float HealCutUntil;

            public float Role103Chance;
            public float Role105Chance;
            public float Role105IntervalSeconds;
            public float Role105DurationSeconds;
            public float Role105CutPercent;
            public float Role105NextAt;
            public float DamageCutPercent;
            public float DamageCutUntil;
            public float ExecuteBelowPercent;
            public float SynergyLifestealPercent;
            public float SynergyAttackHealPercent;
            public float Team208BelowPercent;
            public float Team208InvincibleSeconds;
            public bool Team208Used;
            public float Team209SpeedPerDeath;
            public float Team209HealPercent;
            public int Team209Stacks;
            public bool Team209DeathCredited;
        }

        public static SimResult Simulate(
            int matchId,
            IReadOnlyList<BattleUnitDataMpo> primaryUnits,
            BattleUnitCalculatedStatusSetMpo primaryStatus,
            IReadOnlyList<BattleUnitDataMpo> secondaryUnits,
            BattleUnitCalculatedStatusSetMpo secondaryStatus,
            IReadOnlyDictionary<int, BattleGuildBuffEffectInfo> guildBuffEffects = null,
            IReadOnlyList<ActiveSynergyEffect> primarySynergies = null,
            IReadOnlyList<ActiveSynergyEffect> secondarySynergies = null,
            int rngSalt = 0)
        {
            var result = new SimResult();
            var units = new List<SimUnit>();
            if (primaryUnits != null)
                foreach (var u in primaryUnits)
                    units.Add(MakeUnit(u, primaryStatus, isPrimary: true, guildBuffEffects, primarySynergies));
            if (secondaryUnits != null)
                foreach (var u in secondaryUnits)
                    units.Add(MakeUnit(u, secondaryStatus, isPrimary: false, guildBuffEffects, secondarySynergies));
            units.Sort((a, b) => a.UniqueKey.CompareTo(b.UniqueKey));

            ApplyEnemyDefenseDown(units, primarySynergies, isPrimarySide: true);
            ApplyEnemyDefenseDown(units, secondarySynergies, isPrimarySide: false);

            PrimeTargets(units);

            ApplyAssassinJumps(units, matchId, result);

            var rand = new Random(unchecked(matchId * 7919 + rngSalt));
            float primaryTpHealInterval = 0f, secondaryTpHealInterval = 0f;
            int primaryTpHealAmount = 0, secondaryTpHealAmount = 0;
            ResolveTeam202(primarySynergies, out primaryTpHealInterval, out primaryTpHealAmount);
            ResolveTeam202(secondarySynergies, out secondaryTpHealInterval, out secondaryTpHealAmount);
            float nextPrimaryTpHealAt = primaryTpHealInterval, nextSecondaryTpHealAt = secondaryTpHealInterval;

            foreach (var u in units)
                EmitUnitStatusSync(result, matchId, AtStartSeconds, u);

            foreach (var u in units)
                if (u.CurrentTp > 0) ChangeTp(result, matchId, AtStartSeconds, u, u.CurrentTp, true, force: true);

            int summonCounter = 0;

            for (int tick = 0; tick * StepSeconds <= MaxSeconds + 1e-6f; tick++)
            {
                float t = tick * StepSeconds;
                int tickEventMark = result.Events.Count;

                foreach (var v in RefreshEnemySlowAuras(units))
                    EmitUnitStatusSync(result, matchId, AtStartSeconds + t, v);

                foreach (var u in units)
                {

                    foreach (var cs in u.CritSources)
                        if (cs.GrowthPercent > 0f && u.Alive && t >= cs.NextAt - 1e-6f)
                        {
                            cs.Stacks++;
                            cs.NextAt += CountToSeconds;
                        }

                    if (u.GearReviveHpPercent > 0 && u.GearReviveFires < u.GearReviveMaxFires && u.Life <= 0)
                    {
                        u.GearReviveFires++;
                        int back = Math.Max(1, (int)Math.Floor(u.MaxLife * u.GearReviveHpPercent / 100.0));
                        u.Life = Math.Min(u.MaxLife, back);
                        result.Events.Add((AtStartSeconds + t, new BattleBoardHpHealEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            UniqueKey = u.UniqueKey, TargetUniqueKey = u.UniqueKey,
                            Life = u.Life, Heal = back, PreviousLife = 0,

                            ForceHeal = true, EmitOnDead = true,
                        }));
                    }

                    if (u.Alive && u.GearLowHpHealAmount > 0 && !u.GearLowHpHealUsed)
                    {
                        if (u.GearLowHpHealQueueAt <= 0f)
                        {
                            if (u.MaxLife > 0 && u.Life * 100 <= u.MaxLife * u.GearLowHpHealBelow)
                                u.GearLowHpHealQueueAt = t + 1e-4f;
                        }
                        else if (t >= u.GearLowHpHealQueueAt + u.GearLowHpHealDelay - 1e-4f)
                        {
                            u.GearLowHpHealUsed = true;
                            u.GearLowHpHealQueueAt = 0f;
                            foreach (var m in units.Where(x => x.Alive && x.IsPrimary == u.IsPrimary
                                && HexDistance(u, x) <= u.GearLowHpHealRadius))
                            {
                                int preLife = m.Life;
                                int amount = ApplyHealCut(m, u.GearLowHpHealAmount, t);
                                int healed = Math.Min(m.MaxLife, preLife + amount) - preLife;
                                m.Life = preLife + healed;
                                if (healed > 0)
                                    result.Events.Add((AtStartSeconds + t, new BattleBoardHpHealEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                        UniqueKey = u.UniqueKey, TargetUniqueKey = m.UniqueKey,
                                        Life = m.Life, Heal = healed, PreviousLife = preLife,
                                        ForceHeal = false, EmitOnDead = false,
                                    }));
                            }
                        }
                    }

                    if (u.Alive && u.Team208BelowPercent > 0f && !u.Team208Used
                        && u.MaxLife > 0 && u.Life * 100f <= u.MaxLife * u.Team208BelowPercent)
                    {
                        u.Team208Used = true;
                        u.InvincibleUntil = Math.Max(u.InvincibleUntil, t + u.Team208InvincibleSeconds);

                        u.InvincibleVfx = StatusEffectVfxType.Immortal;
                        u.InvincibleSynergyId = 208;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectAddedEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            UniqueKey = u.UniqueKey, TargetUniqueKey = u.UniqueKey,
                            SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.Immortal, SynergyId = 208,
                        }));
                    }

                    if (!u.Alive && !u.Team209DeathCredited
                        && Array.IndexOf(u.SynergyTeamTypes, 209) >= 0)
                    {
                        u.Team209DeathCredited = true;
                        foreach (var m in units)
                        {
                            if (!m.Alive || m.IsPrimary != u.IsPrimary) continue;

                            if (m.Team209SpeedPerDeath <= 0f && m.Team209HealPercent <= 0f) continue;
                            m.Team209Stacks++;
                            if (m.Team209SpeedPerDeath > 0f)
                                m.AttackRecastTime = m.AttackRecastBase / Math.Max(0.01f,
                                    1f + m.Team209Stacks * m.Team209SpeedPerDeath / 100f);
                            if (m.Team209HealPercent > 0f)
                            {
                                int prevL209 = m.Life;
                                int amt209 = ApplyHealCut(m, (int)Math.Floor(m.MaxLife * m.Team209HealPercent / 100.0), t);
                                int healed209 = Math.Min(m.MaxLife, prevL209 + amt209) - prevL209;
                                m.Life = prevL209 + healed209;
                                if (healed209 > 0)
                                    result.Events.Add((AtStartSeconds + t, new BattleBoardHpHealEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                        UniqueKey = m.UniqueKey, TargetUniqueKey = m.UniqueKey,
                                        Life = m.Life, Heal = healed209, PreviousLife = prevL209,
                                        ForceHeal = false, EmitOnDead = false,
                                    }));
                            }

                            EmitUnitStatusSync(result, matchId, AtStartSeconds + t, m);
                        }
                    }
                }
                bool primaryAlive = units.Any(u => u.IsPrimary && u.Alive);
                bool secondaryAlive = units.Any(u => !u.IsPrimary && u.Alive);
                if (!primaryAlive || !secondaryAlive) break;

                foreach (var u in units)
                    if (u.StunTicksRemaining > 0) u.StunTicksRemaining--;

                foreach (var u in units)
                {
                    if (u.StatMods.Count > 0)
                    {

                        var expired = u.StatMods.Where(m => m.ExpiresAt > 0f && t >= m.ExpiresAt
                            && m.AuraHolderKey == 0).ToList();
                        if (expired.Count > 0)
                        {
                            u.StatMods.RemoveAll(m => m.ExpiresAt > 0f && t >= m.ExpiresAt
                                && m.AuraHolderKey == 0);
                            foreach (var m in expired)
                                result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                                {
                                    MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                    UniqueKey = u.UniqueKey, SkillActionId = 0,
                                    StatusEffectVfxType = m.Vfx, SynergyId = 0,
                                }));
                            EmitUnitStatusSync(result, matchId, AtStartSeconds + t, u);
                        }
                    }
                }

                foreach (var u in units)
                {
                    if (u.ShieldExpiresAt > 0f && t >= u.ShieldExpiresAt)
                    {
                        u.Shield = 0; u.ShieldExpiresAt = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardAttachBarrierEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            UniqueKey = u.UniqueKey, TargetUniqueKey = u.UniqueKey,
                            SkillActionId = 0, IsBarrierEnd = true,
                        }));

                        result.Events.Add((AtStartSeconds + t, new BattleBoardBarrierEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            TargetUniqueKey = u.UniqueKey,
                            MaxBufferedDamage = 0, BufferedDamage = 0,
                        }));
                    }
                    if (u.DamageCutUntil > 0f && t >= u.DamageCutUntil)
                    {

                        u.DamageCutUntil = 0f; u.DamageCutPercent = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey,
                            SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.Shield, SynergyId = 105,
                        }));
                    }
                    if (u.InvincibleUntil > 0f && t >= u.InvincibleUntil)
                    {
                        u.InvincibleUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = u.InvincibleVfx, SynergyId = u.InvincibleSynergyId }));
                        u.InvincibleSynergyId = 0;
                    }
                    if (u.LethalGuardUntil > 0f && t >= u.LethalGuardUntil)
                    {

                        u.LethalGuardUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.Immortal, SynergyId = 0 }));
                    }
                    if (u.MagicInvincibleUntil > 0f && t >= u.MagicInvincibleUntil)
                    {
                        u.MagicInvincibleUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = u.MagicInvincibleVfx, SynergyId = 0 }));
                    }
                    if (u.ReflectInvincibleUntil > 0f && t >= u.ReflectInvincibleUntil)
                    {
                        bool neverReflected = u.ReflectWindowHits <= 0;
                        float refundPct = u.ReflectTpRefundPercent;
                        u.ReflectInvincibleUntil = 0f;
                        u.ReflectWindowHits = 0; u.ReflectTpRefundPercent = 0f; u.ReflectActionId = 0;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = u.ReflectInvincibleVfx, SynergyId = 0 }));

                        if (neverReflected && refundPct > 0f && u.Alive && u.MaxTp > 0 && u.CurrentTp < u.MaxTp)
                            ChangeTp(result, matchId, AtStartSeconds + t, u,
                                Math.Min(u.MaxTp, u.CurrentTp + (int)Math.Round(u.MaxTp * refundPct / 100.0)),
                                isSynergy: false);
                    }
                    if (u.CharmUntil > 0f && t >= u.CharmUntil)
                    {
                        u.CharmUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.Charm, SynergyId = 0 }));
                    }
                    if (u.TauntUntil > 0f && t >= u.TauntUntil)
                    {
                        u.TauntUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectRemovedEventResponse
                        { MatchId = matchId, RaisedAt = AtStartSeconds + t, UniqueKey = u.UniqueKey, SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.None, SynergyId = 0 }));
                    }

                    if (u.CounterStanceUntil > 0f && t >= u.CounterStanceUntil)
                    {
                        u.CounterStanceUntil = 0f;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardCounterSkillOperateEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            UniqueKey = u.UniqueKey, SkillActionId = u.CounterSkillActionId,
                            IsEnd = true, IsCancel = false,
                        }));
                    }
                }

                foreach (var u in units)
                {
                    if (u.Periodics.Count == 0) continue;
                    foreach (var e in u.Periodics)
                    {

                        if (!u.Alive && e.StatAura)
                        {
                            WithdrawStatAura(u, e, AtStartSeconds + t, units, result, matchId);
                            e.ExpiresAt = t;
                            continue;
                        }
                        if (!u.Alive) break;
                        while (e.NextTickAt <= t + 1e-6f && e.NextTickAt <= e.ExpiresAt + 1e-6f)
                        {
                            float tickAt = AtStartSeconds + e.NextTickAt;
                            var src = units.FirstOrDefault(s => s.UniqueKey == e.SourceKey && s.Alive) ?? u;
                            if (e.StatAura)
                            {
                                TickStatAura(u, e, tickAt, units, result, matchId);
                            }
                            else if (e.IsHeal)
                            {

                                var healed = e.Aura
                                    ? units.Where(x => x.Alive && x.IsPrimary == u.IsPrimary && HexDistance(u, x) <= e.AuraRadius)
                                            .OrderBy(x => x.UniqueKey).ToList()
                                    : new List<SimUnit> { u };
                                foreach (var victim in healed)
                                {
                                    int prevLife = victim.Life;
                                    int gain = Math.Min(victim.MaxLife, prevLife + ApplyHealCut(victim, (int)Math.Floor(e.Magnitude), t)) - prevLife;
                                    victim.Life = prevLife + gain;
                                    result.Events.Add((tickAt, new BattleBoardHpHealEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = tickAt,
                                        UniqueKey = src.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                                        Life = victim.Life, Heal = gain, PreviousLife = prevLife,
                                        ForceHeal = false, EmitOnDead = false,
                                    }));
                                }
                            }
                            else
                            {
                                List<SimUnit> hit;
                                if (e.Aura)
                                    hit = units.Where(x => x.Alive && x.IsPrimary != u.IsPrimary && HexDistance(u, x) <= e.AuraRadius).ToList();
                                else
                                    hit = new List<SimUnit> { u };
                                foreach (var victim in hit)
                                {
                                    int amount = e.MaxLifePercent
                                        ? (int)Math.Floor(victim.MaxLife * e.Magnitude / 100.0)
                                        : (int)Math.Floor(e.Magnitude);

                                    int rawAmount = amount;

                                    if (!e.NoDefence)
                                    {
                                        int tickDef = e.IsMagical ? victim.MagicDefence : victim.Defense;
                                        amount = (int)Math.Floor(amount * 100.0 / (100.0 + Math.Max(0, tickDef)));
                                    }
                                    if (amount <= 0) continue;
                                    int prevLife = victim.Life;

                                    int dmg = ApplyDamageToTarget(src, victim, amount, e.NextTickAt, e.IsMagical, result, matchId, tickAt, units, rawDamage: rawAmount);
                                    dmg = ApplyGearReviveGuard(victim, prevLife, dmg, result, matchId, tickAt);
                                    if (e.NoDefence)
                                    {

                                        result.Events.Add((tickAt, new BattleBoardHpDamageEventResponse
                                        {
                                            MatchId = matchId, RaisedAt = tickAt,
                                            OperatorUniqueKey = src.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                                            Life = victim.Life, Damage = dmg, IsDead = !victim.Alive,
                                            AttackType = AttackType.None, PreviousLife = prevLife,
                                        }));
                                    }
                                    else
                                    {
                                        result.Events.Add((tickAt, new BattleBoardSkillDamageEventResponse
                                        {
                                            MatchId = matchId, RaisedAt = tickAt,
                                            OperatorUniqueKey = src.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                                            Life = victim.Life, Damage = dmg, SkillId = e.ActionId,
                                            IsDead = !victim.Alive, SwitchMainEffect = false,
                                            DamageType = e.IsMagical ? DamageType.Magical : DamageType.Physical, PreviousLife = prevLife,
                                            IsCounterDamage = false,
                                        }));
                                    }
                                }
                            }
                            e.NextTickAt += e.Interval;
                        }
                    }

                    foreach (var gone in u.Periodics.Where(e => t >= e.ExpiresAt - 1e-6f && e.StatAura).ToList())
                        WithdrawStatAura(u, gone, AtStartSeconds + t, units, result, matchId);
                    u.Periodics.RemoveAll(e => t >= e.ExpiresAt - 1e-6f);
                }

                foreach (var u in units)
                {
                    if (!u.Alive) continue;

                    if (u.Role105IntervalSeconds > 0f && t >= u.Role105NextAt)
                    {
                        u.Role105NextAt = t + u.Role105IntervalSeconds;
                        if (u.Role105Chance > 0f && rand.NextDouble() * 100.0 < u.Role105Chance)
                        {
                            u.DamageCutPercent = u.Role105CutPercent;
                            u.DamageCutUntil = t + u.Role105DurationSeconds;

                            result.Events.Add((AtStartSeconds + t, new BattleBoardStatusEffectAddedEventResponse
                            {
                                MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                UniqueKey = u.UniqueKey, TargetUniqueKey = u.UniqueKey,
                                SkillActionId = 0, StatusEffectVfxType = StatusEffectVfxType.Shield, SynergyId = 105,
                            }));
                        }
                    }
                    if (u.OldHunterBuffUntil > 0f && t >= u.OldHunterBuffUntil)
                    {
                        u.AttackRecastTime = u.AttackRecastBase;
                        u.OldHunterBuffUntil = 0f;
                    }
                    if (u.OldHunterIntervalSeconds > 0f && t >= u.OldHunterNextAt)
                    {
                        u.OldHunterNextAt = t + u.OldHunterIntervalSeconds;
                        if (u.OldHunterChancePercent > 0f && rand.NextDouble() * 100.0 < u.OldHunterChancePercent)
                        {
                            if (u.OldHunterBuffUntil <= t) u.AttackRecastBase = u.AttackRecastTime;
                            u.AttackRecastTime = u.AttackRecastBase / Math.Max(0.01f, 1f + u.OldHunterSpeedPercent / 100f);
                            u.OldHunterBuffUntil = t + u.OldHunterDurationSeconds;
                        }
                    }
                }

                if (primaryTpHealInterval > 0f && t >= nextPrimaryTpHealAt - 1e-6f)
                {
                    foreach (var u in units)
                        if (u.IsPrimary && u.Alive && u.MaxTp > 0)
                            ChangeTp(result, matchId, AtStartSeconds + t, u,
                                Math.Min(u.MaxTp, u.CurrentTp + primaryTpHealAmount), true);
                    nextPrimaryTpHealAt += primaryTpHealInterval;
                }
                if (secondaryTpHealInterval > 0f && t >= nextSecondaryTpHealAt - 1e-6f)
                {
                    foreach (var u in units)
                        if (!u.IsPrimary && u.Alive && u.MaxTp > 0)
                            ChangeTp(result, matchId, AtStartSeconds + t, u,
                                Math.Min(u.MaxTp, u.CurrentTp + secondaryTpHealAmount), true);
                    nextSecondaryTpHealAt += secondaryTpHealInterval;
                }

                foreach (var a in units)
                {

                    if (!a.Alive || a.StunTicksRemaining > 0 || t < a.BasicAttackSealUntil || t < a.NextAttackAt || t < a.AttackDisabledUntil)
                    {
                        a.WindupTarget = null;
                        continue;
                    }
                    SimUnit target;
                    if (a.WindupTarget != null)
                    {

                        if (t < a.WindupImpactAt - 1e-4f) continue;
                        target = a.WindupTarget;
                        a.WindupTarget = null;
                        a.NextAttackAt = a.WindupSince + Math.Max(StepSeconds, a.EffectiveAttackPeriod);
                        if (!target.Alive)
                        {

                            target = SelectTarget(a, units, t);
                            if (target == null || HexDistance(a, target) > a.EffectiveAttackRange)
                                continue;
                        }
                    }
                    else
                    {

                        if (t >= a.SilenceUntil && t >= a.CharmUntil && UbTakesThisBeat(a, units, matchId, t))
                        {
                            continue;
                        }
                        target = SelectTarget(a, units, t);
                        if (target == null) continue;
                        if (HexDistance(a, target) > a.EffectiveAttackRange) continue;
                        if (a.AttackClipSeconds * 0.5f >= StepSeconds)
                        {

                            float impactDelay = a.AttackClipSeconds * 0.5f;
                            if (impactDelay >= a.EffectiveAttackPeriod)
                                impactDelay = Math.Max(StepSeconds, a.EffectiveAttackPeriod - StepSeconds);
                            a.WindupTarget = target;
                            a.WindupSince = t;
                            a.WindupImpactAt = t + impactDelay;

                            a.MoveBlockedUntil = Math.Max(a.MoveBlockedUntil, a.WindupImpactAt);
                            continue;
                        }

                        a.NextAttackAt = t + Math.Max(StepSeconds, a.EffectiveAttackPeriod);
                    }

                    a.MoveBlockedUntil = Math.Max(a.MoveBlockedUntil, t + StepSeconds);

                    bool missed = !a.GuaranteedHit && target.EvasionChancePercent > 0f
                        && rand.NextDouble() * 100.0 < target.EvasionChancePercent;

                    float critMult = !missed ? RollBasicCrit(a, rand) : 0f;

                    int rawAtk = critMult > 0f && !target.CriticalEvasion
                        ? (int)Math.Floor(AttackRawDamage(a.Attack) * critMult)
                        : AttackRawDamage(a.Attack);
                    int dmg = missed ? 0 : ApplyDamageToTarget(a, target, AttackDamage(a.Attack, target.Defense), t, false, result, matchId, AtStartSeconds + t, units, rawDamage: rawAtk);
                    if (critMult > 0f)
                    {
                        int critDmg = (int)Math.Floor(dmg * critMult);
                        dmg = target.CriticalEvasion ? dmg : critDmg;
                    }
                    int prevLife = target.Life;
                    dmg = ApplyGearReviveGuard(target, prevLife, dmg, result, matchId, AtStartSeconds + t);

                    if (a.ExecuteBelowPercent > 0f && dmg > 0 && target.Life > 0 && target.MaxLife > 0
                        && !target.BossResistance
                        && target.Life * 100f <= target.MaxLife * a.ExecuteBelowPercent)
                        target.Life = 0;

                    if (a.AttackSplashInstalled && dmg > 0 && a.AttackSplashValue > 0f)
                    {
                        int splashDamage = (int)Math.Floor(a.AttackSplashValue);
                        if (splashDamage > 0)
                        {
                            foreach (var splashTarget in units)
                            {
                                if (!splashTarget.Alive) continue;
                                if (splashTarget.IsPrimary == a.IsPrimary) continue;
                                if (splashTarget.UniqueKey == target.UniqueKey) continue;
                                if (HexDistance(target, splashTarget) > a.AttackSplashRadius) continue;
                                int sPrev = splashTarget.Life;
                                int sDmg = ApplyDamageToTarget(a, splashTarget,
                                    splashDamage,
                                    t, false, result, matchId, AtStartSeconds + t, units, rawDamage: splashDamage);
                                sDmg = ApplyGearReviveGuard(splashTarget, sPrev, sDmg, result, matchId, AtStartSeconds + t);

                                result.Events.Add((AtStartSeconds + t, new BattleBoardHpDamageEventResponse
                                {
                                    MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                    OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = splashTarget.UniqueKey,
                                    Life = splashTarget.Life, Damage = sDmg, IsDead = !splashTarget.Alive,
                                    AttackType = AttackType.None, PreviousLife = sPrev,
                                }));
                            }
                        }
                    }

                    a.AttackCount++;

                    if (a.SelfDestructEvery > 0 && !a.SelfDestructUsed && a.AttackCount % a.SelfDestructEvery == 0)
                    {
                        a.SelfDestructUsed = true;
                        if (a.Alive) ApplyGearReviveGuard(a, a.Life, a.MaxLife, result, matchId, AtStartSeconds + t);
                    }

                    if (a.PaSpeedStackPercent > 0f && a.PaSpeedStackMax > 0 && a.PaSpeedStacks < a.PaSpeedStackMax)
                    {
                        a.PaSpeedStacks++;
                        a.StatMods.Add(new SimUnit.StatMod { Stat = StatAttackRecast, Percent = true, Magnitude = -a.PaSpeedStackPercent });
                    }

                    int pending214 = a.ExtraDamagePercent > 0f
                        ? (int)Math.Floor(a.Attack * a.ExtraDamagePercent / 100.0) : 0;
                    if (a.HunterEvery > 0 && a.AttackCount % a.HunterEvery == 0 && a.HunterDamage > 0)
                        DealRandomSpecialDamage(a, units, a.HunterTargets, a.HunterDamage, rand,
                            result, matchId, AtStartSeconds + t, target);
                    if (a.LightningEvery > 0 && a.AttackCount % a.LightningEvery == 0 && a.LightningDamage > 0)
                        DealRandomSpecialDamage(a, units, a.LightningTargets, a.LightningDamage, rand,
                            result, matchId, AtStartSeconds + t, target);
                    if (a.StunChancePercent > 0f && target.Alive && !target.BossResistance
                        && rand.NextDouble() * 100.0 < a.StunChancePercent)
                    {
                        int stunTicks = Math.Max(1, (int)Math.Round(a.StunSeconds / StepSeconds));
                        if (target.StunTicksRemaining < stunTicks) target.StunTicksRemaining = stunTicks;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStunEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                            Duration = a.StunSeconds, HideTargetUnit = false,
                        }));
                    }

                    if (a.PaAttackStunChance > 0f && target.Alive && !target.BossResistance
                        && rand.NextDouble() * 100.0 < a.PaAttackStunChance)
                    {
                        int pt = Math.Max(1, (int)Math.Round(a.PaAttackStunSeconds / StepSeconds));
                        if (target.StunTicksRemaining < pt) target.StunTicksRemaining = pt;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardStunEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                            Duration = a.PaAttackStunSeconds, HideTargetUnit = false,
                        }));
                    }
                    if (a.TpDownPerHit > 0 && target.MaxTp > 0)
                        ChangeTp(result, matchId, AtStartSeconds + t, target,
                            Math.Max(0, target.CurrentTp - a.TpDownPerHit), true);

                    if (a.PaTpDownOnHit > 0 && target.MaxTp > 0 && target.Alive)
                        ChangeTp(result, matchId, AtStartSeconds + t, target,
                            Math.Max(0, target.CurrentTp - (int)a.PaTpDownOnHit), false);

                    if (dmg > 0 && a.EffectiveLifestealPercent > 0f)
                        EmitSelfHeal(result, matchId, AtStartSeconds + t, a,
                            (int)Math.Floor(Math.Min(prevLife, dmg) * a.EffectiveLifestealPercent / 100.0), t);

                    int pendingDot = dmg > 0 && target.Alive && a.MaxLifeDotPercent > 0f
                        ? (int)Math.Floor(target.MaxLife * a.MaxLifeDotPercent / 100.0) : 0;

                    if (dmg > 0 && target.ReflectPercent > 0f && a.Alive && a.IsPrimary != target.IsPrimary)
                        EmitGearReflectFrame(result, matchId, AtStartSeconds + t, target, a,
                            (int)Math.Floor(dmg * target.ReflectPercent / 100.0));

                    if (dmg > 0 && target.GearAttackSealChance > 0f && target.Alive
                        && rand.NextDouble() * 100.0 < target.GearAttackSealChance)
                    {
                        a.BasicAttackSealUntil = Math.Max(a.BasicAttackSealUntil, t + target.GearAttackSealSeconds);
                    }

                    BattleBoardAttackOperateEventResponse copyFrame = null;
                    int copyDelta = 0;

                    if (dmg > 0 && !missed && target.Alive && a.Role103Chance > 0f
                        && rand.NextDouble() * 100.0 < a.Role103Chance)
                    {
                        int copyPrev = target.Life;

                        float copyCritMult = !missed ? RollBasicCrit(a, rand) : 0f;
                        int copyRaw = copyCritMult > 0f && !target.CriticalEvasion
                            ? (int)Math.Floor(AttackRawDamage(a.Attack) * copyCritMult)
                            : AttackRawDamage(a.Attack);
                        int copyDmg = ApplyDamageToTarget(a, target, AttackDamage(a.Attack, target.Defense), t, false, result, matchId, AtStartSeconds + t, units, rawDamage: copyRaw);
                        if (copyCritMult > 0f)
                        {
                            int copyCritDmg = (int)Math.Floor(copyDmg * copyCritMult);
                            copyDmg = target.CriticalEvasion ? copyDmg : copyCritDmg;
                        }
                        copyDmg = ApplyGearReviveGuard(target, copyPrev, copyDmg, result, matchId, AtStartSeconds + t);

                        if (a.Alive && a.MaxTp > 0)
                            ChangeTp(result, matchId, AtStartSeconds + t, a, Math.Min(a.MaxTp,
                                a.CurrentTp + (int)Math.Round(AttackTpGain * a.EffectiveTpGainMult)), false);
                        copyFrame = new BattleBoardAttackOperateEventResponse
                        {
                            MatchId = matchId,
                            RaisedAt = AtStartSeconds + t,
                            UniqueKey = a.UniqueKey,
                            TargetUniqueKey = target.UniqueKey,
                            Damage = copyDmg,
                            Life = target.Life,
                            IsCritical = copyCritMult > 0f,
                            HavePassiveSkillEffect = a.AttackSplashInstalled,
                            IsDead = !target.Alive,
                            DamageType = DamageType.Physical,
                            PreviousLife = copyPrev,
                        };
                        copyDelta = copyPrev - target.Life;
                    }

                    if (dmg > 0 && target.Alive)
                    {
                        if (a.SynergyLifestealPercent > 0f)
                            EmitSelfHeal(result, matchId, AtStartSeconds + t, a,
                                (int)Math.Floor(dmg * a.SynergyLifestealPercent / 100.0), t);
                        if (a.SynergyAttackHealPercent > 0f)
                            EmitSelfHeal(result, matchId, AtStartSeconds + t, a,
                                (int)Math.Floor(dmg * a.SynergyAttackHealPercent / 100.0), t);
                    }
                    if (a.NthAttackEvery > 0 && a.NthAttackMult > 0f && a.AttackCount % a.NthAttackEvery == 0)
                        DealRandomSpecialDamage(a, units, a.NthAttackTargets, (int)Math.Floor(a.Attack * a.NthAttackMult), rand,
                            result, matchId, AtStartSeconds + t, target);

                    int mainLife = target.Life + copyDelta;
                    result.Events.Add((AtStartSeconds + t, new BattleBoardAttackOperateEventResponse
                    {
                        MatchId = matchId,
                        RaisedAt = AtStartSeconds + t,
                        UniqueKey = a.UniqueKey,
                        TargetUniqueKey = target.UniqueKey,
                        Damage = dmg,
                        Life = mainLife,
                        IsCritical = critMult > 0f,

                        HavePassiveSkillEffect = a.AttackSplashInstalled,
                        IsDead = copyDelta > 0 ? mainLife <= 0 : !target.Alive,

                        DamageType = missed ? DamageType.Physical | DamageType.Avoid : DamageType.Physical,
                        PreviousLife = prevLife,
                    }));

                    if (copyFrame != null) result.Events.Add((AtStartSeconds + t, copyFrame));

                    EmitSpecialDamageFrame(result, matchId, AtStartSeconds + t, a, target, pending214);
                    EmitSpecialDamageFrame(result, matchId, AtStartSeconds + t, a, target, pendingDot);

                    if (a.Alive && a.MaxTp > 0)
                        ChangeTp(result, matchId, AtStartSeconds + t, a, Math.Min(a.MaxTp,
                            a.CurrentTp + (int)Math.Round(AttackTpGain * a.EffectiveTpGainMult)), false);

                }

                var pendingSummons = new List<SimUnit>();
                foreach (var a in units)
                {
                    a.UbApproachTarget = null;

                    List<(int actionId, BattleSkillActionInfo action, List<SimUnit> targets)> scheduled;
                    if (a.UbWindupSince >= 0f)
                    {

                        if (!a.Alive)
                        {
                            a.UbWindupSince = -1f; a.UbWindupSchedule = null;
                            Console.WriteLine($"[BATTLE] UB charge INTERRUPTED (caster died) key={a.UniqueKey} t={AtStartSeconds + t:0.0}");
                            continue;
                        }
                        if (a.StunTicksRemaining > 0 || t < a.CharmUntil || t < a.SilenceUntil)
                        {
                            a.UbWindupSince = -1f; a.UbWindupSchedule = null;
                            Console.WriteLine($"[BATTLE] UB charge INTERRUPTED (controlled) key={a.UniqueKey} t={AtStartSeconds + t:0.0} " +
                                $"stun={a.StunTicksRemaining} charm={t < a.CharmUntil} silence={t < a.SilenceUntil}");
                            continue;
                        }
                        if (t < a.UbImpactAt - 1e-4f) continue;
                        scheduled = a.UbWindupSchedule;
                        a.UbWindupSince = -1f;
                        a.UbWindupSchedule = null;

                        scheduled = scheduled
                            .Select(s => (s.actionId, s.action, s.targets.Where(x => x.Alive).ToList()))
                            .ToList();
                    }
                    else
                    {

                        if (!a.Alive || a.StunTicksRemaining > 0 || t < a.SilenceUntil || t < a.CharmUntil || a.MaxTp <= 0 || a.SkillActionIds.Length == 0) continue;
                        if (a.CurrentTp < a.MaxTp) continue;

                        if (t < a.UbLockedUntil - 1e-4f) continue;

                        float reach = BattleMaster.UnitSkillRange(a.UnitId, a.Rank);
                        if (reach < 90f)
                        {
                            var aim = UbReachAim(a, units);
                            if (aim != null && HexDistance(a, aim) > reach)
                            {
                                a.UbApproachTarget = aim;
                                a.UbApproachReach = reach;
                                continue;
                            }
                        }
                        scheduled = BuildUbSchedule(a, units, reach, matchId, t);
                        if (scheduled.Count == 0 || !scheduled.Any(s => s.targets.Count > 0))
                        {

                            continue;
                        }
                        float chargeSeconds = UbChargeSeconds(a, scheduled);

                        a.WindupTarget = null;
                        a.MoveBlockedUntil = 0f;

                        ChangeTp(result, matchId, AtStartSeconds + t, a, 0, false);

                        float busyUntil = t + (chargeSeconds > 0f
                            ? chargeSeconds + a.EffectiveAttackRecast
                            : a.EffectiveAttackRecast);
                        a.NextAttackAt = Math.Max(a.NextAttackAt, busyUntil);

                        a.NextMoveAt = Math.Max(a.NextMoveAt, busyUntil);

                        a.UbLockedUntil = Math.Max(a.UbLockedUntil, busyUntil);

                        if (chargeSeconds > 0f)
                        {
                            a.UbWindupSince = t;
                            a.UbImpactAt = t + chargeSeconds;
                            a.UbWindupSchedule = scheduled;
                            Console.WriteLine($"[BATTLE] UB charge start key={a.UniqueKey} t={AtStartSeconds + t:0.0} " +
                                $"charge={chargeSeconds:0.0} release={a.UbImpactAt:0.0} actions={scheduled.Count}");
                            continue;
                        }
                    }
                    float at = AtStartSeconds + t;
                    var castResults = new List<BattleBoardSkillOperateEventResponse.SkillActionResultResponse>();

                    bool gearNukeProcced = false;
                    foreach (var (actionId, action, targets) in scheduled)
                    {
                        if (!a.Alive) break;
                        if (targets.Count == 0) continue;
                        int atype = action.SkillActionType;
                        bool damageTyped = IsDamageAction(atype);
                        bool isHeal = IsHealAction(atype);
                        bool isStun = IsStunAction(atype);

                        bool ubMagical = IsMagicDamageAction(atype);
                        var postHp = new Dictionary<int, int>();
                        var landed = new List<(SimUnit target, int dmg, int prevLife)>();
                        if (isHeal)
                        {
                            if (IsPeriodicHealAction(atype))
                            {

                                foreach (var target in targets)
                                {
                                    MountPeriodic(a, target, actionId, action, t, true, false,
                                        aura: atype == 10052);
                                    postHp[target.UniqueKey] = target.Life;
                                }
                            }
                            else
                            {

                            int amount = (int)Math.Floor(Math.Max(0f, action.Value(1, a.Rank)));
                            foreach (var target in targets)
                            {
                                int prevLife = target.Life;
                                int healed = Math.Min(target.MaxLife, prevLife + ApplyHealCut(target, amount, t)) - prevLife;
                                target.Life = prevLife + healed;
                                postHp[target.UniqueKey] = target.Life;

                                result.Events.Add((at, new BattleBoardSkillHpHealEventResponse
                                {
                                    MatchId = matchId, RaisedAt = at,
                                    OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                                    Life = target.Life, Heal = healed, SkillActionId = action.Id,
                                    PreviousLife = prevLife,
                                }));
                            }
                            }
                        }
                        else if (IsPeriodicDamageAction(atype))
                        {

                            foreach (var target in targets)
                            {
                                MountPeriodic(a, target, actionId, action, t, false,
                                    maxLifePercent: atype == 99995, aura: atype == 99992);
                                postHp[target.UniqueKey] = target.Life;
                            }
                        }
                        else if (isStun)
                        {

                            float stunSecs = action.Value(1, a.Rank);
                            int ticks = Math.Max(1, (int)Math.Round(stunSecs / StepSeconds));
                            foreach (var target in targets)
                            {
                                if (!target.BossResistance && target.StunTicksRemaining < ticks) target.StunTicksRemaining = ticks;
                                postHp[target.UniqueKey] = target.Life;
                                if (!target.BossResistance)
                                    result.Events.Add((at, new BattleBoardStunEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = at,
                                        OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                                        Duration = stunSecs, HideTargetUnit = false,
                                    }));
                            }
                        }
                        else if (IsStatAction(atype) || atype == 10045 || atype == 10036)
                        {

                            ApplyStatAction(a, at, t, action, targets, units, result, matchId);
                            foreach (var target in targets) postHp[target.UniqueKey] = target.Life;
                        }
                        else if (IsBarrierAction(atype) || IsInvincibleAction(atype))
                        {

                            ApplyBarrierAction(a, at, t, action, targets, result, matchId);
                            foreach (var target in targets) postHp[target.UniqueKey] = target.Life;
                        }
                        else if (IsCharmAction(atype) || IsTauntAction(atype))
                        {

                            float cdur = Math.Max(0f, action.Value(1, a.Rank)) * CountToSeconds;
                            foreach (var target in targets)
                            {
                                if (atype == 10018) target.CharmUntil = t + cdur;
                                else { target.TauntUntil = t + cdur; target.TauntSourceUniqueKey = a.UniqueKey; }
                                result.Events.Add((at, new BattleBoardStatusEffectAddedEventResponse
                                {
                                    MatchId = matchId, RaisedAt = at,
                                    UniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                                    SkillActionId = action.Id,

                                    StatusEffectVfxType = atype == 10018 ? StatusEffectVfxType.Charm : StatusEffectVfxType.None,
                                    SynergyId = 0,
                                }));
                                postHp[target.UniqueKey] = target.Life;
                            }
                        }
                        else if (IsMoveAction(atype))
                        {

                            ApplyMoveAction(a, at, action, targets, units, result, matchId);
                            postHp[a.UniqueKey] = a.Life;
                        }
                        else if (IsTpGainAction(atype))
                        {

                            int tp = Math.Max(0, (int)Math.Floor(action.Value(1, a.Rank)));
                            foreach (var target in targets)
                            {
                                if (target.MaxTp <= 0) continue;
                                ChangeTp(result, matchId, at, target,
                                    Math.Min(target.MaxTp, target.CurrentTp + tp), false);
                                postHp[target.UniqueKey] = target.Life;
                            }
                        }
                        else if (IsAttackSplashAction(atype))
                        {

                            a.AttackSplashInstalled = true;
                            a.AttackSplashValue = action.Value(1, a.Rank);
                            a.AttackSplashRadius = action.Value(2, a.Rank);
                            a.AttackSplashActionId = actionId;
                            result.Events.Add((at, new BattleBoardStatusEffectAddedEventResponse
                            {
                                MatchId = matchId, RaisedAt = at,
                                UniqueKey = a.UniqueKey, TargetUniqueKey = a.UniqueKey,
                                SkillActionId = action.Id,
                                StatusEffectVfxType = StatusEffectVfxType.ExtraAttackDamage,
                                SynergyId = 0,
                            }));
                            postHp[a.UniqueKey] = a.Life;
                        }
                        else if (IsCounterStanceAction(atype))
                        {

                            float stanceSecs = Math.Max(0f, action.Value(2, a.Rank));
                            a.CounterStanceUntil = stanceSecs > 0f ? t + stanceSecs : t + MaxSeconds;
                            a.CounterStanceSeconds = stanceSecs;
                            a.CounterDamageFlat = action.Value(1, a.Rank);
                            a.CounterSkillActionId = actionId;
                            result.Events.Add((at, new BattleBoardCounterSkillOperateEventResponse
                            {
                                MatchId = matchId, RaisedAt = at,
                                UniqueKey = a.UniqueKey, SkillActionId = action.Id,
                                IsEnd = false, IsCancel = false,
                            }));
                            postHp[a.UniqueKey] = a.Life;
                        }
                        else if (IsSummonAction(atype))
                        {

                            int unitId = (int)action.Value(1, a.Rank);
                            int count = Math.Max(1, (int)action.Value(2, a.Rank));
                            var info = BattleMaster.GetUnit(unitId);

                            int summonRank = Math.Max(1, a.Rank);
                            var panel = BattleMpoFactory.MakeBoardStatus(unitId, summonRank);
                            for (int i = 0; i < count; i++)
                            {
                                int sx = -1, sy = -1;
                                for (int ring = 1; ring <= 2 && sx < 0; ring++)
                                {
                                    foreach (var (x, y) in HexRingCells(a.X, a.Y, ring))
                                    {
                                        if (x < 0 || x > BoardMaxX || y < 0 || y > BoardMaxY) continue;
                                        if (units.Any(u => u.X == x && u.Y == y)) continue;
                                        if (pendingSummons.Any(u => u.X == x && u.Y == y)) continue;
                                        sx = x; sy = y; break;
                                    }
                                }
                                if (sx < 0) break;
                                int skey = -(100000 + matchId * 100 + summonCounter++);
                                var summon = new SimUnit
                                {
                                    UniqueKey = skey,
                                    UnitId = unitId,
                                    X = sx, Y = sy,
                                    Life = panel.Life < 1 ? 1 : panel.Life,
                                    AttackBase = panel.Attack,
                                    DefenseBase = panel.Defense,
                                    MagicDefenceBase = panel.MagicDefense,
                                    AttackRange = info?.AttackRange > 0f ? info.AttackRange : 1f,
                                    AttackRecastTime = info?.AttackRecastTime > 0f ? info.AttackRecastTime : 1f,
                                    AttackClipSeconds = AnimClipTable.AttackSeconds(unitId),
                                    MoveRecastTime = info?.MoveRecastTime > 0f ? info.MoveRecastTime : 1f,
                                    MaxTp = 0,
                                    Rank = summonRank,
                                    IsPrimary = a.IsPrimary,
                                    RoleTypes = info?.RoleTypes ?? Array.Empty<int>(),
                                    TeamTypes = info?.TeamTypes ?? Array.Empty<int>(),
                                };
                                summon.MaxLife = summon.Life;
                                pendingSummons.Add(summon);
                                result.Events.Add((at, new BattleBoardSkillSummonUnitEventResponse
                                {
                                    MatchId = matchId, RaisedAt = at, UniqueKey = a.UniqueKey,
                                    AddUnits = new[] { BattleMpoFactory.MakeUnit(skey, unitId, sx, sy, UnitState.Field, i) },

                                    CalculatedStatus = BattleMpoFactory.MakeStatusSet(new[]
                                    {
                                        BattleMpoFactory.MakeCalculated(skey, unitId, summonRank),
                                    }),
                                    SkillActionId = action.Id,
                                }));
                                EmitUnitStatusSync(result, matchId, at, summon);
                            }
                            postHp[a.UniqueKey] = a.Life;
                        }
                        else if (!damageTyped)
                        {

                            foreach (var target in targets) postHp[target.UniqueKey] = target.Life;
                        }
                        else
                        {
                            foreach (var target in targets)
                            {
                                int prevLife = target.Life;

                                int ubDef = ubMagical ? target.MagicDefence : target.Defense;

                                bool execActive = action.SkillActionType == 10048
                                    && target.MaxLife > 0
                                    && prevLife * 100f <= target.MaxLife * action.Value(2, a.Rank);
                                int dmg = ApplyDamageToTarget(a, target, SkillDamage(a.Attack, ubDef, action, a.Rank,
                                    a.SkillDamageMult, target.SkillDamageTakenMult, execActive,
                                    SkillFalloffMult(action, a, target, targets, a.Rank)), t, ubMagical, result, matchId, at, units,

                                    rawDamage: SkillRawDamage(a.Attack, action, a.Rank,
                                        a.SkillDamageMult, execActive,
                                        SkillFalloffMult(action, a, target, targets, a.Rank)));
                                dmg = ApplyGearReviveGuard(target, prevLife, dmg, result, matchId, at);
                                postHp[target.UniqueKey] = target.Life;
                                landed.Add((target, dmg, prevLife));
                            }

                            if (action.SkillActionType == 10041 && landed.Count > 0)
                            {
                                float sidePct = Math.Max(0f, action.Value(2, a.Rank));
                                float mainPct = action.Value(1, a.Rank);
                                foreach (var hit in landed.ToList())
                                {
                                    var sides = units
                                        .Where(u => u.Alive && u.IsPrimary != a.IsPrimary
                                            && u.UniqueKey != hit.target.UniqueKey
                                            && !landed.Any(h => ReferenceEquals(h.target, u))
                                            && HexDistanceTo(hit.target.X, hit.target.Y, u.X, u.Y) <= 1)
                                        .OrderBy(u => u.UniqueKey).Take(2).ToList();
                                    foreach (var side in sides)
                                    {
                                        int sPrev = side.Life;
                                        double sPower = a.Attack * (double)mainPct / 100.0 * sidePct / 100.0;
                                        int sDmg = ApplyDamageToTarget(a, side,
                                            (int)(sPower * DamageScale * Math.Max(0f, a.SkillDamageMult)
                                                  * Math.Max(0f, side.SkillDamageTakenMult)),
                                            t, false, result, matchId, at, units,
                                            rawDamage: (int)(sPower * DamageScale * Math.Max(0f, a.SkillDamageMult)));
                                        sDmg = ApplyGearReviveGuard(side, sPrev, sDmg, result, matchId, at);
                                        postHp[side.UniqueKey] = side.Life;
                                        landed.Add((side, sDmg, sPrev));
                                    }
                                }
                            }
                        }

                        castResults.Add(new BattleBoardSkillOperateEventResponse.SkillActionResultResponse
                        {
                            SkillActionId = actionId,

                            Targets = postHp,
                        });
                        foreach (var (target, dmg, prevLife) in landed)
                        {
                            result.Events.Add((at, new BattleBoardSkillDamageEventResponse
                            {
                                MatchId = matchId, RaisedAt = at,
                                OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                                Life = target.Life, Damage = dmg, SkillId = actionId,
                                IsDead = !target.Alive, SwitchMainEffect = false,
                                DamageType = ubMagical ? DamageType.Magical : DamageType.Physical, PreviousLife = prevLife,
                                IsCounterDamage = false,
                            }));

                            if (dmg <= 0) continue;

                            if (a.SynergyLifestealPercent > 0f)
                                EmitSelfHeal(result, matchId, at, a,
                                    (int)Math.Floor(dmg * a.SynergyLifestealPercent / 100.0), t);

                            if (target.GearSkillStunMaxFires > 0 && target.GearSkillStunFires < target.GearSkillStunMaxFires)
                            {
                                target.GearSkillStunFires++;
                                int stunTicks = Math.Max(1, (int)Math.Round(target.GearSkillStunSeconds / StepSeconds));
                                if (a.Alive && !a.BossResistance && a.StunTicksRemaining < stunTicks) a.StunTicksRemaining = stunTicks;
                                if (a.Alive && !a.BossResistance)
                                    result.Events.Add((AtStartSeconds + t, new BattleBoardStunEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                        OperatorUniqueKey = target.UniqueKey, TargetUniqueKey = a.UniqueKey,
                                        Duration = target.GearSkillStunSeconds, HideTargetUnit = false,
                                    }));
                            }

                            if (!gearNukeProcced && a.GearSkillNukeDamage > 0f && a.GearSkillNukeRadius > 0f)
                            {
                                gearNukeProcced = true;
                                int want = 1 + a.GearSkillNukeTargets;
                                var cand = new List<(int dist, SimUnit unit)>();
                                foreach (var x in units)
                                {
                                    if (!x.Alive || x.IsPrimary == a.IsPrimary) continue;
                                    int dist = HexDistance(target, x);
                                    if (dist <= a.GearSkillNukeRadius) cand.Add((dist, x));
                                }
                                var nuked = cand
                                    .OrderBy(c => c.dist).ThenBy(c => c.unit.UniqueKey)
                                    .Select(c => c.unit).Take(want).ToList();
                                foreach (var v in nuked)
                                {
                                    int nprev = v.Life;

                                    int ndmg = ApplyDamageToTarget(a, v, (int)Math.Floor(a.GearSkillNukeDamage), t, true, result, matchId, at, units);
                                    ndmg = ApplyGearReviveGuard(v, nprev, ndmg, result, matchId, at);
                                    result.Events.Add((at, new BattleBoardSkillDamageEventResponse
                                    {
                                        MatchId = matchId, RaisedAt = at,
                                        OperatorUniqueKey = a.UniqueKey, TargetUniqueKey = v.UniqueKey,
                                        Life = v.Life, Damage = ndmg, SkillId = a.GearSkillNukePassiveId,
                                        IsDead = !v.Alive, SwitchMainEffect = false,
                                        DamageType = DamageType.Magical, PreviousLife = nprev,
                                        IsCounterDamage = false,
                                    }));
                                }
                            }

                            if (a.GearSkillDotPercent > 0f && a.GearSkillDotSeconds > 0f)
                            {
                                var existing = target.Periodics.FirstOrDefault(p => p.ActionId == 300920 && p.SourceKey == a.UniqueKey);
                                if (existing != null)
                                {
                                    existing.NextTickAt = t + existing.Interval;
                                    existing.ExpiresAt = t + a.GearSkillDotSeconds;
                                }
                                else
                                {
                                    target.Periodics.Add(new SimUnit.PeriodicEffect
                                    {
                                        ActionId = 300920,
                                        SourceKey = a.UniqueKey,
                                        IsHeal = false,
                                        MaxLifePercent = true,
                                        NoDefence = true,
                                        IsMagical = true,
                                        Magnitude = a.GearSkillDotPercent,
                                        Interval = 1f,
                                        NextTickAt = t + 1f,
                                        ExpiresAt = t + a.GearSkillDotSeconds,
                                    });
                                }
                                target.HealCutPercent = a.GearSkillDotHealCut;
                                target.HealCutUntil = t + a.GearSkillDotSeconds;
                            }
                        }
                    }

                    if (castResults.Count > 0)
                    {
                        result.Events.Add((at, new BattleBoardSkillOperateEventResponse
                        {
                            MatchId = matchId, RaisedAt = at, UniqueKey = a.UniqueKey,
                            SkillActionResults = castResults.ToArray(),
                            SkillId = a.UnitId,
                            PreviousTargetStatus = new Dictionary<int, BoardUnitStatusMpo>(),
                        }));

                    }

                    if (a.ChargeSealValues != null && a.ChargeSealValues.Length >= 3 && a.ChargeLevel < 9)
                    {
                        a.ChargeLevel++;
                        int sealHp = (int)a.ChargeSealValues[0];
                        a.MaxLife += sealHp;
                        a.AttackBase += (int)a.ChargeSealValues[1];
                        a.DefenseBase += (int)a.ChargeSealValues[2];
                        if (a.Alive) a.Life = Math.Min(a.MaxLife, a.Life + sealHp);
                    }

                    if (a.PaWeaponChange && !a.WeaponChanged)
                    {
                        a.WeaponChanged = true;
                        result.Events.Add((AtStartSeconds + t, new BattleBoardWeaponChangeEventResponse
                        {
                            MatchId = matchId, RaisedAt = AtStartSeconds + t,
                            OperatorUniqueKey = a.UniqueKey,
                        }));

                        if (a.MaxTp > 0)
                        {
                            ChangeTp(result, matchId, AtStartSeconds + t, a, 0, false, force: true);
                            a.MaxTp = 0;
                            a.CurrentTp = 0;
                            EmitUnitStatusSync(result, matchId, AtStartSeconds + t, a);
                        }
                    }

                    if (a.GearSkillEndTpAmount > 0 && a.MaxTp > 0
                        && rand.NextDouble() * 100.0 < a.GearSkillEndTpChance)
                    {
                        ChangeTp(result, matchId, AtStartSeconds + t, a,
                            Math.Min(a.MaxTp, a.CurrentTp + a.GearSkillEndTpAmount), false);
                    }
                }

                if (pendingSummons.Count > 0) units.AddRange(pendingSummons);

                foreach (var a in units)
                {

                    if (!a.Alive || a.StunTicksRemaining > 0 || t < a.RootUntil || t < a.NextMoveAt
                        || t < a.MoveBlockedUntil) continue;
                    var target = SelectTarget(a, units, t);

                    float hold = a.EffectiveAttackRange;
                    if (a.UbApproachTarget != null && a.UbApproachTarget.Alive)
                    {
                        target = a.UbApproachTarget;
                        hold = a.UbApproachReach;
                    }
                    if (target == null) continue;

                    bool trackStuck = a.TargetUniqueKey != 0 && target.UniqueKey == a.TargetUniqueKey;
                    int dist0 = HexDistance(a, target);
                    if (dist0 <= hold)
                    {
                        continue;
                    }
                    var offsets = (a.Y & 1) == 0 ? HexEvenOffsets : HexOddOffsets;
                    int bx = a.X, by = a.Y, bd = dist0;
                    int ex = -1, ey = -1;
                    int rbx = a.X, rby = a.Y, rbd = dist0;
                    foreach (var o in offsets)
                    {
                        int nx = a.X + o.dx, ny = a.Y + o.dy;
                        if (nx < 0 || nx > BoardMaxX || ny < 0 || ny > BoardMaxY) continue;
                        if (units.Any(u => u.Alive && !ReferenceEquals(u, a) && u.X == nx && u.Y == ny)) continue;
                        int d = HexDistanceTo(nx, ny, target.X, target.Y);
                        bool recent = a.RecentCells.Contains((nx, ny));

                        if (d < bd && !recent) { bd = d; bx = nx; by = ny; }
                        else if (d < rbd && recent) { rbd = d; rbx = nx; rby = ny; }

                        else if (d == bd && ex < 0 && !recent) { ex = nx; ey = ny; }
                    }
                    if (bx == a.X && by == a.Y)
                    {

                        if (TryFindApproach(a, target, hold, units, DetourMaxSteps,
                                out int dx2, out int dy2, out _, avoidRecent: true))
                        {
                            bx = dx2; by = dy2;
                        }
                        else if (TryFindApproach(a, target, hold, units, DetourMaxSteps,
                                out dx2, out dy2, out _))
                        {
                            bx = dx2; by = dy2;
                        }
                        else if (ex >= 0) { bx = ex; by = ey; }
                        else if (rbx != a.X || rby != a.Y) { bx = rbx; by = rby; }
                    }
                    if (bx == a.X && by == a.Y)
                    {

                        if (trackStuck) TryRetargetIfUnreachable(a, units);
                        continue;
                    }
                    int px = a.X, py = a.Y;
                    a.X = bx; a.Y = by;
                    a.NextMoveAt = t + Math.Max(StepSeconds, a.EffectiveMoveRecast);
                    a.RecentCells.Add((px, py));
                    while (a.RecentCells.Count > 6) a.RecentCells.RemoveAt(0);
                    if (trackStuck)
                    {

                        a.OutOfRangeSteps++;

                        if (HexDistanceTo(bx, by, target.X, target.Y) < dist0) { }
                        else TryRetargetIfUnreachable(a, units);
                    }
                    result.Events.Add((AtStartSeconds + t, new BattleBoardMoveFieldEventResponse
                    {
                        MatchId = matchId, RaisedAt = AtStartSeconds + t,
                        UniqueKey = a.UniqueKey, X = a.X, Y = a.Y, IsJump = false,
                        PreviousX = px, PreviousY = py,
                    }));
                }

                if (result.PendingReflects.Count > 0)
                {
                    var pending = result.PendingReflects.ToArray();
                    result.PendingReflects.Clear();
                    result.Reflecting = true;
                    foreach (var (opKey, back, backRaw, radius, actionId, opIsPrimary) in pending)
                    {
                        var ray = units.FirstOrDefault(u => u.UniqueKey == opKey);
                        if (ray == null || !ray.Alive || back <= 0) continue;
                        foreach (var f in units)
                        {
                            if (!f.Alive || f.IsPrimary == opIsPrimary) continue;
                            if (HexDistance(ray, f) > radius) continue;
                            int fPrev = f.Life;
                            int fDmg = ApplyDamageToTarget(ray, f, back, t, false, result, matchId,
                                AtStartSeconds + t, units, allowCounter: false, rawDamage: backRaw);
                            fDmg = ApplyGearReviveGuard(f, fPrev, fDmg, result, matchId, AtStartSeconds + t);

                            result.Events.Add((AtStartSeconds + t, new BattleBoardHpDamageEventResponse
                            {
                                MatchId = matchId, RaisedAt = AtStartSeconds + t,
                                OperatorUniqueKey = ray.UniqueKey, TargetUniqueKey = f.UniqueKey,
                                Life = f.Life, Damage = fDmg, IsDead = !f.Alive,
                                AttackType = AttackType.None, PreviousLife = fPrev,
                            }));
                        }
                    }
                    result.Reflecting = false;
                }

                if (result.PendingDamageTp.Count > 0)
                {
                    foreach (var (key, amount) in result.PendingDamageTp)
                    {
                        var victim = units.FirstOrDefault(u => u.UniqueKey == key);

                        if (victim == null || !victim.Alive || victim.MaxTp <= 0) continue;
                        ChangeTp(result, matchId, AtStartSeconds + t, victim,
                            Math.Min(victim.MaxTp, victim.CurrentTp + amount), false);
                    }
                    result.PendingDamageTp.Clear();
                }

                EmitUnreportedDeaths(result, matchId, AtStartSeconds + t, units, tickEventMark);
            }

            result.PrimaryAlive = units.Count(u => u.IsPrimary && u.Alive);
            result.SecondaryAlive = units.Count(u => !u.IsPrimary && u.Alive);
            result.PrimaryRemainingLife = units.Where(u => u.IsPrimary && u.Alive).Sum(u => u.Life);
            result.SecondaryRemainingLife = units.Where(u => !u.IsPrimary && u.Alive).Sum(u => u.Life);
            foreach (var u in units)
            {
                result.FinalLives[u.UniqueKey] = u.Alive ? Math.Max(0, u.Life) : 0;
            }
            if (result.PrimaryAlive > 0 && result.SecondaryAlive == 0)
            {
                result.PrimaryWin = true;
            }
            else if (result.SecondaryAlive > 0 && result.PrimaryAlive == 0)
            {
                result.PrimaryWin = false;
            }
            else if (result.PrimaryAlive == 0 && result.SecondaryAlive == 0)
            {
                result.IsDraw = true;
            }
            else
            {
                result.TimedOut = true;
                if (result.PrimaryRemainingLife > result.SecondaryRemainingLife) result.PrimaryWin = true;
                else if (result.SecondaryRemainingLife > result.PrimaryRemainingLife) result.PrimaryWin = false;
                else result.IsDraw = true;
            }

            foreach (var u in units)
            {
                if (u.ChargeLevel > u.ChargeInitial)
                    result.ChargeDeltas[u.UniqueKey] = u.ChargeLevel - u.ChargeInitial;
            }
            return result;
        }

        public static void DiscretizeAxis(List<(float at, BattleBoardEventResponse ev)> axis)
        {
            if (axis == null || axis.Count == 0) return;
            var ordered = axis.OrderBy(e => e.at).ToList();
            axis.Clear();
            int i = 0;
            while (i < ordered.Count)
            {
                float at = ordered[i].at;
                int j = i;
                while (j < ordered.Count && ordered[j].at == at) j++;
                int sub = 0;
                for (int k = i; k < j; k++)
                {
                    int idxInGroup = k - i;
                    if (idxInGroup > 0 && idxInGroup % MaxEventsPerFrame == 0) sub++;
                    float newAt = at + sub * MinFrameGapSeconds;
                    var ev = ordered[k].ev;
                    ev.RaisedAt = newAt;
                    axis.Add((newAt, ev));
                }
                i = j;
            }
        }

        private static float RollBasicCrit(SimUnit a, Random rand)
        {
            float mult = 0f;
            foreach (var cs in a.CritSources)
            {
                float chance = cs.ChancePercent + cs.Stacks * cs.GrowthPercent;
                if (chance <= 0f) continue;
                if (rand.NextDouble() * 100.0 >= Math.Min(100f, chance)) continue;
                if (cs.GrowthPercent > 0f) cs.Stacks = 0;
                if (cs.Multiplier > mult) mult = cs.Multiplier;
            }
            return mult;
        }

        private static void ChangeTp(SimResult result, int matchId, float at, SimUnit u, int newTp,
            bool isSynergy, bool force = false)
        {
            int prev = u.CurrentTp;
            if (newTp == prev && !force) return;
            u.CurrentTp = newTp;
            if (u.MaxTp <= 0) return;
            result.Events.Add((at, new BattleBoardChangeTechnicalPointEventResponse
            {
                MatchId = matchId, RaisedAt = at, UniqueKey = u.UniqueKey,
                TechnicalPoint = newTp, Tp = newTp, IsSynergy = isSynergy, PreviousTp = prev,
            }));
        }

        private static void QueueDamageTp(SimResult result, SimUnit u, int incomingDamage)
        {
            if (incomingDamage <= 0 || u.MaxTp <= 0) return;

            int denom = Math.Max(1, u.BaseLife > 0 ? u.BaseLife : u.MaxLife);
            int amount = (int)Math.Round(DamageTpScale * u.MaxTp
                * Math.Min(1f, (float)incomingDamage / denom));
            if (amount <= 0) return;
            result.PendingDamageTp.Add((u.UniqueKey, amount));
        }

        private static void CancelLastDamageTp(SimResult result, int uniqueKey)
        {
            for (int i = result.PendingDamageTp.Count - 1; i >= 0; i--)
            {
                if (result.PendingDamageTp[i].UniqueKey == uniqueKey)
                {
                    result.PendingDamageTp.RemoveAt(i);
                    return;
                }
            }
        }

        private static void EmitUnitStatusSync(SimResult result, int matchId, float at, SimUnit u)
        {
            var current = new BoardUnitSyncStatusMpo
            {
                Attack = u.Attack,
                MagicAttack = u.Attack,
                Defense = u.Defense,
                MagicDefense = u.MagicDefence,
                AttackRecastTime = u.EffectiveAttackRecast,
                MoveRecastTime = u.EffectiveMoveRecast,
                AttackRecastTimeTickRate = u.AttackRecastTickRate,
                MoveRecastTimeTickRate = 1f,
                MaxTechnicalPoint = u.MaxTp,
            };
            result.Events.Add((at, new BattleBoardUnitStatusSyncEventResponse
            {
                MatchId = matchId, RaisedAt = at,
                UniqueKey = u.UniqueKey,
                UnitStatus = current,
                PreviousUnitStatus = u.LastSyncStatus ?? current,
            }));
            u.LastSyncStatus = current;
        }

        private static void EmitUnreportedDeaths(SimResult result, int matchId, float at,
            List<SimUnit> units, int tickEventMark)
        {
            foreach (var u in units)
            {

                if (u.Alive) { u.DeadEmitted = false; continue; }
                if (u.DeadEmitted) continue;
                for (int i = tickEventMark; i < result.Events.Count; i++)
                    if (DeathReportTarget(result.Events[i].ev) == u.UniqueKey)
                    {
                        u.DeadEmitted = true;
                        break;
                    }
                if (u.DeadEmitted) continue;
                result.Events.Add((at, new BattleBoardSkillDamageEventResponse
                {
                    MatchId = matchId, RaisedAt = at,

                    OperatorUniqueKey = u.UniqueKey, TargetUniqueKey = u.UniqueKey,
                    Life = 0, Damage = 0, SkillId = 0,
                    IsDead = true, SwitchMainEffect = false,
                    DamageType = DamageType.None, PreviousLife = 1,
                    IsCounterDamage = false,
                }));
                u.DeadEmitted = true;
                Console.WriteLine($"[SIM] death backfill unit={u.UnitId} key={u.UniqueKey} at={at:0.00}");
            }
        }

        static int DeathReportTarget(BattleBoardEventResponse ev)
        {
            switch (ev)
            {
                case BattleBoardAttackOperateEventResponse a: return a.IsDead ? a.TargetUniqueKey : 0;
                case BattleBoardSkillDamageEventResponse s: return s.IsDead ? s.TargetUniqueKey : 0;
                case BattleBoardEquipmentSkillDamageEventResponse q: return q.IsDead ? q.TargetUniqueKey : 0;
                case BattleBoardHpDamageEventResponse d: return d.IsDead ? d.TargetUniqueKey : 0;
                case BattleBoardCounterSkillDamageEventResponse c: return c.IsDead ? c.TargetUniqueKey : 0;
                default: return 0;
            }
        }

        public static void EnforceFrameRevisions(List<(float at, BattleBoardEventResponse ev)> axis)
        {
            if (axis == null || axis.Count == 0) return;
            var ordered = axis.OrderBy(e => e.at).ToList();
            axis.Clear();
            var lastRevision = new Dictionary<long, float>();
            var lastTpRevision = new Dictionary<long, float>();
            float floorAt = 0f;
            for (int i = 0; i < ordered.Count; i++)
            {
                var ev = ordered[i].ev;

                float at = Math.Max(1e-6f, ordered[i].at);
                if (at < floorAt) at = floorAt;
                long? owner = HpBarOwner(ev);
                if (owner != null)
                {
                    if (lastRevision.TryGetValue(owner.Value, out float prev) && at <= prev)
                        at = NextFloatRevision(prev);
                    lastRevision[owner.Value] = at;
                }

                long? tpOwner = TpBarOwner(ev);
                if (tpOwner != null)
                {
                    if (lastTpRevision.TryGetValue(tpOwner.Value, out float tpPrev) && at <= tpPrev)
                        at = NextFloatRevision(tpPrev);
                    lastTpRevision[tpOwner.Value] = at;
                }
                floorAt = at;
                ev.RaisedAt = at;
                axis.Add((at, ev));
            }
        }

        static long? HpBarOwner(BattleBoardEventResponse ev)
        {
            int t;
            switch (ev)
            {
                case BattleBoardAttackOperateEventResponse a: t = a.TargetUniqueKey; break;
                case BattleBoardHpDamageEventResponse d: t = d.TargetUniqueKey; break;
                case BattleBoardHpHealEventResponse h: t = h.TargetUniqueKey; break;
                case BattleBoardSkillDamageEventResponse s: t = s.TargetUniqueKey; break;
                case BattleBoardCounterSkillDamageEventResponse c: t = c.TargetUniqueKey; break;
                case BattleBoardEquipmentSkillDamageEventResponse q: t = q.TargetUniqueKey; break;
                case BattleBoardSkillHpHealEventResponse g: t = g.TargetUniqueKey; break;
                case BattleBoardStunEventResponse u: t = u.TargetUniqueKey; break;
                default: return null;
            }
            return ((long)ev.MatchId << 32) | (uint)t;
        }

        static long? TpBarOwner(BattleBoardEventResponse ev)
        {
            if (ev is BattleBoardChangeTechnicalPointEventResponse t)
                return ((long)ev.MatchId << 32) | (uint)t.UniqueKey;
            return null;
        }

        static float NextFloatRevision(float prev)
        {
            var bytes = BitConverter.GetBytes(prev);
            for (int i = 0; i < bytes.Length; i++)
            {
                unchecked { bytes[i]++; }
                if (bytes[i] != 0) break;
            }
            return BitConverter.ToSingle(bytes, 0);
        }

        private static SimUnit MakeUnit(BattleUnitDataMpo u, BattleUnitCalculatedStatusSetMpo status, bool isPrimary,
            IReadOnlyDictionary<int, BattleGuildBuffEffectInfo> guildBuffEffects,
            IReadOnlyList<ActiveSynergyEffect> sideSynergies = null)
        {
            var st = FindStatus(status, u.UniqueKey);
            var info = BattleMaster.GetUnit(u.UnitId);

            var guildBuff = guildBuffEffects != null && guildBuffEffects.TryGetValue(u.UnitId, out var gb) ? gb : null;
            int life = st?.Life > 0 ? st.Life : (info?.Life > 0 ? info.Life : 1);
            int attack = st?.Attack > 0 ? st.Attack : (info?.Atk ?? 0);
            int defense = st != null ? st.Defense : (info?.Defence ?? 0);

            int magicDefense = st != null ? st.MagicDefense : (info?.MagicDefence ?? 0);
            float attackRecast = st?.AttackRecastTime > 0f ? st.AttackRecastTime : (info?.AttackRecastTime > 0f ? info.AttackRecastTime : 1f);

            if (guildBuff != null && st == null)
            {
                life += guildBuff.Life;
                attack += guildBuff.Atk;
                defense += guildBuff.Defence;
                magicDefense += guildBuff.MagicDefence;
            }

            float recastTickRateSum = st == null ? (guildBuff?.AttackRecastTimeTickRate ?? 0) : 0f;
            float moveTickRateSum = 0f;
            double skillMultChain = 1.0;
            float tpRateSum = guildBuff?.TpRatePercent ?? 0f;

            bool guaranteedHit = false, criticalEvasion = false;
            float lifestealPercent = 0f, maxLifeDotPercent = 0f, reflectPercent = 0f;

            int reflectPassiveId = 0;
            int nthEvery = 0, nthTargets = 0; float nthMult = 0f;
            float auraSlowMult = 1f, auraSlowRadius = 0f, skillTakenMult = 1f;

            float gearNukeDamage = 0f, gearNukeRadius = 0f;
            int gearNukeTargets = 0, gearNukePassiveId = 0;
            int gearStunMaxFires = 0; float gearStunSeconds = 0f;
            int gearReviveHp = 0, gearReviveMax = 0;
            float gearSealChance = 0f, gearSealSeconds = 0f;
            int gearLowHpBelow = 0, gearLowHpAmount = 0;
            float gearLowHpDelay = 0f, gearLowHpRadius = 0f;
            float gearSkillEndTpChance = 0f;
            int gearSkillEndTpAmount = 0;
            float gearSkillDotPct = 0f, gearSkillDotSec = 0f, gearSkillDotCut = 0f;

            float? attackRangeOverride = null;
            var critSources = new List<CritSource>();
            if (u.EquipmentList != null)
            {
                foreach (var gear in u.EquipmentList)
                {
                    var gi = gear != null ? BattleMaster.GetEquipment(gear.Id) : null;
                    if (gi == null) continue;

                    if (st == null)
                    {
                        life += gi.Life;
                        attack += gi.Atk;
                        defense += gi.Defence;

                        magicDefense += gi.MagicDefence;

                        recastTickRateSum += gi.AttackRecastTimeTickRate;
                        moveTickRateSum += gi.MoveRecastTimeTickRate;
                    }
                    if (gi.SkillDamageScale != 0) skillMultChain *= 1.0 + gi.SkillDamageScale / 100.0;
                    tpRateSum += gi.TechnicalPointRate;

                    foreach (var pid in BattleMaster.GetEquipmentPassiveIds(gear.Id))
                    {
                        var pa = BattleMaster.GetPassiveSkillAction(pid);
                        if (pa == null) continue;
                        switch (pa.SkillActionType)
                        {
                            case 200000021:
                                if (pa.Value(1) > 0f)
                                {
                                    float baseRange = st?.AttackRange > 0f ? st.AttackRange : (info?.AttackRange > 0f ? info.AttackRange : 1f);
                                    attackRangeOverride = Math.Max(baseRange, baseRange * pa.Value(1) / 100f);
                                }
                                break;
                            case 200000023: guaranteedHit = true; break;
                            case 200000044: criticalEvasion = true; break;
                            case 200000003: lifestealPercent += pa.Value(1); break;
                            case 200000042: maxLifeDotPercent = Math.Max(maxLifeDotPercent, pa.Value(1)); break;
                            case 200000008:

                                if (pa.Value(1) >= reflectPercent)
                                {
                                    reflectPercent = pa.Value(1);
                                    reflectPassiveId = pa.Id;
                                }
                                break;
                            case 200000026:
                                nthEvery = Math.Max(nthEvery, (int)pa.Value(1));
                                nthMult = Math.Max(nthMult, pa.Value(2) / 100f);
                                nthTargets = Math.Max(nthTargets, pa.TargetCountForRank1);
                                break;
                            case 200000019:
                                if (pa.Value(1) > 0f) auraSlowMult = Math.Max(auraSlowMult, 100f / pa.Value(1));

                                auraSlowRadius = Math.Max(auraSlowRadius, BattleMaster.GetEquipmentSkillRange(gear.Id));
                                break;
                            case 200000010:
                                skillTakenMult *= Math.Max(0f, (100f - pa.Value(1)) / 100f);
                                break;
                            case 200000029:
                                tpRateSum += pa.Value(1);
                                break;
                            case 10170111:

                                critSources.Add(new CritSource { ChancePercent = pa.Value(1), Multiplier = pa.Value(2) > 0f ? pa.Value(2) / 100f : 1f });
                                break;
                            case 200000034:

                                critSources.Add(new CritSource
                                {
                                    ChancePercent = pa.Value(1),
                                    Multiplier = pa.Value(2) > 0f ? pa.Value(2) / 100f : 1f,
                                    GrowthPercent = pa.Value(3),
                                    NextAt = CountToSeconds,
                                });
                                break;

                            case 200000099:

                                if (pa.Value(1) >= gearNukeDamage)
                                {
                                    gearNukeDamage = pa.Value(1);
                                    gearNukePassiveId = pa.Id;
                                }
                                gearNukeRadius = Math.Max(gearNukeRadius, BattleMaster.GetEquipmentSkillRange(gear.Id));
                                gearNukeTargets = Math.Max(gearNukeTargets, pa.TargetCountForRank1);
                                break;
                            case 200000012:
                                gearStunMaxFires = Math.Max(gearStunMaxFires, (int)pa.Value(1));
                                gearStunSeconds = Math.Max(gearStunSeconds, pa.Value(2));
                                break;
                            case 200000015:
                                gearReviveHp = Math.Max(gearReviveHp, (int)pa.Value(1));
                                gearReviveMax = Math.Max(gearReviveMax, (int)pa.Value(2));
                                break;
                            case 200000046:
                                gearSealChance = Math.Max(gearSealChance, pa.Value(1));
                                gearSealSeconds = Math.Max(gearSealSeconds, pa.Value(2));
                                break;
                            case 200000032:
                                gearLowHpBelow = Math.Max(gearLowHpBelow, (int)pa.Value(1));
                                gearLowHpAmount = Math.Max(gearLowHpAmount, (int)pa.Value(2));
                                gearLowHpDelay = Math.Max(gearLowHpDelay, pa.Value(3));
                                gearLowHpRadius = Math.Max(gearLowHpRadius, BattleMaster.GetEquipmentSkillRange(gear.Id));
                                break;
                            case 200000006:
                                gearSkillEndTpChance = Math.Max(gearSkillEndTpChance, pa.Value(1));
                                gearSkillEndTpAmount = Math.Max(gearSkillEndTpAmount, (int)pa.Value(2));
                                break;
                            case 200000048:
                                gearSkillDotPct = Math.Max(gearSkillDotPct, pa.Value(1));
                                gearSkillDotSec = Math.Max(gearSkillDotSec, pa.Value(2));
                                gearSkillDotCut = Math.Max(gearSkillDotCut, pa.Value(3));
                                break;
                        }
                    }
                }
            }

            float paUntargetableUntil = 0f, paAttackDisabledUntil = 0f;
            float paSilenceUntil = 0f, paRootUntil = 0f;
            bool paBossResistance = false;
            float paAttackStunChance = 0f, paAttackStunSeconds = 0f;
            float paSpeedStackPercent = 0f; int paSpeedStackMax = 0;
            float paTpDownOnHit = 0f; int selfDestructEvery = 0;
            bool paWeaponChange = false;
            if (info != null)
            {
                foreach (var pid in BattleMaster.UnitPassiveSkillActionIds(u.UnitId))
                {
                    if (pid == 0) continue;
                    var pa = BattleMaster.GetPassiveSkillAction(pid);
                    if (pa == null) continue;
                    switch (pa.SkillActionType)
                    {
                        case 31010011:

                            paBossResistance = true;
                            break;
                        case 90000111:
                            paUntargetableUntil = Math.Max(paUntargetableUntil, pa.Value(1));
                            break;

                        case 90000112:

                            paAttackDisabledUntil = Math.Max(paAttackDisabledUntil, pa.Value(1));
                            paSilenceUntil = Math.Max(paSilenceUntil, pa.Value(1));
                            paRootUntil = Math.Max(paRootUntil, pa.Value(1));
                            break;
                        case 10032:
                            paAttackStunChance = Math.Max(paAttackStunChance, pa.Value(1));
                            paAttackStunSeconds = Math.Max(paAttackStunSeconds, pa.Value(2));
                            break;
                        case 10170111:

                            critSources.Add(new CritSource { ChancePercent = pa.Value(1), Multiplier = pa.Value(2) > 0f ? pa.Value(2) / 100f : 1f });
                            break;
                        case 10370111:
                            paSpeedStackPercent = Math.Max(paSpeedStackPercent, pa.Value(1));
                            paSpeedStackMax = Math.Max(paSpeedStackMax, (int)pa.Value(2));
                            break;
                        case 10550111:
                            paTpDownOnHit = Math.Max(paTpDownOnHit, pa.Value(1));
                            break;
                        case 99994:

                            selfDestructEvery = Math.Max(selfDestructEvery, Math.Max(1, (int)pa.Value(1)));
                            break;
                        case 10610111:

                            paWeaponChange = true;
                            break;
                        case 10700111:
                            break;
                    }
                }
            }

            float moveRecast = st?.MoveRecastTime > 0f ? st.MoveRecastTime : (info?.MoveRecastTime > 0f ? info.MoveRecastTime : 1f);
            if (moveTickRateSum != 0)
                moveRecast = Math.Max(0.2f, moveRecast / Math.Max(0.01f, 1f + moveTickRateSum / 100f));

            float attackPercentSum = 0f;
            int openTp = 0;

            int hunterEvery = 0, hunterTargets = 0, hunterDamage = 0;
            int lightningEvery = 0, lightningTargets = 0, lightningDamage = 0;
            float stunChance = 0f, stunSeconds = 0f;
            int tpDown = 0;
            float extraDamagePercent = 0f;
            float oldHunterChance = 0f, oldHunterInterval = 0f, oldHunterDuration = 0f, oldHunterSpeed = 0f;

            float role103Chance = 0f;
            float role105Chance = 0f, role105Interval = 0f, role105Duration = 0f, role105Cut = 0f;
            float executeBelow = 0f;
            float synLifesteal = 0f, synAttackHeal = 0f;
            float team208Below = 0f, team208Seconds = 0f;
            float team209Speed = 0f, team209Heal = 0f;

            float evasionChance = 0f;
            float magicDefPercentSum = 0f;
            float[] chargeSealValues = null;

            var synergyRoleSet = new HashSet<int>(info?.RoleTypes ?? Array.Empty<int>());
            var synergyTeamSet = new HashSet<int>(info?.TeamTypes ?? Array.Empty<int>());
            if (u.MirroringRoleType != null)
                foreach (var r in u.MirroringRoleType) synergyRoleSet.Add((int)r);
            if (u.MirroringTeamType != null)
                foreach (var tp in u.MirroringTeamType) synergyTeamSet.Add((int)tp);
            if (u.EquipmentList != null)
                foreach (var g in u.EquipmentList)
                {
                    var ge = g != null ? BattleMaster.GetEquipment(g.Id) : null;
                    if (ge == null) continue;
                    if (ge.RoleType > 0) synergyRoleSet.Add(ge.RoleType);
                    if (ge.TeamType > 0) synergyTeamSet.Add(ge.TeamType);
                }
            var roleTypes = synergyRoleSet.ToArray();
            var teamTypes = synergyTeamSet.ToArray();
            if (sideSynergies != null)
            {
                foreach (var syn in sideSynergies)
                {
                    var v = syn.Values;
                    bool isMember = syn.SynergyType == 1
                        ? Array.IndexOf(roleTypes, syn.Id) >= 0
                        : Array.IndexOf(teamTypes, syn.Id) >= 0;
                    switch ((syn.SynergyType, syn.Id))
                    {
                        case (1, 101):

                            if (st == null && isMember && v.Length >= 1) defense += (int)v[0];
                            break;
                        case (1, 110):

                            if (st == null && v.Length >= 1) magicDefense += (int)v[0];
                            break;
                        case (0, 207):

                            if (st == null && v.Length >= 1) magicDefPercentSum += v[0];
                            break;
                        case (0, 201):
                            if (st == null && isMember && v.Length >= 1) life += (int)v[0];
                            break;
                        case (1, 104):
                            if (isMember && v.Length >= 1) skillMultChain *= 1.0 + v[0] / 100.0;
                            break;
                        case (1, 114):

                            if (isMember && v.Length >= 3)
                            {
                                chargeSealValues = v;

                                if (st == null)
                                {
                                    int seals = Math.Min(9, Math.Max(0, u.TotalSkillCount));
                                    if (seals > 0)
                                    {
                                        life += (int)(v[0] * seals);
                                        attack += (int)(v[1] * seals);
                                        defense += (int)(v[2] * seals);
                                    }
                                }
                            }
                            break;
                        case (0, 204):
                        case (0, 210):
                            if (st == null && v.Length >= 1) attackPercentSum += v[0];
                            break;
                        case (0, 216):

                            if (st == null && v.Length >= 4 && v[3] > 0f) recastTickRateSum += v[3] - 100f;
                            break;
                        case (1, 111):
                            if (isMember && v.Length >= 1)
                                openTp = syn.Level >= 2 ? int.MaxValue : (int)v[0];
                            break;
                        case (1, 113):
                            if (isMember && v.Length >= 3)
                            {
                                hunterEvery = (int)v[0]; hunterTargets = (int)v[1]; hunterDamage = (int)v[2];
                            }
                            break;
                        case (0, 205):
                            if (isMember && v.Length >= 3)
                            {
                                lightningEvery = (int)v[0]; lightningTargets = (int)v[1]; lightningDamage = (int)v[2];
                            }
                            break;
                        case (0, 206):
                            if (isMember && v.Length >= 2)
                            {
                                stunChance = v[0]; stunSeconds = v[1] * CountToSeconds;
                            }
                            break;
                        case (0, 211):
                            if (isMember && v.Length >= 1) tpDown = (int)v[0];
                            break;
                        case (0, 214):
                            if (isMember && v.Length >= 1) extraDamagePercent = v[0];
                            break;
                        case (1, 102):
                            if (isMember && v.Length >= 4)
                            {
                                oldHunterChance = v[0];
                                oldHunterInterval = v[1] * CountToSeconds;
                                oldHunterDuration = v[2] * CountToSeconds;
                                oldHunterSpeed = v[3];
                            }
                            break;
                        case (1, 106):
                            if (isMember && v.Length >= 2)
                            {

                                critSources.Add(new CritSource { ChancePercent = v[0], Multiplier = 1f + v[1] / 100f });
                            }
                            break;
                        case (0, 203):
                            if (v.Length >= 1) evasionChance += v[0];
                            break;

                        case (1, 103):
                            if (isMember && v.Length >= 1) role103Chance = v[0];
                            break;
                        case (1, 105):
                            if (isMember && v.Length >= 4)
                            {
                                role105Chance = v[0];
                                role105Interval = v[1] * CountToSeconds;
                                role105Duration = v[2] * CountToSeconds;
                                role105Cut = 100f - v[3];
                            }
                            break;
                        case (1, 107):
                            if (isMember && v.Length >= 1) executeBelow = v[0];
                            break;
                        case (1, 108):
                            if (v.Length >= 1) synLifesteal += v[0];
                            break;
                        case (1, 109):
                            if (v.Length >= 1) synAttackHeal += v[0];
                            break;
                        case (0, 208):
                            if (isMember && v.Length >= 2)
                            {
                                team208Below = v[0];
                                team208Seconds = v[1] * CountToSeconds;
                            }
                            break;
                        case (0, 209):
                            if (isMember && v.Length >= 2)
                            {
                                team209Speed = v[0];
                                team209Heal = v[1];
                            }
                            break;
                    }
                }

                if (recastTickRateSum != 0)
                    attackRecast = attackRecast / Math.Max(0.01f, 1f + recastTickRateSum / 100f);
            }
            if (attackPercentSum != 0f)
                attack = (int)Math.Round(attack * (1f + attackPercentSum / 100f));

            if (magicDefPercentSum != 0f)
                magicDefense = (int)Math.Round(magicDefense * (1f + magicDefPercentSum / 100f));
            var simUnit = new SimUnit
            {
                UniqueKey = u.UniqueKey,
                UnitId = u.UnitId,
                X = u.PositionX,
                Y = u.PositionY,
                Life = life,
                MaxLife = life,
                BaseLife = info?.Life > 0 ? info.Life : life,
                AttackBase = attack,
                DefenseBase = defense,
                MagicDefenceBase = magicDefense,
                AttackRange = attackRangeOverride ?? (st?.AttackRange > 0f ? st.AttackRange : (info?.AttackRange > 0f ? info.AttackRange : 1f)),
                AttackRecastTime = attackRecast,
                AttackClipSeconds = AnimClipTable.AttackSeconds(u.UnitId),
                MoveRecastTime = moveRecast,
                CurrentTp = openTp,
                MaxTp = st?.MaxTechnicalPoint > 0 ? st.MaxTechnicalPoint : (info?.TechnicalPoint ?? 0),
                SkillActionIds = BattleMaster.SkillActionIds(u.UnitId),
                Rank = u.Rank > 0 ? u.Rank : 1,
                IsPrimary = isPrimary,
                RoleTypes = info?.RoleTypes ?? Array.Empty<int>(),
                TeamTypes = info?.TeamTypes ?? Array.Empty<int>(),
                SynergyRoleTypes = roleTypes,
                SynergyTeamTypes = teamTypes,

                ChargeSealValues = chargeSealValues,
                ChargeLevel = Math.Min(9, Math.Max(0, u.TotalSkillCount)),
                ChargeInitial = Math.Min(9, Math.Max(0, u.TotalSkillCount)),

                UntargetableUntil = paUntargetableUntil,
                AttackDisabledUntil = paAttackDisabledUntil,
                SilenceUntil = paSilenceUntil,
                RootUntil = paRootUntil,
                BossResistance = paBossResistance,
                PaAttackStunChance = paAttackStunChance,
                PaAttackStunSeconds = paAttackStunSeconds,
                PaSpeedStackPercent = paSpeedStackPercent,
                PaSpeedStackMax = paSpeedStackMax,
                PaTpDownOnHit = paTpDownOnHit,
                SelfDestructEvery = selfDestructEvery,
                PaWeaponChange = paWeaponChange,
                SkillDamageMult = (float)skillMultChain,

                TpGainMult = 1f + tpRateSum / 100f,

                HunterEvery = hunterEvery, HunterTargets = hunterTargets, HunterDamage = hunterDamage,
                LightningEvery = lightningEvery, LightningTargets = lightningTargets, LightningDamage = lightningDamage,
                StunChancePercent = stunChance, StunSeconds = stunSeconds,
                TpDownPerHit = tpDown, ExtraDamagePercent = extraDamagePercent,
                OldHunterChancePercent = oldHunterChance, OldHunterIntervalSeconds = oldHunterInterval,
                OldHunterSpeedPercent = oldHunterSpeed, OldHunterDurationSeconds = oldHunterDuration,
                OldHunterNextAt = oldHunterInterval, AttackRecastBase = attackRecast,
                CritSources = critSources,
                EvasionChancePercent = Math.Min(100f, evasionChance),

                GuaranteedHit = guaranteedHit,
                CriticalEvasion = criticalEvasion,
                LifestealPercent = lifestealPercent,
                MaxLifeDotPercent = maxLifeDotPercent,
                ReflectPercent = reflectPercent,
                ReflectPassiveId = reflectPassiveId,
                NthAttackEvery = nthEvery,
                NthAttackMult = nthMult,
                NthAttackTargets = nthTargets,
                AuraEnemySlowMult = auraSlowMult,
                AuraEnemySlowRadius = auraSlowRadius,
                SkillDamageTakenMult = skillTakenMult,

                GearSkillNukeDamage = gearNukeDamage,
                GearSkillNukeRadius = gearNukeRadius,
                GearSkillNukeTargets = gearNukeTargets,
                GearSkillNukePassiveId = gearNukePassiveId,
                GearSkillStunMaxFires = gearStunMaxFires,
                GearSkillStunSeconds = gearStunSeconds,
                GearReviveHpPercent = gearReviveHp,
                GearReviveMaxFires = gearReviveMax,
                GearAttackSealChance = gearSealChance,
                GearAttackSealSeconds = gearSealSeconds,
                GearLowHpHealBelow = gearLowHpBelow,
                GearLowHpHealAmount = gearLowHpAmount,
                GearLowHpHealDelay = gearLowHpDelay,
                GearLowHpHealRadius = gearLowHpRadius,
                GearSkillEndTpChance = gearSkillEndTpChance,
                GearSkillEndTpAmount = gearSkillEndTpAmount,
                GearSkillDotPercent = gearSkillDotPct,
                GearSkillDotSeconds = gearSkillDotSec,
                GearSkillDotHealCut = gearSkillDotCut,

                Role103Chance = role103Chance,
                Role105Chance = role105Chance,
                Role105IntervalSeconds = role105Interval,
                Role105DurationSeconds = role105Duration,
                Role105CutPercent = role105Cut,
                Role105NextAt = role105Interval,
                ExecuteBelowPercent = executeBelow,
                SynergyLifestealPercent = synLifesteal,
                SynergyAttackHealPercent = synAttackHeal,
                Team208BelowPercent = team208Below,
                Team208InvincibleSeconds = team208Seconds,
                Team209SpeedPerDeath = team209Speed,
                Team209HealPercent = team209Heal,

                Team209DeathCredited = life <= 0,
            };
            if (openTp == int.MaxValue) simUnit.CurrentTp = simUnit.MaxTp;
            return simUnit;
        }

        private static BoardUnitStatusMpo FindStatus(BattleUnitCalculatedStatusSetMpo set, int uniqueKey)
        {
            if (set?.UnitStatuses == null) return null;
            foreach (var st in set.UnitStatuses)
                if (st.UniqueKey == uniqueKey) return st.Current;
            return null;
        }

        private static void ApplyEnemyDefenseDown(List<SimUnit> units,
            IReadOnlyList<ActiveSynergyEffect> sideSynergies, bool isPrimarySide)
        {
            if (sideSynergies == null) return;
            foreach (var syn in sideSynergies)
            {
                if (syn.SynergyType != 0 || syn.Id != 212 || syn.Values.Length < 1 || syn.Values[0] <= 0f) continue;
                float down = syn.Values[0];
                foreach (var u in units)
                {
                    if (u.Alive && u.IsPrimary != isPrimarySide)
                        u.DefenseBase = (int)Math.Round(u.DefenseBase * (1f - down / 100f));
                }
            }
        }

        private static void ResolveTeam202(IReadOnlyList<ActiveSynergyEffect> sideSynergies,
            out float intervalSeconds, out int amount)
        {
            intervalSeconds = 0f; amount = 0;
            if (sideSynergies == null) return;
            foreach (var syn in sideSynergies)
            {
                if (syn.SynergyType == 0 && syn.Id == 202 && syn.Values.Length >= 2 && syn.Values[0] > 0f)
                {
                    intervalSeconds = syn.Values[0] * CountToSeconds;
                    amount = (int)syn.Values[1];
                    return;
                }
            }
        }

        private static void DealRandomSpecialDamage(SimUnit attacker, List<SimUnit> units,
            int targetCount, int damage, Random rand,
            SimResult result, int matchId, float at, SimUnit attackTarget)
        {
            var pool = units.Where(u => u.Alive && u.IsPrimary != attacker.IsPrimary)
                .OrderBy(u => u.UniqueKey).ToList();
            for (int i = 0; i < targetCount && pool.Count > 0; i++)
            {
                int idx = rand.Next(pool.Count);
                var victim = pool[idx];
                int prev = victim.Life;
                int dealt = ApplyGearReviveGuard(victim, prev, damage, result, matchId, at);

                if (dealt > 0 && !ReferenceEquals(victim, attacker)) QueueDamageTp(result, victim, damage);
                pool.RemoveAt(idx);
                if (ReferenceEquals(victim, attackTarget)) continue;
                result.Events.Add((at, new BattleBoardHpDamageEventResponse
                {
                    MatchId = matchId, RaisedAt = at,
                    OperatorUniqueKey = attacker.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                    Life = victim.Life, Damage = prev - victim.Life,
                    IsDead = victim.Life <= 0,
                    AttackType = AttackType.None, PreviousLife = prev,
                }));
            }
        }

        private static void EmitSpecialDamageFrame(SimResult result, int matchId, float at,
            SimUnit attacker, SimUnit victim, int damage)
        {
            if (damage <= 0 || victim.Life <= 0) return;
            int prev = victim.Life;
            int dealt = ApplyGearReviveGuard(victim, prev, damage, result, matchId, at);

            if (dealt > 0 && !ReferenceEquals(victim, attacker)) QueueDamageTp(result, victim, damage);
            result.Events.Add((at, new BattleBoardHpDamageEventResponse
            {
                MatchId = matchId, RaisedAt = at,
                OperatorUniqueKey = attacker.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                Life = victim.Life, Damage = prev - victim.Life,
                IsDead = victim.Life <= 0,
                AttackType = AttackType.None, PreviousLife = prev,
            }));
        }

        private static void EmitGearReflectFrame(SimResult result, int matchId, float at,
            SimUnit wearer, SimUnit attacker, int damage)
        {
            if (damage <= 0) return;
            int prev = attacker.Life;
            int dealt = ApplyGearReviveGuard(attacker, prev, damage, result, matchId, at);

            if (dealt > 0 && !ReferenceEquals(attacker, wearer)) QueueDamageTp(result, attacker, damage);

            result.Events.Add((at, new BattleBoardHpDamageEventResponse
            {
                MatchId = matchId, RaisedAt = at,
                OperatorUniqueKey = wearer.UniqueKey, TargetUniqueKey = attacker.UniqueKey,
                Life = attacker.Life, Damage = prev - attacker.Life, IsDead = attacker.Life <= 0,
                AttackType = AttackType.None, PreviousLife = prev,
            }));
        }

        private static SimUnit SelectTarget(SimUnit self, List<SimUnit> units, float t)
        {

            if (t < self.CharmUntil)
                return NearestByKey(self, units, t, friendlySide: true);
            if (t < self.TauntUntil)
            {

                var src = units.FirstOrDefault(u => u.Alive && t >= u.UntargetableUntil && u.UniqueKey == self.TauntSourceUniqueKey && u.IsPrimary != self.IsPrimary);
                if (src != null) return src;
            }

            if (self.RetaliateTarget != null)
            {
                var rt = self.RetaliateTarget;
                if (rt.Alive && rt.IsPrimary != self.IsPrimary && t >= rt.UntargetableUntil)
                {
                    if (HexDistance(self, rt) <= self.EffectiveAttackRange)
                    {
                        self.TargetUniqueKey = rt.UniqueKey;
                        self.OutOfRangeSteps = 0;
                        return rt;
                    }
                }
                else if (!rt.Alive || rt.IsPrimary == self.IsPrimary) self.RetaliateTarget = null;
            }

            if (self.TargetUniqueKey != 0)
            {
                var locked = units.FirstOrDefault(u => u.UniqueKey == self.TargetUniqueKey);
                if (locked != null && locked.Alive && t >= locked.UntargetableUntil)
                {

                    if (HexDistance(self, locked) > self.EffectiveAttackRange)
                    {
                        if (self.OutOfRangeSteps >= RetargetStepTolerance)
                        {
                            self.TargetUniqueKey = 0;
                            self.OutOfRangeSteps = 0;

                        }
                        else return locked;
                    }
                    else
                    {
                        self.OutOfRangeSteps = 0;
                        return locked;
                    }
                }
                else
                {
                    self.TargetUniqueKey = 0;
                }
            }
            var best = NearestByKey(self, units, t, friendlySide: false);
            if (best != null)
            {
                self.TargetUniqueKey = best.UniqueKey;
                self.OutOfRangeSteps = 0;
            }
            return best;
        }

        private static SimUnit NearestByKey(SimUnit self, List<SimUnit> units, float t, bool friendlySide)
        {
            SimUnit best = null;
            foreach (var u in units)
            {
                if (!u.Alive) continue;
                if (ReferenceEquals(u, self)) continue;
                if (t < u.UntargetableUntil) continue;
                if (friendlySide ? u.IsPrimary != self.IsPrimary : u.IsPrimary == self.IsPrimary) continue;
                if (best == null) { best = u; continue; }
                int d = HexDistance(self, u), bd = HexDistance(self, best);
                if (d < bd || (d == bd && (u.Life < best.Life || (u.Life == best.Life && u.UniqueKey < best.UniqueKey))))
                    best = u;
            }
            return best;
        }

        private static void PrimeTargets(List<SimUnit> units)
        {
            foreach (var u in units)
            {
                if (!u.Alive) continue;
                u.TargetUniqueKey = 0;
                SelectTarget(u, units, 0f);
            }
        }

        private static void TryRetargetIfUnreachable(SimUnit a, List<SimUnit> units)
        {
            var old = a.TargetUniqueKey;
            SimUnit locked = null;
            if (old != 0)
                foreach (var u in units)
                    if (u.UniqueKey == old) { locked = u; break; }
            if (locked != null && locked.Alive
                && TryFindApproach(a, locked, a.EffectiveAttackRange, units, TargetProbeSteps,
                                   out _, out _, out bool reachable)
                && reachable)
                return;
            var next = NearestReachableTarget(a, units);
            if (next == null || next.UniqueKey == old) return;

            if (locked != null && ReferenceEquals(a.RetaliateTarget, locked)) a.RetaliateTarget = null;
            a.TargetUniqueKey = next.UniqueKey;
            if (VerboseSimLog) Console.WriteLine($"[SIM] target-unlock key={a.UniqueKey} unit={a.UnitId} stuck=blocked old={old} next={a.TargetUniqueKey}");
        }

        private static SimUnit NearestReachableTarget(SimUnit self, List<SimUnit> units)
        {
            var candidates = new List<SimUnit>();
            foreach (var u in units)
            {
                if (!u.Alive || ReferenceEquals(u, self)) continue;
                if (u.IsPrimary == self.IsPrimary) continue;
                candidates.Add(u);
            }
            candidates.Sort((x, y) =>
            {
                int dx = HexDistance(self, x), dy = HexDistance(self, y);
                if (dx != dy) return dx.CompareTo(dy);
                if (x.Life != y.Life) return x.Life.CompareTo(y.Life);
                return x.UniqueKey.CompareTo(y.UniqueKey);
            });
            foreach (var c in candidates)
                if (TryFindApproach(self, c, self.EffectiveAttackRange, units, TargetProbeSteps,
                                    out _, out _, out bool reachable)
                    && reachable)
                    return c;
            return null;
        }

        private static bool TryFindApproach(SimUnit a, SimUnit target, float reach,
            List<SimUnit> units, int maxSteps, out int stepX, out int stepY, out bool inRange,
            bool avoidRecent = false)
        {
            stepX = a.X; stepY = a.Y; inRange = false;

            if (HexDistance(a, target) <= reach) { inRange = true; return true; }
            var blocked = new bool[BoardMaxX + 1, BoardMaxY + 1];
            foreach (var u in units)
                if (u.Alive && !ReferenceEquals(u, a)) blocked[u.X, u.Y] = true;
            if (avoidRecent)
                foreach (var c in a.RecentCells)
                    if (c.x >= 0 && c.x <= BoardMaxX && c.y >= 0 && c.y <= BoardMaxY)
                        blocked[c.x, c.y] = true;
            var steps = new int[BoardMaxX + 1, BoardMaxY + 1];
            for (int x = 0; x <= BoardMaxX; x++)
                for (int y = 0; y <= BoardMaxY; y++) steps[x, y] = -1;
            var prev = new (int X, int Y)[BoardMaxX + 1, BoardMaxY + 1];
            var queue = new Queue<(int X, int Y)>();
            steps[a.X, a.Y] = 0;
            queue.Enqueue((a.X, a.Y));
            int curDist = HexDistance(a, target);
            int hitX = -1, hitY = -1, hitKey = int.MaxValue;
            int nearX = -1, nearY = -1, nearKey = int.MaxValue;
            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();
                int cd = steps[cx, cy];
                if (cd >= maxSteps) continue;
                var offsets = (cy & 1) == 0 ? HexEvenOffsets : HexOddOffsets;
                foreach (var o in offsets)
                {
                    int nx = cx + o.dx, ny = cy + o.dy;
                    if (nx < 0 || nx > BoardMaxX || ny < 0 || ny > BoardMaxY) continue;
                    if (blocked[nx, ny] || steps[nx, ny] >= 0) continue;
                    steps[nx, ny] = cd + 1;
                    prev[nx, ny] = (cx, cy);
                    int hd = HexDistanceTo(nx, ny, target.X, target.Y);
                    if (hd <= reach)
                    {
                        int key = ((cd + 1) * 16 + hd) * 128 + nx * 16 + ny;
                        if (key < hitKey) { hitKey = key; hitX = nx; hitY = ny; }
                    }
                    else if (hd < curDist)
                    {
                        int key = (hd * 16 + (cd + 1)) * 128 + nx * 16 + ny;
                        if (key < nearKey) { nearKey = key; nearX = nx; nearY = ny; }
                    }
                    queue.Enqueue((nx, ny));
                }
            }
            int tx, ty;
            if (hitX >= 0) { tx = hitX; ty = hitY; inRange = true; }
            else if (nearX >= 0) { tx = nearX; ty = nearY; }
            else return false;
            while (steps[tx, ty] > 1)
            {
                var p = prev[tx, ty];
                tx = p.X; ty = p.Y;
            }
            stepX = tx; stepY = ty;
            return true;
        }

        private static bool IsDamageAction(int type)
            => type == 1 || type == 9 || type == 10 || type == 10004 ||
               type == 10015 || type == 10016 || type == 10029 || type == 10030 ||
               type == 10041 || type == 10046 || type == 10047 || type == 10048 ||
               type == 99992 || type == 99995;

        private static bool IsAttackSplashAction(int type) => type == 10049;
        private static bool IsCounterStanceAction(int type) => type == 10050;

        private static bool IsAttackPercentDamageAction(int type)
            => type == 10004 || type == 10015 || type == 10041;

        private static int DamageValueRow(int type)
            => type == 10016 || type == 10029 || type == 10046 ? 2 : 1;

        private static bool IsDistanceFalloffAction(int type) => type == 10016 || type == 10029;
        private static bool IsPierceFalloffAction(int type) => type == 10046;

        private static float SkillFalloffMult(BattleSkillActionInfo action, SimUnit caster, SimUnit target,
            List<SimUnit> targets, int rank)
        {
            int type = action.SkillActionType;
            bool distance = IsDistanceFalloffAction(type);
            if (!distance && !IsPierceFalloffAction(type)) return 1f;
            float pct = action.Value(3, rank);
            if (pct <= 0f) return 1f;
            int steps;
            if (distance)
            {
                steps = Math.Max(0, HexDistance(caster, target) - 1);
            }
            else
            {
                int td = HexDistance(caster, target);
                steps = 0;
                foreach (var u in targets)
                {
                    if (ReferenceEquals(u, target)) continue;
                    int ud = HexDistance(caster, u);
                    if (ud < td || (ud == td && u.UniqueKey < target.UniqueKey)) steps++;
                }
            }
            return (float)Math.Pow(1.0 - Math.Min(100f, pct) / 100.0, steps);
        }

        private static bool IsSpecialDamageAction(int type)
            => type == 10 || type == 10041 || type == 10049 || type == 10050 || type == 99995;

        private static bool IsMagicDamageAction(int type)
            => type == 9 || type == 10016 || type == 10029 || type == 10030 ||
               type == 10046 || type == 10047 || type == 99992;

        private static bool IsHealAction(int type)
            => type == 2 || type == 99993 || type == 10052;

        private static bool IsPeriodicDamageAction(int type)
            => type == 99992 || type == 99995;

        private static bool IsPeriodicHealAction(int type)
            => type == 99993 || type == 10052;

        private static bool IsStunAction(int type)
            => type == 10003 || type == 10053;

        private static bool IsStatAction(int type)
            => type == 3 || type == 13 || type == 14;

        private static bool IsBarrierAction(int type)
            => type == 10002 || type == 10033;

        private static bool IsInvincibleAction(int type)
            => type == 10021 || type == 10017 || type == 10044;

        private static bool IsCharmAction(int type) => type == 10018;
        private static bool IsTauntAction(int type) => type == 10020;

        private static bool IsMoveAction(int type) => type == 10034 || type == 10023;

        private static bool IsTpGainAction(int type) => type == 5;

        private static bool IsSummonAction(int type) => type == 7;

        private static string StatNameByParameter(int parameter) => parameter switch
        {
            1 => StatAttack,
            3 => StatDefense,
            4 => StatMagicDefense,
            6 => StatMoveRecast,
            7 => StatAttackRecast,
            8 => StatAttackRecast,
            9 => StatAttackRange,
            10 => StatTpRate,
            _ => null,
        };

        private static void ApplyStatAction(SimUnit a, float at, float t, BattleSkillActionInfo action, List<SimUnit> targets, List<SimUnit> units, SimResult result, int matchId)
        {
            int atype = action.SkillActionType;
            var affected = targets;
            string stat;
            bool percent;
            float magnitude;
            float duration;
            StatusEffectVfxType vfx;
            if (atype == 10045)
            {
                stat = StatLifesteal;
                percent = false;
                magnitude = action.Value(1, a.Rank);
                duration = action.Value(2, a.Rank);
                vfx = StatusEffectVfxType.ExtraAttackDamage;
            }
            else if (atype == 10036)
            {

                if (targets.Count == 0) return;
                var center = targets[0];
                float auraRadius = Math.Max(0f, action.Value(2, a.Rank));
                float auraInterval = Math.Max(0.01f, action.Value(3, a.Rank));
                float auraDuration = action.Value(4, a.Rank);
                if (auraDuration <= 0f) return;
                var carried = center.Periodics.FirstOrDefault(p =>
                    p.ActionId == action.Id && p.SourceKey == a.UniqueKey && p.StatAura);
                if (carried != null)
                {

                    carried.NextTickAt = Math.Min(carried.NextTickAt, t + auraInterval);
                    carried.ExpiresAt = t + auraDuration;
                    return;
                }
                var aura = new SimUnit.PeriodicEffect
                {
                    ActionId = action.Id,
                    SourceKey = a.UniqueKey,
                    StatAura = true,
                    AuraRadius = auraRadius,
                    AuraStat = StatDefense,
                    AuraPercent = true,
                    AuraMagnitude = -Math.Abs(action.Value(1, a.Rank)),
                    AuraVfx = StatusEffectVfxType.DownStatusDefense,
                    Interval = auraInterval,
                    NextTickAt = t + auraInterval,
                    ExpiresAt = t + auraDuration,
                };
                center.Periodics.Add(aura);
                TickStatAura(center, aura, at, units, result, matchId);
                return;
            }
            else
            {
                magnitude = action.Value(1, a.Rank);
                int parameter = (int)action.Value(2, a.Rank);
                percent = atype != 3;
                if (atype == 14) magnitude = -magnitude;
                if (parameter == 7)
                {

                    float speed = magnitude;
                    if (speed <= -99f) speed = -99f;
                    magnitude = -100f * speed / (100f + speed);
                }
                else if (parameter == 8) magnitude = -magnitude;
                stat = StatNameByParameter(parameter);
                if (stat == null) return;
                duration = action.Value(3, a.Rank);
                vfx = VfxFor(stat, magnitude);
            }
            foreach (var target in affected)
            {
                target.StatMods.Add(new SimUnit.StatMod
                {
                    Stat = stat,
                    Percent = percent,
                    Magnitude = magnitude,
                    ExpiresAt = duration > 0f ? t + duration : 0f,
                    Vfx = vfx,
                });
                result.Events.Add((at, new BattleBoardStatusEffectAddedEventResponse
                {
                    MatchId = matchId, RaisedAt = at,
                    UniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                    SkillActionId = action.Id,
                    StatusEffectVfxType = vfx, SynergyId = 0,
                }));
                EmitUnitStatusSync(result, matchId, at, target);
            }
        }

        private static StatusEffectVfxType VfxFor(string stat, float signedMagnitude)
        {
            bool up = signedMagnitude >= 0f;
            switch (stat)
            {
                case StatAttack: return up ? StatusEffectVfxType.UpStatusAttack : StatusEffectVfxType.DownStatusAttack;
                case StatDefense: return up ? StatusEffectVfxType.UpStatusDefence : StatusEffectVfxType.DownStatusDefense;
                case StatMagicDefense: return up ? StatusEffectVfxType.UpStatusMagicDefence : StatusEffectVfxType.DownStatusMagicDefense;

                case StatAttackRecast: return up ? StatusEffectVfxType.DownStatusAttackRecast : StatusEffectVfxType.UpStatusAttackRecast;
                case StatMoveRecast: return up ? StatusEffectVfxType.UpStatusMoveSpeed : StatusEffectVfxType.DownStatusMoveSpeed;
                case StatAttackRange: return up ? StatusEffectVfxType.UpStatusRange : StatusEffectVfxType.DownStatusRange;
                case StatTpRate: return up ? StatusEffectVfxType.UpStatusTechnicalPoint : StatusEffectVfxType.DownStatusTechnicalPoint;
                default: return StatusEffectVfxType.None;
            }
        }

        private static void MountPeriodic(SimUnit caster, SimUnit target, int actionId, BattleSkillActionInfo action, float t, bool isHeal, bool maxLifePercent, bool aura)
        {

            bool mitsukiLayout = action.SkillActionType == 10052;
            float duration = mitsukiLayout ? action.Value(4, caster.Rank) : action.Value(3, caster.Rank);
            if (duration <= 0f) return;
            float interval = Math.Max(0.01f, mitsukiLayout ? action.Value(3, caster.Rank) : action.Value(2, caster.Rank));
            float radius = mitsukiLayout ? action.Value(2, caster.Rank) : action.Value(4, caster.Rank);
            var existing = target.Periodics.FirstOrDefault(e =>
                e.ActionId == actionId && e.SourceKey == caster.UniqueKey);
            if (existing != null)
            {

                existing.NextTickAt = Math.Min(existing.NextTickAt, t + interval);
                existing.ExpiresAt = t + duration;
                return;
            }
            target.Periodics.Add(new SimUnit.PeriodicEffect
            {
                ActionId = actionId,
                SourceKey = caster.UniqueKey,
                IsHeal = isHeal,

                IsMagical = IsMagicDamageAction(action.SkillActionType),
                NoDefence = IsSpecialDamageAction(action.SkillActionType),
                MaxLifePercent = maxLifePercent,
                Aura = aura,
                Magnitude = Math.Max(0f, action.Value(1, caster.Rank)),
                Interval = interval,
                AuraRadius = aura ? Math.Max(0f, radius) : 0f,
                NextTickAt = t + interval,
                ExpiresAt = t + duration,
            });
        }

        private static void TickStatAura(SimUnit holder, SimUnit.PeriodicEffect e, float at,
            List<SimUnit> units, SimResult result, int matchId)
        {
            var covered = units
                .Where(x => x.Alive && x.IsPrimary == holder.IsPrimary && HexDistance(holder, x) <= e.AuraRadius)
                .OrderBy(x => x.UniqueKey)
                .ToList();
            foreach (var v in covered)
            {
                var m = v.StatMods.FirstOrDefault(x =>
                    x.AuraHolderKey == holder.UniqueKey && x.AuraActionId == e.ActionId);
                if (m != null)
                {
                    if (m.ExpiresAt < e.ExpiresAt) m.ExpiresAt = e.ExpiresAt;
                    continue;
                }
                v.StatMods.Add(new SimUnit.StatMod
                {
                    Stat = e.AuraStat,
                    Percent = e.AuraPercent,
                    Magnitude = e.AuraMagnitude,
                    ExpiresAt = e.ExpiresAt,
                    Vfx = e.AuraVfx,
                    AuraHolderKey = holder.UniqueKey,
                    AuraActionId = e.ActionId,
                });
                result.Events.Add((at, new BattleBoardStatusEffectAddedEventResponse
                {
                    MatchId = matchId, RaisedAt = at,
                    UniqueKey = holder.UniqueKey, TargetUniqueKey = v.UniqueKey,
                    SkillActionId = e.ActionId, StatusEffectVfxType = e.AuraVfx, SynergyId = 0,
                }));
                EmitUnitStatusSync(result, matchId, at, v);
            }
            foreach (var v in units) DropAuraMods(v, holder, e, at, result, matchId, covered);
        }

        private static void WithdrawStatAura(SimUnit holder, SimUnit.PeriodicEffect e, float at,
            List<SimUnit> units, SimResult result, int matchId)
        {
            foreach (var v in units) DropAuraMods(v, holder, e, at, result, matchId, null);
        }

        private static void DropAuraMods(SimUnit v, SimUnit holder, SimUnit.PeriodicEffect e, float at,
            SimResult result, int matchId, List<SimUnit> stillCovered)
        {
            if (stillCovered != null && stillCovered.Contains(v)) return;
            var stale = v.StatMods
                .Where(m => m.AuraHolderKey == holder.UniqueKey && m.AuraActionId == e.ActionId)
                .ToList();
            if (stale.Count == 0) return;
            v.StatMods.RemoveAll(m => m.AuraHolderKey == holder.UniqueKey && m.AuraActionId == e.ActionId);
            foreach (var m in stale)
                result.Events.Add((at, new BattleBoardStatusEffectRemovedEventResponse
                {
                    MatchId = matchId, RaisedAt = at, UniqueKey = v.UniqueKey, SkillActionId = 0,
                    StatusEffectVfxType = m.Vfx, SynergyId = 0,
                }));
            EmitUnitStatusSync(result, matchId, at, v);
        }

        private static void ApplyBarrierAction(SimUnit a, float at, float t, BattleSkillActionInfo action, List<SimUnit> targets, SimResult result, int matchId)
        {
            int atype = action.SkillActionType;
            if (IsBarrierAction(atype))
            {
                int capacity = Math.Max(0, (int)Math.Floor(action.Value(1, a.Rank)));

                float duration = atype == 10033 ? action.Value(2, a.Rank)
                    : action.Value(2, a.Rank) * CountToSeconds;
                foreach (var target in targets)
                {
                    target.Shield = capacity;
                    target.ShieldCapacity = capacity;
                    target.ShieldExpiresAt = duration > 0f ? t + duration : 0f;
                    if (atype == 10002 && duration > 0f)
                    {
                        target.BarrierReflectUntil = t + duration;
                        target.BarrierReflectRadius = Math.Max(0f, action.Value(4, a.Rank));
                        target.BarrierReflectActionId = action.Id;
                    }
                    result.Events.Add((at, new BattleBoardAttachBarrierEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        UniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                        SkillActionId = action.Id, IsBarrierEnd = false,
                    }));

                    result.Events.Add((at, new BattleBoardBarrierEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        TargetUniqueKey = target.UniqueKey,
                        MaxBufferedDamage = capacity, BufferedDamage = 0,
                    }));
                }
                return;
            }

            float dur = action.Value(1, a.Rank);
            float until = dur > 0f ? t + dur : t + MaxSeconds;
            var vfx = atype == 10044 ? StatusEffectVfxType.Barrier
                : atype == 10017 ? StatusEffectVfxType.Barrier
                : StatusEffectVfxType.Immortal;
            foreach (var target in targets)
            {
                if (atype == 10021)
                {

                    target.LethalGuardUntil = until;
                }
                else if (atype == 10017) { target.MagicInvincibleUntil = until; target.MagicInvincibleVfx = vfx; }
                else
                {

                    target.ReflectInvincibleUntil = until;
                    target.ReflectInvincibleVfx = vfx;
                    target.ReflectWindowHits = 0;
                    target.ReflectTpRefundPercent = Math.Max(0f, action.Value(3, a.Rank));
                    target.ReflectActionId = action.Id;
                }
                result.Events.Add((at, new BattleBoardStatusEffectAddedEventResponse
                {
                    MatchId = matchId, RaisedAt = at,
                    UniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                    SkillActionId = action.Id, StatusEffectVfxType = vfx, SynergyId = 0,
                }));
            }
        }

        private static int ApplyHealCut(SimUnit target, int amount, float t)
        {
            if (amount <= 0) return 0;
            float cut = t < target.HealCutUntil ? target.HealCutPercent : 0f;
            if (cut <= 0f) return amount;
            if (cut >= 100f) return 0;
            return (int)Math.Floor(amount * (1.0 - cut / 100.0));
        }

        private static void EmitSelfHeal(SimResult result, int matchId, float at, SimUnit self, int amount, float t)
        {
            if (amount <= 0 || !self.Alive) return;
            int prev = self.Life;
            int healed = Math.Min(self.MaxLife, prev + ApplyHealCut(self, amount, t)) - prev;
            if (healed <= 0) return;
            self.Life = prev + healed;
            result.Events.Add((at, new BattleBoardHpHealEventResponse
            {
                MatchId = matchId, RaisedAt = at,
                UniqueKey = self.UniqueKey, TargetUniqueKey = self.UniqueKey,
                Life = self.Life, Heal = healed, PreviousLife = prev,
                ForceHeal = false, EmitOnDead = false,
            }));
        }

        private static int ApplyDamageToTarget(SimUnit attacker, SimUnit target, int dmg, float t, bool isMagical, SimResult result, int matchId, float at, List<SimUnit> units, bool allowCounter = true, int rawDamage = 0)
        {
            if (dmg <= 0) return 0;

            if (!ReferenceEquals(attacker, target) && attacker.IsPrimary != target.IsPrimary)
            {
                target.LastDamagedAt = t;
                target.LastAttackerKey = attacker.UniqueKey;

                if (target.RetaliateTarget == null || !target.RetaliateTarget.Alive
                    || target.RetaliateTarget.IsPrimary == target.IsPrimary)
                    target.RetaliateTarget = attacker;
            }

            int nominal = rawDamage > 0 ? rawDamage : dmg;
            bool immune = t < target.InvincibleUntil || (isMagical && t < target.MagicInvincibleUntil)
                || t < target.ReflectInvincibleUntil;

            if (!immune && !result.Reflecting && units != null && target.BarrierReflectUntil > t
                && target.Shield > 0 && (target.ShieldExpiresAt <= 0f || t < target.ShieldExpiresAt)
                && dmg > 0)
            {
                result.PendingReflects.Add((target.UniqueKey, dmg, nominal, target.BarrierReflectRadius,
                    target.BarrierReflectActionId, target.IsPrimary));
            }
            if (!immune && target.Shield > 0 && (target.ShieldExpiresAt <= 0f || t < target.ShieldExpiresAt))
            {
                int absorbed = Math.Min(target.Shield, dmg);
                target.Shield -= absorbed;
                dmg -= absorbed;
                result.Events.Add((at, new BattleBoardBarrierDamageEventResponse
                {
                    MatchId = matchId, RaisedAt = at,
                    OperatorUniqueKey = attacker.UniqueKey, TargetUniqueKey = target.UniqueKey,

                    MaxBufferedDamage = target.ShieldCapacity,
                    BufferedDamage = target.ShieldCapacity - target.Shield,

                    Damage = absorbed,
                    DamageType = isMagical ? DamageType.Magical : DamageType.Physical,
                }));
            }
            int toHp = immune ? 0 : dmg;

            if (toHp > 0 && t < target.DamageCutUntil && target.DamageCutPercent > 0f)
                toHp = (int)Math.Floor(toHp * Math.Max(0.0, 1.0 - target.DamageCutPercent / 100.0));

            if (toHp > target.Life && t < target.LethalGuardUntil) toHp = 0;

            if (!ReferenceEquals(attacker, target) && toHp > 0) QueueDamageTp(result, target, nominal);

            if (immune && !result.Reflecting && units != null && t < target.ReflectInvincibleUntil
                && attacker.Alive && !ReferenceEquals(attacker, target)
                && attacker.IsPrimary != target.IsPrimary && dmg > 0)
            {
                var atkInfo = BattleMaster.GetUnit(attacker.UnitId);
                int reflected = AttackDamage(Math.Max(0, atkInfo?.Atk ?? 0), attacker.Defense);
                if (reflected > 0)
                {
                    target.ReflectWindowHits++;
                    int mPrev = attacker.Life;
                    int mDealt;
                    result.Reflecting = true;
                    try
                    {
                        int mToHp = ApplyDamageToTarget(target, attacker, reflected, t, false, result, matchId, at,
                            units, allowCounter: false, rawDamage: reflected);
                        mDealt = ApplyGearReviveGuard(attacker, mPrev, mToHp, result, matchId, at);
                    }
                    finally
                    {
                        result.Reflecting = false;
                    }

                    result.Events.Add((at, new BattleBoardHpDamageEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        OperatorUniqueKey = target.UniqueKey, TargetUniqueKey = attacker.UniqueKey,
                        Life = attacker.Life, Damage = mDealt, IsDead = !attacker.Alive,
                        AttackType = AttackType.None, PreviousLife = mPrev,
                    }));
                }
            }

            if (allowCounter && toHp > 0 && attacker.Alive
                && attacker.IsPrimary != target.IsPrimary
                && target.CounterStanceUntil > t && target.CounterDamageFlat > 0f
                && !ReferenceEquals(attacker, target))
            {
                int cPower = (int)Math.Floor(target.CounterDamageFlat * DamageScale);
                if (cPower > 0)
                {
                    if (HexDistance(target, attacker) > 1)
                    {
                        foreach (var (mx, my) in HexRingCells(attacker.X, attacker.Y, 1))
                        {
                            if (units.Any(u => u.X == mx && u.Y == my)) continue;
                            result.Events.Add((at, new BattleBoardSkillMoveEventResponse
                            {
                                MatchId = matchId, RaisedAt = at,
                                UniqueKey = target.UniqueKey, TargetUniqueKey = attacker.UniqueKey,
                                X = mx, Y = my, IsJump = false,
                                PreviousX = target.X, PreviousY = target.Y,
                            }));
                            target.X = mx; target.Y = my;
                            break;
                        }
                    }
                    int cPrev = attacker.Life;
                    int cDmg = ApplyDamageToTarget(target, attacker, cPower,
                        t, false, result, matchId, at, units, allowCounter: false);
                    cDmg = ApplyGearReviveGuard(attacker, cPrev, cDmg, result, matchId, at);
                    result.Events.Add((at, new BattleBoardCounterSkillDamageEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        OperatorUniqueKey = target.UniqueKey, TargetUniqueKey = attacker.UniqueKey,
                        Life = attacker.Life, Damage = cDmg, CounterBeforeMove = false,
                        SkillId = target.CounterSkillActionId, IsDead = !attacker.Alive,
                        DamageType = DamageType.Physical, PreviousLife = cPrev, IsCounterDamage = true,
                    }));

                    target.TargetUniqueKey = 0;
                    SelectTarget(target, units, t);
                    bool survivesThisHit = target.Life > toHp;
                    if (!attacker.Alive && survivesThisHit)
                    {
                        target.CounterStanceUntil = target.CounterStanceSeconds > 0f
                            ? t + target.CounterStanceSeconds : t + MaxSeconds;
                        result.Events.Add((at, new BattleBoardCounterSkillOperateEventResponse
                        {
                            MatchId = matchId, RaisedAt = at,
                            UniqueKey = target.UniqueKey, SkillActionId = target.CounterSkillActionId,
                            IsEnd = false, IsCancel = false,
                        }));
                        result.Events.Add((at, new BattleBoardSkillOperateEventResponse
                        {
                            MatchId = matchId, RaisedAt = at, UniqueKey = target.UniqueKey,
                            SkillId = target.UnitId,
                            SkillActionResults = new[]
                            {
                                new BattleBoardSkillOperateEventResponse.SkillActionResultResponse
                                {
                                    SkillActionId = target.CounterSkillActionId,
                                    Targets = new Dictionary<int, int> { { target.UniqueKey, target.Life } },
                                },
                            },
                            PreviousTargetStatus = new Dictionary<int, BoardUnitStatusMpo>(),
                        }));
                    }
                    else
                    {
                        target.CounterStanceUntil = 0f;
                        result.Events.Add((at, new BattleBoardCounterSkillOperateEventResponse
                        {
                            MatchId = matchId, RaisedAt = at,
                            UniqueKey = target.UniqueKey, SkillActionId = target.CounterSkillActionId,
                            IsEnd = true, IsCancel = false,
                        }));
                    }
                }
            }

            return toHp;
        }

        private static int ApplyGearReviveGuard(SimUnit victim, int prevLife, int dmg,
            SimResult result, int matchId, float at)
        {
            int newLife = Math.Max(0, prevLife - dmg);
            if (prevLife > 0 && newLife <= 0
                && victim.GearReviveHpPercent > 0 && victim.GearReviveFires < victim.GearReviveMaxFires)
            {
                victim.GearReviveFires++;
                int minHp = Math.Max(1, (int)Math.Floor(victim.MaxLife * victim.GearReviveHpPercent / 100.0));
                newLife = Math.Min(victim.MaxLife, Math.Max(prevLife, minHp));
                if (newLife > prevLife)
                {
                    result.Events.Add((at, new BattleBoardHpHealEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        UniqueKey = victim.UniqueKey, TargetUniqueKey = victim.UniqueKey,
                        Life = newLife, Heal = newLife - prevLife, PreviousLife = prevLife,
                        ForceHeal = false, EmitOnDead = false,
                    }));
                }
                Console.WriteLine($"[SIM] gear-revive unit={victim.UnitId} key={victim.UniqueKey} " +
                                  $"at={at:0.00} prev={prevLife} life={newLife} fires={victim.GearReviveFires}/{victim.GearReviveMaxFires}");
                victim.Life = newLife;

                CancelLastDamageTp(result, victim.UniqueKey);
                return 0;
            }
            victim.Life = newLife;
            return dmg;
        }

        private static void ApplyMoveAction(SimUnit a, float at, BattleSkillActionInfo action, List<SimUnit> targets, List<SimUnit> units, SimResult result, int matchId)
        {
            if (targets.Count == 0) return;
            var target = targets[0];
            if (ReferenceEquals(target, a)) return;
            bool isJump = action.SkillActionType == 10023;
            for (int ring = 1; ring <= 2; ring++)
            {
                foreach (var (x, y) in HexRingCells(target.X, target.Y, ring))
                {
                    if (x < 0 || x > BoardMaxX || y < 0 || y > BoardMaxY) continue;
                    if (units.Any(u => u.X == x && u.Y == y)) continue;
                    int px = a.X, py = a.Y;
                    a.X = x; a.Y = y;
                    result.Events.Add((at, new BattleBoardMoveFieldEventResponse
                    {
                        MatchId = matchId, RaisedAt = at,
                        UniqueKey = a.UniqueKey, X = x, Y = y, IsJump = isJump,
                        PreviousX = px, PreviousY = py,
                    }));
                    return;
                }
            }
        }

        private static SimUnit UbReachAim(SimUnit a, List<SimUnit> units)
        {
            foreach (var actionId in a.SkillActionIds)
            {
                if (actionId == 0) continue;
                var act = BattleMaster.SkillAction(actionId);
                if (act == null) continue;
                if (act.TargetType == 4 || act.TargetType == 6 || IsSummonAction(act.SkillActionType)) continue;
                bool ownSide = act.AssignmentType == 2;
                return units
                    .Where(u => u.Alive && (ownSide || !ReferenceEquals(u, a))
                        && u.IsPrimary == (ownSide ? a.IsPrimary : !a.IsPrimary))
                    .OrderBy(u => HexDistance(a, u)).ThenBy(u => u.UniqueKey)
                    .FirstOrDefault();
            }
            return null;
        }

        private static List<(int actionId, BattleSkillActionInfo action, List<SimUnit> targets)> BuildUbSchedule(
            SimUnit a, List<SimUnit> units, float reach, int matchId, float t)
        {
            var scheduled = new List<(int actionId, BattleSkillActionInfo action, List<SimUnit> targets)>();
            var previousTargets = new List<SimUnit>();
            foreach (var actionId in a.SkillActionIds)
            {
                if (actionId == 0) continue;
                var act = BattleMaster.SkillAction(actionId);
                if (act == null) continue;
                var actTargets = ResolveActionTargets(a, units, act, previousTargets, reach, matchId, t);
                scheduled.Add((actionId, act, actTargets));
                previousTargets = actTargets;
            }
            return scheduled;
        }

        private static bool UbTakesThisBeat(SimUnit a, List<SimUnit> units, int matchId, float t)
        {
            if (a.MaxTp <= 0 || a.SkillActionIds.Length == 0) return false;
            if (a.CurrentTp < a.MaxTp) return false;

            float reach = BattleMaster.UnitSkillRange(a.UnitId, a.Rank);
            if (reach < 90f)
            {
                var aim = UbReachAim(a, units);
                if (aim != null && HexDistance(a, aim) > reach) return false;
            }
            return BuildUbSchedule(a, units, reach, matchId, t).Any(s => s.targets.Count > 0);
        }

        private static float UbChargeSeconds(SimUnit a, List<(int actionId, BattleSkillActionInfo action, List<SimUnit> targets)> schedule)
        {
            foreach (var (_, action, _) in schedule)
            {
                int at = action.SkillActionType;
                if (at == 10016 || at == 10029 || at == 10046)
                {
                    float charge = action.Value(1, a.Rank);
                    if (charge > 0f) return charge;
                }
            }
            return 0f;
        }

        private static List<SimUnit> UbSearchScope(SimUnit self, List<SimUnit> units, float reach)
            => reach >= 90f ? units
                : units.Where(u => u.Alive && HexDistance(self, u) <= reach).ToList();

        private static List<SimUnit> ResolveActionTargets(SimUnit self, List<SimUnit> units, BattleSkillActionInfo action, List<SimUnit> previousTargets, float reach, int matchId, float t)
        {
            int atype = action.SkillActionType;

            var scope = action.TargetType == 4 ? units : UbSearchScope(self, units, reach);

            if (action.TargetType == 4)
            {
                var pool = previousTargets.Where(u => u.Alive).ToList();
                if (pool.Count == 0) return pool;
                if (IsDamageAction(atype))
                    return ExpandGeometry(self, pool, action, units);
                return pool;
            }

            if (atype == 99992)
                return SelectSideTargets(self, scope, action, sameSide: true, matchId, t);
            if (atype == 10052)
                return new List<SimUnit> { self };
            if (IsHealAction(atype)) return SelectSideTargets(self, scope, action, sameSide: true, matchId, t);

            if (IsAttackSplashAction(atype) || IsCounterStanceAction(atype))
                return new List<SimUnit> { self };

            if (IsStunAction(atype))
            {
                var stunTargets = SelectSideTargets(self, scope, action, sameSide: false, matchId, t);
                return ExpandGeometry(self, stunTargets, action, scope);
            }

            if (IsStatAction(atype) || atype == 10045 || atype == 10036)
                return SelectSideTargets(self, scope, action, sameSide: action.AssignmentType == 2, matchId, t);

            if (IsBarrierAction(atype) || IsInvincibleAction(atype))
                return SelectSideTargets(self, scope, action, sameSide: true, matchId, t);

            if (IsCharmAction(atype) || IsTauntAction(atype))
                return SelectSideTargets(self, scope, action, sameSide: false, matchId, t);

            if (IsMoveAction(atype))
                return SelectSideTargets(self, scope, action, sameSide: false, matchId, t);

            if (IsTpGainAction(atype))
                return SelectSideTargets(self, scope, action, sameSide: action.AssignmentType == 2, matchId, t);

            if (IsSummonAction(atype))
                return new List<SimUnit> { self };
            return SelectUbTargets(self, scope, action, IsDamageAction(atype), matchId, t);
        }

        private static List<SimUnit> SelectSideTargets(SimUnit self, List<SimUnit> units, BattleSkillActionInfo action, bool sameSide, int matchId, float t)
        {
            var side = units
                .Where(u => u.Alive && u.IsPrimary == self.IsPrimary)
                .OrderBy(u => u.UniqueKey)
                .ToList();
            if (!sameSide)
                side = units
                    .Where(u => u.Alive && u.IsPrimary != self.IsPrimary)
                    .OrderBy(u => HexDistance(self, u))
                    .ThenBy(u => u.Life)
                    .ThenBy(u => u.UniqueKey)
                    .ToList();
            if (side.Count == 0) return new List<SimUnit>();
            if (action.TargetType == 6)
            {
                var selfHit = side.FirstOrDefault(u => u.UniqueKey == self.UniqueKey);
                return selfHit != null ? new List<SimUnit> { selfHit } : new List<SimUnit>();
            }

            side = ApplyTargetExtras(side, action);
            if (side.Count == 0) return new List<SimUnit>();

            if (action.TargetType == 3)
                ShuffleDeterministic(side, matchId, self.UniqueKey, action.Id, t);
            IEnumerable<SimUnit> ordered = action.TargetType switch
            {
                1 => side.OrderBy(u => HexDistance(self, u)).ThenBy(u => u.UniqueKey),
                2 => side.OrderByDescending(u => HexDistance(self, u)).ThenBy(u => u.UniqueKey),
                _ => side,
            };
            int count = action.TargetCount(self.Rank);
            var list = ordered.ToList();
            if (count >= 99) return list;
            return list.Take(Math.Min(Math.Max(1, count), list.Count)).ToList();
        }

        private const int AssasinRoleType = 106;

        private static bool IsAssassin(SimUnit u) =>
            Array.IndexOf(u.RoleTypes, AssasinRoleType) >= 0
            || Array.IndexOf(u.SynergyRoleTypes, AssasinRoleType) >= 0;

        private static void ApplyAssassinJumps(List<SimUnit> units, int matchId, SimResult result)
        {
            var occupied = new HashSet<(int, int)>(units.Select(u => (u.X, u.Y)));
            var pending = new List<(SimUnit a, SimUnit target, int tx, int ty, int x, int y)>();
            foreach (var a in units)
            {
                if (!a.Alive) continue;
                if (!IsAssassin(a)) continue;
                var enemies = units.Where(u => u.Alive && u.IsPrimary != a.IsPrimary).ToList();
                if (enemies.Count == 0) continue;

                var candidates = enemies.Where(u => u.UntargetableUntil <= 0f).ToList();
                if (candidates.Count == 0) candidates = enemies;
                var nonAssassin = candidates.Where(u => !IsAssassin(u)).ToList();
                if (nonAssassin.Count > 0) candidates = nonAssassin;
                var target = candidates
                    .OrderByDescending(u => HexDistance(a, u))
                    .ThenByDescending(u => u.UniqueKey)
                    .FirstOrDefault();
                if (target == null) continue;
                int lx = -1, ly = -1;
                for (int ring = 1; ring <= 3 && lx < 0; ring++)
                {
                    foreach (var (x, y) in HexRingCells(target.X, target.Y, ring))
                    {
                        if (!occupied.Add((x, y))) continue;
                        lx = x; ly = y; break;
                    }
                }
                if (lx < 0) continue;
                pending.Add((a, target, target.X, target.Y, lx, ly));
            }
            foreach (var (a, target, tx, ty, lx, ly) in pending)
            {
                result.Events.Add((AtStartSeconds, new BattleBoardSkillMoveEventResponse
                {
                    MatchId = matchId, RaisedAt = AtStartSeconds,
                    UniqueKey = a.UniqueKey, TargetUniqueKey = target.UniqueKey,
                    X = lx, Y = ly, IsJump = true,
                    PreviousX = a.X, PreviousY = a.Y,
                }));
                if (VerboseSimLog)
                    Console.WriteLine($"[SIM] assassin jump key={a.UniqueKey} unit={a.UnitId} ({a.X},{a.Y})->({lx},{ly}) target={target.UniqueKey}@({tx},{ty})");
                a.X = lx; a.Y = ly;
            }

            foreach (var (a, _, _, _, _, _) in pending)
            {
                a.TargetUniqueKey = 0;
                SelectTarget(a, units, 0f);
            }
        }

        private static List<(int x, int y)> HexRingCells(int cx, int cy, int ring)
        {
            var seen = new HashSet<(int, int)> { (cx, cy) };
            var frontier = new List<(int, int)> { (cx, cy) };
            for (int step = 0; step < ring; step++)
            {
                var next = new List<(int, int)>();
                foreach (var (x, y) in frontier)
                {
                    var offsets = (y & 1) == 0 ? HexEvenOffsets : HexOddOffsets;
                    foreach (var o in offsets)
                    {
                        var n = (x + o.dx, y + o.dy);
                        if (n.Item1 < 0 || n.Item1 > BoardMaxX || n.Item2 < 0 || n.Item2 > BoardMaxY) continue;
                        if (!seen.Add(n)) continue;
                        next.Add(n);
                    }
                }
                frontier = next;
            }
            return frontier;
        }

        private static List<SimUnit> ApplyTargetExtras(List<SimUnit> candidates, BattleSkillActionInfo action)
        {
            if (candidates.Count == 0 || string.IsNullOrEmpty(action.TargetExtras)) return candidates;
            foreach (var part in action.TargetExtras.Split('&'))
            {
                var eq = part.IndexOf('=');
                if (eq <= 0) continue;
                var key = part.Substring(0, eq);
                var val = part.Substring(eq + 1);
                switch (key)
                {
                    case "role_type":
                        if (int.TryParse(val, out var role))
                            candidates = candidates.Where(u => Array.IndexOf(u.RoleTypes, role) >= 0).ToList();
                        break;
                    case "team_type":
                        if (int.TryParse(val, out var team))
                            candidates = candidates.Where(u => Array.IndexOf(u.TeamTypes, team) >= 0).ToList();
                        break;
                    case "parameter_highest":
                    case "parameter_higher":
                    case "parameter_lowest":
                    case "parameter_lower":
                        if (!int.TryParse(val, out var param)) break;
                        Func<SimUnit, int> field = param switch
                        {
                            1 => u => u.Attack,
                            3 => u => u.Life,
                            _ => null,
                        };
                        if (field == null) break;
                        bool descending = key.StartsWith("parameter_high");
                        bool single = key.EndsWith("highest") || key.EndsWith("lowest");
                        candidates = descending
                            ? candidates.OrderByDescending(field).ThenBy(u => u.UniqueKey).ToList()
                            : candidates.OrderBy(field).ThenBy(u => u.UniqueKey).ToList();
                        if (single && candidates.Count > 1)
                            candidates = new List<SimUnit> { candidates[0] };
                        break;

                }
            }
            return candidates;
        }

        private static List<SimUnit> ExpandGeometry(SimUnit caster, List<SimUnit> selected, BattleSkillActionInfo action, List<SimUnit> units)
        {
            if (selected.Count == 0 || string.IsNullOrEmpty(action.TargetExtras)) return selected;
            bool hasGeometry = false, hasCap = false;
            float width = 0f, maximum = 99f, penetrateValue = 0f;
            foreach (var part in action.TargetExtras.Split('&'))
            {
                var eq = part.IndexOf('=');
                if (eq <= 0) continue;
                var key = part.Substring(0, eq);
                var val = part.Substring(eq + 1);
                if (!float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var num)) continue;
                if (key == "width") { width = num; hasGeometry = true; }

                else if (key == "maximum_range") { maximum = num * num; hasCap = true; hasGeometry = true; }

                else if (key == "penetrate") { hasGeometry = true; penetrateValue = num; }
            }

            if (width <= 0f && penetrateValue > 0f) width = penetrateValue;
            if (!hasGeometry) return selected;
            var primary = selected[0];
            static (int x, int y, int z) ToCube(int px, int py)
            {
                var cx = px - ((py & ~1) >> 1);
                return (cx, -cx - py, py);
            }
            var o = ToCube(caster.X, caster.Y);
            var e = ToCube(primary.X, primary.Y);
            int vx = e.x - o.x, vz = e.z - o.z;
            if (vx == 0 && vz == 0) return selected;
            int scale = Math.Max(1, Math.Max(Math.Abs(vx), Math.Abs(vz)));
            var corridor = new List<SimUnit>();
            var withinBand = new List<SimUnit>();
            foreach (var u in units)
            {
                if (!u.Alive || u.IsPrimary == caster.IsPrimary) continue;
                var c = ToCube(u.X, u.Y);
                int ux = c.x - o.x, uz = c.z - o.z;
                int dot = vx * ux + vz * uz;
                double cross = Math.Abs(vx * uz - vz * ux) / (double)scale;
                if (dot < 0 || cross > width + 1e-9) continue;
                corridor.Add(u);
                if (!hasCap || HexDistanceTo(caster.X, caster.Y, u.X, u.Y) < maximum) withinBand.Add(u);
            }

            if (withinBand.Count > 0) return withinBand;
            return corridor.Count > 0 ? corridor : selected;
        }

        private static void ShuffleDeterministic(List<SimUnit> list, int matchId, int casterKey, int actionId, float t)
        {
            if (list.Count < 2) return;
            ulong s = 0x9E3779B97F4A7C15UL;
            s ^= (ulong)(uint)matchId * 0xC2B2AE3D27D4EB4FUL;
            s ^= (ulong)(uint)casterKey * 0x165667B19E3779F9UL;
            s ^= (ulong)(uint)actionId * 0x27D4EB2F165667C5UL;
            s ^= (ulong)(uint)(int)Math.Round(t * 10f) * 0x9E3779B185EBCA87UL;
            for (int i = list.Count - 1; i > 0; i--)
            {
                s ^= s >> 12; s ^= s << 25; s ^= s >> 27;
                int j = (int)(s % (ulong)(i + 1));
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private static int SkillDamage(int atk, int def, BattleSkillActionInfo action, int rank, float skillMult = 1f, float takenMult = 1f, bool executeActive = false, float falloffMult = 1f)
        {
            double power = SkillPower(atk, action, rank, executeActive, falloffMult);

            double dmg = IsSpecialDamageAction(action.SkillActionType)
                ? power
                : power * 100.0 / (100.0 + Math.Max(0, def));

            return (int)(dmg * DamageScale * Math.Max(0f, skillMult) * Math.Max(0f, takenMult));
        }

        private static int SkillRawDamage(int atk, BattleSkillActionInfo action, int rank,
            float skillMult, bool executeActive, float falloffMult)
            => (int)(SkillPower(atk, action, rank, executeActive, falloffMult) * DamageScale * Math.Max(0f, skillMult));

        private static double SkillPower(int atk, BattleSkillActionInfo action, int rank, bool executeActive, float falloffMult)
        {
            float v = action.Value(DamageValueRow(action.SkillActionType), rank);
            double power = IsAttackPercentDamageAction(action.SkillActionType)
                ? atk * (double)v / 100.0
                : (double)v;
            if (action.SkillActionType == 10048 && executeActive) power += action.Value(3, rank);

            power *= Math.Max(0f, falloffMult);
            return power;
        }

        private static List<SimUnit> SelectUbTargets(SimUnit self, List<SimUnit> units, BattleSkillActionInfo action, bool damageTyped, int matchId, float t)
        {
            var enemies = units
                .Where(u => u.Alive && u.IsPrimary != self.IsPrimary)
                .OrderBy(u => HexDistance(self, u))
                .ThenBy(u => u.Life)
                .ThenBy(u => u.UniqueKey)
                .ToList();
            if (enemies.Count == 0) return new List<SimUnit>();
            if (action == null) return new List<SimUnit> { enemies[0] };
            if (action.TargetType == 2)
                enemies = enemies
                    .OrderByDescending(u => HexDistance(self, u))
                    .ThenByDescending(u => u.UniqueKey)
                    .ToList();
            enemies = ApplyTargetExtras(enemies, action);
            if (enemies.Count == 0) return new List<SimUnit>();

            if (action.TargetType == 3)
                ShuffleDeterministic(enemies, matchId, self.UniqueKey, action.Id, t);
            if (!damageTyped) return new List<SimUnit> { enemies[0] };
            int count = action.TargetCount(self.Rank);

            if (action.SkillActionType == 10030) count = 1;
            List<SimUnit> selected;
            if (count >= 99) selected = enemies;
            else selected = enemies.Take(Math.Min(Math.Max(1, count), enemies.Count)).ToList();
            var expanded = ExpandGeometry(self, selected, action, units);

            if (action.SkillActionType == 10030)
                expanded = ExpandRadiusAroundTargets(self, expanded, units, Math.Max(0f, action.Value(2, self.Rank)));

            if (action.SkillActionType == 10015)
            {
                int hits = Math.Max(1, (int)action.Value(2, self.Rank));
                if (hits < expanded.Count) expanded = expanded.Take(hits).ToList();
            }
            return expanded;
        }

        private static List<SimUnit> ExpandRadiusAroundTargets(SimUnit self, List<SimUnit> selected, List<SimUnit> units, float radius)
        {
            if (selected.Count == 0 || radius <= 0f) return selected;
            var seen = new HashSet<int>(selected.Select(u => u.UniqueKey));
            var result = new List<SimUnit>(selected);
            foreach (var pivot in selected)
            {
                foreach (var u in units)
                {
                    if (!u.Alive || u.IsPrimary == self.IsPrimary) continue;
                    if (HexDistanceTo(pivot.X, pivot.Y, u.X, u.Y) > radius) continue;
                    if (seen.Add(u.UniqueKey)) result.Add(u);
                }
            }
            return result;
        }

        private static readonly (int dx, int dy)[] HexEvenOffsets = { (1, 0), (-1, 0), (0, -1), (-1, -1), (0, 1), (-1, 1) };
        private static readonly (int dx, int dy)[] HexOddOffsets = { (1, 0), (-1, 0), (1, -1), (0, -1), (1, 1), (0, 1) };

        private static int HexDistance(SimUnit a, SimUnit b) => HexDistanceTo(a.X, a.Y, b.X, b.Y);

        private static List<SimUnit> RefreshEnemySlowAuras(List<SimUnit> units)
        {
            foreach (var v in units) v.SufferedAuraSlowMult = 1f;
            foreach (var w in units)
            {
                if (!w.Alive || w.AuraEnemySlowMult <= 1f) continue;
                float radius = w.AuraEnemySlowRadius > 0f ? w.AuraEnemySlowRadius : float.MaxValue;
                foreach (var v in units)
                {
                    if (!v.Alive || ReferenceEquals(v, w) || v.IsPrimary == w.IsPrimary) continue;
                    if (HexDistance(w, v) > radius) continue;
                    v.SufferedAuraSlowMult = Math.Max(v.SufferedAuraSlowMult, w.AuraEnemySlowMult);
                }
            }

            List<SimUnit> changed = null;
            foreach (var v in units)
            {
                if (!v.Alive) continue;
                if (Math.Abs(v.SufferedAuraSlowMult - v.ReportedAuraSlowMult) < 1e-4f) continue;
                v.ReportedAuraSlowMult = v.SufferedAuraSlowMult;
                (changed ??= new List<SimUnit>()).Add(v);
            }
            return changed ?? new List<SimUnit>();
        }

        private static int HexDistanceTo(int x1, int y1, int x2, int y2)
        {
            int cx1 = x1 - ((y1 & ~1) >> 1);
            int cx2 = x2 - ((y2 & ~1) >> 1);
            int dcx = cx1 - cx2, dy = y1 - y2;
            return (Math.Abs(dcx) + Math.Abs(dy) + Math.Abs(dcx + dy)) >> 1;
        }

        private static int AttackDamage(int atk, int def)
        {

            double raw = atk * 100.0 / (100.0 + Math.Max(0, def));
            return (int)(raw * DamageScale);
        }

        private static int AttackRawDamage(int atk) => (int)(atk * (double)DamageScale);

        private static float ReadScale(string raw)
            => float.TryParse(raw, out var v) && v > 0f ? v : 1f;
    }
}
