using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Byac.ServerShared;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.Services.Battle
{
    public class BattleUnitInfo
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public bool Playable { get; set; }
        public bool IsToken { get; set; }

        public int GuildId { get; set; }

        public int[] RoleTypes { get; set; } = Array.Empty<int>();
        public int[] TeamTypes { get; set; } = Array.Empty<int>();
        public int Price { get; set; }
        public int Life { get; set; }
        public int Atk { get; set; }
        public int MagicAtk { get; set; }
        public int Defence { get; set; }
        public int MagicDefence { get; set; }
        public int TechnicalPoint { get; set; }
        public float AttackRecastTime { get; set; }
        public float MoveRecastTime { get; set; }
        public float AttackRange { get; set; }

        public int AttackType { get; set; } = 1;
    }

    public class BattleRoundInfo
    {
        public int Number { get; set; }
        public bool IsLoop { get; set; }
        public int NpcId { get; set; }
        public int DistributeGold { get; set; }
        public int WinGold { get; set; }
        public int LoseGold { get; set; }
        public int WinBonus { get; set; }

        public int BaseDamage { get; set; }
        public int TeamBaseDamage { get; set; }
        public int UnitDamage { get; set; }
        public int ExperiencePoints { get; set; }
        public int PrepareTime { get; set; }
    }

    public class BattleEquipmentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Type { get; set; }
        public int Life { get; set; }
        public int Atk { get; set; }
        public int MagicAtk { get; set; }
        public int Defence { get; set; }
        public int MagicDefence { get; set; }
        public float TechnicalPointRate { get; set; }
        public int AttackRecastTimeTickRate { get; set; }
        public int MoveRecastTimeTickRate { get; set; }
        public float AttackRange { get; set; }
        public int AttackType { get; set; }
        public int[] AfterSynthesisIds { get; set; } = Array.Empty<int>();
        public int Rank { get; set; }
        public int RoleType { get; set; }
        public int TeamType { get; set; }
        public int[] SelectableEquipmentIds { get; set; } = Array.Empty<int>();
        public string Description { get; set; } = "";
        public int SkillDamageScale { get; set; }
    }

    public class BattleRuleInfo
    {
        public int Id { get; set; }
        public int BattleRule { get; set; }
        public int InitialGold { get; set; }
        public int InitialLevel { get; set; }
        public int PlayerMaxLife { get; set; }
        public int PrepareTime { get; set; }
        public int BuyExpValue { get; set; }
        public int ExpPrice { get; set; }
        public int RefreshShopPrice { get; set; }
        public int ShopUnitLotteryPatternId { get; set; }
    }

    public class BattleGuildBuffInfo
    {
        public int GuildId { get; set; }
        public int[] GuildBuffIds { get; set; } = Array.Empty<int>();
        public int LotteryRate { get; set; }
    }

    public class BattleItemDropInfo
    {
        public int Round { get; set; }
        public int ItemId { get; set; }
        public int RewardNumber { get; set; }
        public int RelativeOdds { get; set; }
    }

    public class BattleSynergyInfo
    {
        public int SynergyType { get; set; }
        public int Id { get; set; }
        public int LowCount { get; set; }
        public int MiddleCount { get; set; }
        public int HighCount { get; set; }

        public float[] LowValues { get; set; } = Array.Empty<float>();
        public float[] MiddleValues { get; set; } = Array.Empty<float>();
        public float[] HighValues { get; set; } = Array.Empty<float>();

        public float[] ValuesForLevel(int level) => level switch
        {
            1 => LowValues,
            2 => MiddleValues,
            3 => HighValues,
            _ => Array.Empty<float>(),
        };
    }

    public sealed class ActiveSynergyEffect
    {
        public int SynergyType { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public float[] Values { get; set; } = Array.Empty<float>();
    }

    public sealed class BattleEquipmentSkillInfo
    {
        public int Id { get; set; }
        public int[] PassiveSkillActionIds { get; set; } = Array.Empty<int>();

        public float[] Range { get; set; } = Array.Empty<float>();
    }

    public sealed class BattlePassiveSkillActionInfo
    {
        public int Id { get; set; }
        public int SkillActionType { get; set; }

        public float[] Values { get; set; } = Array.Empty<float>();
        public int[] TargetCount { get; set; } = Array.Empty<int>();

        public float Value(int n) => n >= 1 && n <= Values.Length ? Values[n - 1] : 0f;
        public int TargetCountForRank1 => TargetCount.Length > 0 ? TargetCount[0] : 1;
    }

    public class BattleGuildBuffEffectInfo
    {
        public int GuildBuffId { get; set; }
        public int BuffType { get; set; }
        public int Life { get; set; }
        public int Atk { get; set; }
        public int MagicAtk { get; set; }
        public int Defence { get; set; }
        public int MagicDefence { get; set; }
        public float TechnicalPointRate { get; set; }
        public int AttackRecastTimeTickRate { get; set; }
        public int MoveRecastTimeTickRate { get; set; }
        public float AttackRange { get; set; }
        public float[] EffectValues { get; set; } = Array.Empty<float>();

        public float TpRatePercent =>
            TechnicalPointRate != 0f ? TechnicalPointRate
            : BuffType == 7 && (EffectValues?.Length ?? 0) > 0 ? EffectValues[0] : 0f;

        public bool NeedsRuntimeHandler =>
            MoveRecastTimeTickRate != 0 || AttackRange != 0f ||
            (BuffType != 7 && (EffectValues?.Length ?? 0) > 0 && Array.Exists(EffectValues, v => v != 0f));
    }

    public class BattleAiKindInfo
    {
        public int Id { get; set; }
        public int StrategySetId { get; set; }
        public int TargetPartyPower { get; set; }
        public float EmoteProb { get; set; }
        public int EmoteDelaySecMin { get; set; }
        public int EmoteDelaySecMax { get; set; }
        public int EmoteTimesMin { get; set; }
        public int EmoteTimesMax { get; set; }
        public float MissProbRefreshShop { get; set; }
        public float MissProbBuyUnit { get; set; }
        public float MissProbLevelup { get; set; }
        public float MissProbDeployUnit { get; set; }
        public int MinCommandDelay { get; set; }
        public int MaxCommandDelay { get; set; }
    }

    public class BattleSkillActionInfo
    {
        public int Id { get; set; }
        public int SkillActionType { get; set; }
        public float[][] ActionValues { get; set; } = Array.Empty<float[]>();
        public int[] TargetCounts { get; set; } = Array.Empty<int>();

        public int AssignmentType { get; set; }
        public int TargetType { get; set; }
        public string TargetExtras { get; set; } = "";

        public float Value(int n, int rank)
        {
            if (n < 1 || n > ActionValues.Length) return 0f;
            var arr = ActionValues[n - 1];
            if (arr.Length == 0) return 0f;
            int idx = Math.Max(1, rank) - 1;
            if (idx >= arr.Length) idx = arr.Length - 1;
            return arr[idx];
        }

        public int TargetCount(int rank)
        {
            if (TargetCounts.Length == 0) return 1;
            int idx = Math.Max(1, rank) - 1;
            if (idx >= TargetCounts.Length) idx = TargetCounts.Length - 1;
            return Math.Max(1, TargetCounts[idx]);
        }
    }

    public static class BattleMaster
    {
        private static readonly object LoadLock = new object();
        private static bool _loaded;
        private static Dictionary<int, BattleUnitInfo> _units = new Dictionary<int, BattleUnitInfo>();
        private static Dictionary<int, BattleRoundInfo> _rounds = new Dictionary<int, BattleRoundInfo>();

        private static List<BattleRoundInfo> _loopRounds = new List<BattleRoundInfo>();

        private static Dictionary<int, (int unitId, int x, int y)[]> _npcLayouts =
            new Dictionary<int, (int, int, int)[]>();
        private static Dictionary<int, BattleRuleInfo> _rules = new Dictionary<int, BattleRuleInfo>();

        private static (int BossId, int NpcId, int BoardId) _bossRow = (1, BossBattleStore.BossUnitId, 2);

        public static (int BossId, int NpcId, int BoardId) BossRow => _bossRow;
        private static int[] _initialPool = Array.Empty<int>();
        private static int[] _avatarIds = Array.Empty<int>();
        private static int[] _gradeIds = Array.Empty<int>();

        private static (int gradeId, int min, int max)[] _gradeRanges = Array.Empty<(int, int, int)>();

        private static Dictionary<int, (float rank2Rate, float rank3Rate)> _unitRankRates =
            new Dictionary<int, (float, float)>();

        private static int _maxAvatarAvailableIndex = -1;

        private static HashSet<int> _emoteIds = new HashSet<int>();

        private static HashSet<int> _recommendPartyIds = new HashSet<int>();

        private static Dictionary<(int pattern, int level), int[]> _lotteryRates = new Dictionary<(int, int), int[]>();
        private static Dictionary<int, int[]> _sellPrices = new Dictionary<int, int[]>();
        private static Dictionary<int, int> _levelExp = new Dictionary<int, int>();
        private static int[][] _shopPools = new int[6][];
        private static Dictionary<int, int[]> _skillActions = new Dictionary<int, int[]>();
        private static Dictionary<int, int[]> _skillPassives = new Dictionary<int, int[]>();
        private static Dictionary<int, float[]> _skillRanges = new Dictionary<int, float[]>();
        private static Dictionary<int, BattleEquipmentInfo> _equipments = new Dictionary<int, BattleEquipmentInfo>();

        private static Dictionary<int, int> _disassembleTokens = new Dictionary<int, int>();

        private static List<(int min, int max, int gold)> _interestBands = new List<(int, int, int)>();
        private static List<(int min, int max, int gold)> _winBonusBands = new List<(int, int, int)>();
        private static List<(int min, int max, int gold)> _loseBonusBands = new List<(int, int, int)>();

        private static Dictionary<int, BattleGuildBuffInfo> _guildBuffs = new Dictionary<int, BattleGuildBuffInfo>();

        private static Dictionary<int, List<BattleItemDropInfo>> _itemDrops = new Dictionary<int, List<BattleItemDropInfo>>();

        private static Dictionary<(int type, int id), BattleSynergyInfo> _synergies =
            new Dictionary<(int, int), BattleSynergyInfo>();

        private static Dictionary<int, BattleEquipmentSkillInfo> _equipmentSkills =
            new Dictionary<int, BattleEquipmentSkillInfo>();
        private static Dictionary<int, BattlePassiveSkillActionInfo> _passiveSkillActions =
            new Dictionary<int, BattlePassiveSkillActionInfo>();

        private static Dictionary<int, BattleSkillActionInfo> _skillActionInfos =
            new Dictionary<int, BattleSkillActionInfo>();

        private static Dictionary<int, BattleGuildBuffEffectInfo> _guildBuffEffects =
            new Dictionary<int, BattleGuildBuffEffectInfo>();

        private static Dictionary<int, BattleAiKindInfo> _aiKinds =
            new Dictionary<int, BattleAiKindInfo>();
        private static int[] _aiPowerUnitValues = Array.Empty<int>();
        private static int[] _aiPowerSynergyValues = Array.Empty<int>();

        public static string MasterPath { get; private set; }

        private static int ReadInt(in JsonElement e, string name, int def = 0)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : def;

        private static float ReadFloat(in JsonElement e, string name, float def = 0f)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetSingle() : def;

        private static string ReadString(in JsonElement e, string name, string def = "")
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? def : def;

        private static bool ReadBool(in JsonElement e, string name, bool def = false)
            => e.TryGetProperty(name, out var v) && v.ValueKind != JsonValueKind.Null
                ? v.ValueKind == JsonValueKind.True
                : def;

        private static int[] ReadIntArray(in JsonElement e, string name)
        {
            if (!e.TryGetProperty(name, out var v) || v.ValueKind != JsonValueKind.Array) return Array.Empty<int>();
            var list = new List<int>();
            foreach (var item in v.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number) list.Add(item.GetInt32());
            }
            return list.ToArray();
        }

        private static float[] ReadFloatArray(in JsonElement e, string name)
        {
            if (!e.TryGetProperty(name, out var v) || v.ValueKind != JsonValueKind.Array) return Array.Empty<float>();
            var list = new List<float>();
            foreach (var item in v.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number) list.Add(item.GetSingle());
            }
            return list.ToArray();
        }

        public static void EnsureLoaded()
        {
            if (_loaded) return;
            lock (LoadLock)
            {
                if (_loaded) return;
                try
                {
                    MasterPath = Environment.GetEnvironmentVariable("GSERVER_MASTER")
                        ?? Path.Combine(AppContext.BaseDirectory, "master.json");

                    var masterText = AndroidFileBridge.ReadText(MasterPath)
                        ?? throw new FileNotFoundException($"master.json missing at {MasterPath}", MasterPath);
                    using var doc = JsonDocument.Parse(masterText);
                    var root = doc.RootElement;

                if (root.TryGetProperty("unit_data", out var units))
                {
                    foreach (var e in units.EnumerateArray())
                    {
                        var info = new BattleUnitInfo
                        {
                            Id = ReadInt(e, "Id"),
                            Name = e.TryGetProperty("Name", out var un) && un.ValueKind == JsonValueKind.String ? un.GetString() ?? "" : "",
                            Playable = ReadBool(e, "Playable"),
                            IsToken = ReadBool(e, "IsToken"),
                            GuildId = ReadInt(e, "GuildId"),

                            RoleTypes = ReadIntArray(e, "RoleTypes"),
                            TeamTypes = ReadIntArray(e, "TeamTypes"),
                            Price = ReadInt(e, "Price"),
                            Life = ReadInt(e, "Life"),
                            Atk = ReadInt(e, "Atk"),
                            MagicAtk = ReadInt(e, "MagicAtk"),
                            Defence = ReadInt(e, "Defence"),
                            MagicDefence = ReadInt(e, "MagicDefence"),
                            TechnicalPoint = ReadInt(e, "TechnicalPoint"),
                            AttackRecastTime = ReadFloat(e, "AttackRecastTime"),
                            MoveRecastTime = ReadFloat(e, "MoveRecastTime"),
                            AttackRange = ReadFloat(e, "AttackRange"),
                            AttackType = ReadInt(e, "AttackType"),
                        };
                        _units[info.Id] = info;
                    }
                }

                if (root.TryGetProperty("round_data", out var rounds))
                {
                    _loopRounds.Clear();
                    foreach (var e in rounds.EnumerateArray())
                    {
                        var info = new BattleRoundInfo
                        {
                            Number = ReadInt(e, "Number"),
                            IsLoop = ReadBool(e, "IsLoop"),
                            NpcId = ReadInt(e, "NpcId"),
                            DistributeGold = ReadInt(e, "DistributeGold"),
                            WinGold = ReadInt(e, "WinGold"),
                            LoseGold = ReadInt(e, "LoseGold"),
                            WinBonus = ReadInt(e, "WinBonus"),
                            BaseDamage = ReadInt(e, "BaseDamage"),
                            TeamBaseDamage = ReadInt(e, "TeamBaseDamage"),
                            UnitDamage = ReadInt(e, "UnitDamage"),
                            ExperiencePoints = ReadInt(e, "ExperiencePoints"),
                            PrepareTime = ReadInt(e, "PrepareTime"),
                        };
                        _rounds[info.Number] = info;
                        if (info.IsLoop) _loopRounds.Add(info);
                    }
                }

                if (root.TryGetProperty("boss_battle_data", out var bossRows))
                {
                    foreach (var e in bossRows.EnumerateArray())
                    {
                        _bossRow = (ReadInt(e, "BossId"), ReadInt(e, "NpcId"), ReadInt(e, "BoardId"));
                        break;
                    }
                }

                if (root.TryGetProperty("npc_data", out var npcRows))
                {
                    var grouped = new Dictionary<int, List<(int, int, int)>>();
                    foreach (var e in npcRows.EnumerateArray())
                    {
                        var npcId = ReadInt(e, "NpcId");

                        if (npcId < 1 || (npcId > 7 && npcId != _bossRow.NpcId)) continue;
                        if (!grouped.TryGetValue(npcId, out var list))
                        {
                            list = new List<(int, int, int)>();
                            grouped[npcId] = list;
                        }
                        list.Add((ReadInt(e, "UnitId"), ReadInt(e, "PositionX"), ReadInt(e, "PositionY")));
                    }
                    foreach (var kv in grouped)
                    {
                        _npcLayouts[kv.Key] = kv.Value.ToArray();
                    }
                }

                if (root.TryGetProperty("battle_rule_data", out var rules))
                {
                    foreach (var e in rules.EnumerateArray())
                    {
                        var info = new BattleRuleInfo
                        {
                            Id = ReadInt(e, "Id"),
                            BattleRule = ReadInt(e, "BattleRule"),
                            InitialGold = ReadInt(e, "InitialGold"),
                            InitialLevel = ReadInt(e, "InitialLevel"),
                            PlayerMaxLife = ReadInt(e, "PlayerMaxLife"),
                            PrepareTime = ReadInt(e, "PrepareTime", 25),
                            BuyExpValue = ReadInt(e, "BuyExpValue"),
                            ExpPrice = ReadInt(e, "ExpPrice"),
                            RefreshShopPrice = ReadInt(e, "RefreshShopPrice"),
                            ShopUnitLotteryPatternId = ReadInt(e, "ShopUnitLotteryPatternId"),
                        };
                        _rules[info.BattleRule] = info;
                    }
                }

                LoadBandTable(root, "intereset_data", _interestBands);
                LoadBandTable(root, "win_bonus_data", _winBonusBands);
                LoadBandTable(root, "lose_bonus_data", _loseBonusBands);

                var pool = new List<int>();
                foreach (var u in _units.Values)
                {

                    if (u.Playable && !u.IsToken && u.Price >= 1 && u.Price <= 2) pool.Add(u.Id);
                }
                pool.Sort();
                _initialPool = pool.ToArray();

                if (root.TryGetProperty("avatar_data", out var avatars))
                {
                    var list = new List<int>();
                    var maxAvail = -1;
                    foreach (var e in avatars.EnumerateArray())
                    {
                        list.Add(ReadInt(e, "AvatarId"));
                        var avail = ReadInt(e, "AvailableIndex", -1);
                        if (avail > maxAvail) maxAvail = avail;
                    }
                    _avatarIds = list.ToArray();
                    _maxAvatarAvailableIndex = maxAvail;
                }

                if (root.TryGetProperty("grade_data", out var grades))
                {
                    var list = new List<int>();
                    var ranges = new List<(int, int, int)>();
                    foreach (var e in grades.EnumerateArray())
                    {
                        var gid = ReadInt(e, "GradeId");
                        list.Add(gid);
                        ranges.Add((gid, ReadInt(e, "MinRequirePoint"), ReadInt(e, "MaxRequirePoint")));
                    }
                    _gradeIds = list.ToArray();
                    _gradeRanges = ranges.ToArray();
                }

                if (root.TryGetProperty("unit_level_table_data", out var unitLevels))
                {
                    foreach (var e in unitLevels.EnumerateArray())
                    {
                        var uid = ReadInt(e, "UnitId");
                        if (uid > 0)
                        {
                            _unitRankRates[uid] = (ReadFloat(e, "Rank2Rate"), ReadFloat(e, "Rank3Rate"));
                        }
                    }
                }

                if (root.TryGetProperty("emote_data", out var emotes) && emotes.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in emotes.EnumerateArray()) _emoteIds.Add(ReadInt(e, "Id"));
                }

                if (root.TryGetProperty("recommend_party", out var recParties) && recParties.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in recParties.EnumerateArray()) _recommendPartyIds.Add(ReadInt(e, "Id"));
                }

                if (root.TryGetProperty("shop_unit_lottery_data", out var lottery))
                {
                    foreach (var e in lottery.EnumerateArray())
                    {
                        var pid = ReadInt(e, "LotteryPatternId");
                        var lvl = ReadInt(e, "PlayerLevel");
                        _lotteryRates[(pid, lvl)] = new[]
                        {
                            ReadInt(e, "Tier1OddsRate"),
                            ReadInt(e, "Tier2OddsRate"),
                            ReadInt(e, "Tier3OddsRate"),
                            ReadInt(e, "Tier4OddsRate"),
                            ReadInt(e, "Tier5OddsRate"),
                        };
                    }
                }

                if (root.TryGetProperty("sell_unit_price_data", out var sellPrices))
                {
                    foreach (var e in sellPrices.EnumerateArray())
                    {
                        var rank = ReadInt(e, "Rank");
                        _sellPrices[rank] = new[]
                        {
                            ReadInt(e, "Tier1Price"),
                            ReadInt(e, "Tier2Price"),
                            ReadInt(e, "Tier3Price"),
                            ReadInt(e, "Tier4Price"),
                            ReadInt(e, "Tier5Price"),
                        };
                    }
                }

                if (root.TryGetProperty("player_level_data", out var levels))
                {
                    foreach (var e in levels.EnumerateArray())
                    {
                        _levelExp[ReadInt(e, "Level")] = ReadInt(e, "Exp");
                    }
                }

                for (int tier = 1; tier <= 5; tier++)
                {
                    var list = new List<int>();
                    foreach (var u in _units.Values)
                    {
                        if (u.Playable && !u.IsToken && u.Price == tier) list.Add(u.Id);
                    }
                    list.Sort();
                    _shopPools[tier] = list.ToArray();
                }

                if (root.TryGetProperty("skill_data", out var skills))
                {
                    foreach (var e in skills.EnumerateArray())
                    {
                        var sid = ReadInt(e, "Id");
                        if (sid <= 0) continue;
                        var actions = new List<int>();
                        if (e.TryGetProperty("SkillActionIds", out var arr) && arr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var a in arr.EnumerateArray())
                            {
                                if (a.ValueKind == JsonValueKind.Number && a.GetInt32() > 0) actions.Add(a.GetInt32());
                            }
                        }
                        if (actions.Count > 0) _skillActions[sid] = actions.ToArray();

                        var passives = new List<int>();
                        if (e.TryGetProperty("PassiveSkillActionIds", out var parr) && parr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var a in parr.EnumerateArray())
                            {
                                if (a.ValueKind == JsonValueKind.Number && a.GetInt32() > 0) passives.Add(a.GetInt32());
                            }
                        }
                        if (passives.Count > 0) _skillPassives[sid] = passives.ToArray();

                        var skillRange = ReadFloatArray(e, "range");
                        if (skillRange.Length > 0) _skillRanges[sid] = skillRange;
                    }
                }

                _skillActionInfos.Clear();
                if (root.TryGetProperty("skill_action_data", out var skillActions))
                {
                    foreach (var e in skillActions.EnumerateArray())
                    {
                        var aid = ReadInt(e, "Id");
                        if (aid <= 0) continue;
                        var values = new List<float[]>();
                        if (e.TryGetProperty("ActionValues", out var av) && av.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var group in av.EnumerateArray())
                            {
                                if (group.ValueKind != JsonValueKind.Array) continue;
                                var arr = new List<float>();
                                foreach (var v in group.EnumerateArray())
                                    if (v.ValueKind == JsonValueKind.Number) arr.Add(v.GetSingle());
                                values.Add(arr.ToArray());
                            }
                        }
                        _skillActionInfos[aid] = new BattleSkillActionInfo
                        {
                            Id = aid,
                            SkillActionType = ReadInt(e, "SkillActionType"),
                            ActionValues = values.ToArray(),
                            TargetCounts = ReadIntArray(e, "targetCount"),
                            AssignmentType = ReadInt(e, "AssignmentType"),
                            TargetType = ReadInt(e, "TargetType"),
                            TargetExtras = ReadString(e, "TargetExtras"),
                        };
                    }
                }

                _guildBuffEffects.Clear();
                if (root.TryGetProperty("guild_buff_effect_data", out var guildEffects))
                {
                    foreach (var e in guildEffects.EnumerateArray())
                    {
                        var bid = ReadInt(e, "GuildBuffId");
                        if (bid <= 0) continue;
                        var effectValues = new List<float>();
                        if (e.TryGetProperty("EffectValues", out var ev) && ev.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var v in ev.EnumerateArray())
                                if (v.ValueKind == JsonValueKind.Number) effectValues.Add(v.GetSingle());
                        }
                        _guildBuffEffects[bid] = new BattleGuildBuffEffectInfo
                        {
                            GuildBuffId = bid,
                            BuffType = ReadInt(e, "BuffType"),
                            Life = ReadInt(e, "Life"),
                            Atk = ReadInt(e, "Atk"),
                            MagicAtk = ReadInt(e, "MagicAtk"),
                            Defence = ReadInt(e, "Defence"),
                            MagicDefence = ReadInt(e, "MagicDefence"),
                            TechnicalPointRate = ReadFloat(e, "TechnicalPointRate"),
                            AttackRecastTimeTickRate = ReadInt(e, "AttackRecastTimeTickRate"),
                            MoveRecastTimeTickRate = ReadInt(e, "MoveRecastTimeTickRate"),
                            AttackRange = ReadFloat(e, "AttackRange"),
                            EffectValues = effectValues.ToArray(),
                        };
                    }
                }

                _aiKinds.Clear();
                if (root.TryGetProperty("battle_ai_kind", out var aiKinds))
                {
                    foreach (var e in aiKinds.EnumerateArray())
                    {
                        var aid2 = ReadInt(e, "Id");
                        if (aid2 <= 0) continue;
                        _aiKinds[aid2] = new BattleAiKindInfo
                        {
                            Id = aid2,
                            StrategySetId = ReadInt(e, "StrategySetId"),
                            TargetPartyPower = ReadInt(e, "TargetPartyPower"),
                            EmoteProb = ReadFloat(e, "EmoteProb"),
                            EmoteDelaySecMin = ReadInt(e, "EmoteDelaySecMin"),
                            EmoteDelaySecMax = ReadInt(e, "EmoteDelaySecMax"),
                            EmoteTimesMin = ReadInt(e, "EmoteTimesMin"),
                            EmoteTimesMax = ReadInt(e, "EmoteTimesMax"),
                            MissProbRefreshShop = ReadFloat(e, "MissProbRefreshShop"),
                            MissProbBuyUnit = ReadFloat(e, "MissProbBuyUnit"),
                            MissProbLevelup = ReadFloat(e, "MissProbLevelup"),
                            MissProbDeployUnit = ReadFloat(e, "MissProbDeployUnit"),
                            MinCommandDelay = ReadInt(e, "MinCommandDelay"),
                            MaxCommandDelay = ReadInt(e, "MaxCommandDelay"),
                        };
                    }
                }

                _aiPowerUnitValues = Array.Empty<int>();
                _aiPowerSynergyValues = Array.Empty<int>();
                if (root.TryGetProperty("battle_ai_power", out var aiPowers))
                {
                    foreach (var e in aiPowers.EnumerateArray())
                    {
                        if (ReadInt(e, "Id") != 1) continue;
                        _aiPowerUnitValues = ReadIntArray(e, "UnitRankValue");
                        _aiPowerSynergyValues = ReadIntArray(e, "SynergyLevelValue");
                        break;
                    }
                }

                if (root.TryGetProperty("equipment_data", out var equips))
                {
                    foreach (var e in equips.EnumerateArray())
                    {
                        var info = new BattleEquipmentInfo
                        {
                            Id = ReadInt(e, "Id"),
                            Name = e.TryGetProperty("Name", out var nm) && nm.ValueKind == JsonValueKind.String ? nm.GetString() ?? "" : "",
                            Type = ReadInt(e, "Type"),
                            Life = ReadInt(e, "Life"),
                            Atk = ReadInt(e, "Atk"),
                            MagicAtk = ReadInt(e, "MagicAtk"),
                            Defence = ReadInt(e, "Defence"),
                            MagicDefence = ReadInt(e, "MagicDefence"),
                            TechnicalPointRate = ReadFloat(e, "TechnicalPointRate"),
                            AttackRecastTimeTickRate = ReadInt(e, "AttackRecastTimeTickRate"),
                            MoveRecastTimeTickRate = ReadInt(e, "MoveRecastTimeTickRate"),
                            AttackRange = ReadFloat(e, "AttackRange"),
                            AttackType = ReadInt(e, "AttackType"),
                            AfterSynthesisIds = ReadIntArray(e, "AfterSynthesisIds"),
                            Rank = ReadInt(e, "Rank"),
                            RoleType = ReadInt(e, "RoleType"),
                            TeamType = ReadInt(e, "TeamType"),
                            SelectableEquipmentIds = ReadIntArray(e, "SelectableEquipmentIds"),
                            Description = e.TryGetProperty("Description", out var d) && d.ValueKind == JsonValueKind.String ? d.GetString() ?? "" : "",
                            SkillDamageScale = ReadInt(e, "SkillDamageScale"),
                        };
                        if (info.Id > 0) _equipments[info.Id] = info;
                    }
                }

                _disassembleTokens.Clear();
                if (root.TryGetProperty("disassemble_equipment_data", out var disassemble))
                {
                    foreach (var e in disassemble.EnumerateArray())
                    {
                        var tier = ReadInt(e, "Tier");
                        if (tier <= 0) continue;
                        _disassembleTokens[tier] = ReadInt(e, "GetTokenNum");
                    }
                }

                _guildBuffs.Clear();
                if (root.TryGetProperty("guild_buff_data", out var guildBuffs) && guildBuffs.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in guildBuffs.EnumerateArray())
                    {
                        var info = new BattleGuildBuffInfo
                        {
                            GuildId = ReadInt(e, "GuildId"),
                            GuildBuffIds = ReadIntArray(e, "GuildBuffIds"),
                            LotteryRate = ReadInt(e, "LotteryRate"),
                        };
                        if (info.GuildId > 0) _guildBuffs[info.GuildId] = info;
                    }
                }

                _synergies.Clear();
                if (root.TryGetProperty("role_synergy_data", out var roleSyn) && roleSyn.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in roleSyn.EnumerateArray())
                    {
                        var info = new BattleSynergyInfo
                        {
                            SynergyType = 1,
                            Id = ReadInt(e, "Id"),
                            LowCount = ReadInt(e, "LowCount"),
                            MiddleCount = ReadInt(e, "MiddleCount"),
                            HighCount = ReadInt(e, "HighCount"),
                            LowValues = ReadFloatArray(e, "LowValue"),
                            MiddleValues = ReadFloatArray(e, "MiddleValue"),
                            HighValues = ReadFloatArray(e, "HighValue"),
                        };
                        if (info.Id > 0) _synergies[(1, info.Id)] = info;
                    }
                }
                if (root.TryGetProperty("team_synergy_data", out var teamSyn) && teamSyn.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in teamSyn.EnumerateArray())
                    {
                        var info = new BattleSynergyInfo
                        {
                            SynergyType = 0,
                            Id = ReadInt(e, "Id"),
                            LowCount = ReadInt(e, "LowCount"),
                            MiddleCount = ReadInt(e, "MiddleCount"),
                            HighCount = ReadInt(e, "HighCount"),
                            LowValues = ReadFloatArray(e, "LowValue"),
                            MiddleValues = ReadFloatArray(e, "MiddleValue"),
                            HighValues = ReadFloatArray(e, "HighValue"),
                        };
                        if (info.Id > 0) _synergies[(0, info.Id)] = info;
                    }
                }

                _equipmentSkills.Clear();
                _passiveSkillActions.Clear();
                if (root.TryGetProperty("equipment_skill_data", out var eqSkills) && eqSkills.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in eqSkills.EnumerateArray())
                    {
                        var info = new BattleEquipmentSkillInfo
                        {
                            Id = ReadInt(e, "Id"),
                            PassiveSkillActionIds = ReadIntArray(e, "PassiveSkillActionIds"),
                            Range = ReadFloatArray(e, "range"),
                        };
                        if (info.Id > 0) _equipmentSkills[info.Id] = info;
                    }
                }
                if (root.TryGetProperty("passive_skill_action_data", out var psa) && psa.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in psa.EnumerateArray())
                    {
                        var info = new BattlePassiveSkillActionInfo
                        {
                            Id = ReadInt(e, "Id"),
                            SkillActionType = ReadInt(e, "SkillActionType"),
                            TargetCount = ReadIntArray(e, "targetCount"),
                        };

                        var vals = new List<float>();
                        if (e.TryGetProperty("ActionValues", out var av) && av.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var g in av.EnumerateArray())
                            {
                                if (g.ValueKind != JsonValueKind.Array) continue;
                                float first = 0f;
                                bool got = false;
                                foreach (var item in g.EnumerateArray())
                                {
                                    if (item.ValueKind == JsonValueKind.Number) { first = item.GetSingle(); got = true; break; }
                                }
                                if (got) vals.Add(first);
                            }
                        }
                        info.Values = vals.ToArray();
                        if (info.Id > 0) _passiveSkillActions[info.Id] = info;
                    }
                }

                _itemDrops.Clear();
                if (root.TryGetProperty("item_drop_data", out var itemDrops) && itemDrops.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in itemDrops.EnumerateArray())
                    {
                        var info = new BattleItemDropInfo
                        {
                            Round = ReadInt(e, "Round"),
                            ItemId = ReadInt(e, "ItemId"),
                            RewardNumber = ReadInt(e, "RewardNumber"),
                            RelativeOdds = ReadInt(e, "RelativeOdds"),
                        };
                        if (info.Round <= 0 || info.ItemId <= 0 || info.RewardNumber <= 0 || info.RelativeOdds <= 0)
                        {
                            Console.WriteLine($"[MASTER] item_drop_data skip invalid row Round={info.Round} ItemId={info.ItemId} RewardNumber={info.RewardNumber} Odds={info.RelativeOdds}");
                            continue;
                        }
                        if (!_itemDrops.TryGetValue(info.Round, out var list))
                        {
                            list = new List<BattleItemDropInfo>();
                            _itemDrops[info.Round] = list;
                        }
                        list.Add(info);
                    }
                }

                _loaded = true;
                Console.WriteLine($"[MASTER] loaded {MasterPath} units={_units.Count} rounds={_rounds.Count} loopRounds={_loopRounds.Count} npcLayouts={_npcLayouts.Count} rules={_rules.Count} initialPool={_initialPool.Length} avatars={_avatarIds.Length} grades={_gradeIds.Length} lotteryRows={_lotteryRates.Count} sellPriceRanks={_sellPrices.Count} levels={_levelExp.Count} shopPools=[{string.Join(",", _shopPools.Skip(1).Select(p => p?.Length ?? 0))}] equipments={_equipments.Count} disassembleTiers={_disassembleTokens.Count} guildBuffs={_guildBuffs.Count} synergies={_synergies.Count} skillActions={_skillActionInfos.Count} guildBuffEffects={_guildBuffEffects.Count} aiKinds={_aiKinds.Count} aiPower=[{string.Join(",", _aiPowerUnitValues)}/{string.Join(",", _aiPowerSynergyValues)}] itemDrops={_itemDrops.Count}r/{_itemDrops.Values.Sum(l => l.Count)} attackClips={AnimClipTable.AttackCount} skillClips={AnimClipTable.SkillCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MASTER] EnsureLoaded EXCEPTION: {ex}");
                    throw;
                }
            }
        }

        public static BattleUnitInfo GetUnit(int unitId)
        {
            EnsureLoaded();
            return _units.TryGetValue(unitId, out var u) ? u : null;
        }

        public static BattleEquipmentInfo GetEquipment(int equipmentId)
        {
            EnsureLoaded();
            return _equipments.TryGetValue(equipmentId, out var e) ? e : null;
        }

        public static int[] GetEquipmentPassiveIds(int equipmentSkillId)
        {
            EnsureLoaded();
            return _equipmentSkills.TryGetValue(equipmentSkillId, out var s) ? s.PassiveSkillActionIds : Array.Empty<int>();
        }

        public static float GetEquipmentSkillRange(int equipmentSkillId)
        {
            EnsureLoaded();
            if (!_equipmentSkills.TryGetValue(equipmentSkillId, out var s) || s.Range == null || s.Range.Length == 0) return 0f;
            return s.Range[0];
        }

        public static BattlePassiveSkillActionInfo GetPassiveSkillAction(int passiveId)
        {
            EnsureLoaded();
            return _passiveSkillActions.TryGetValue(passiveId, out var a) ? a : null;
        }

        public static bool TryGetDisassembleToken(int equipmentRank, out int tokenNum)
        {
            EnsureLoaded();
            return _disassembleTokens.TryGetValue(equipmentRank, out tokenNum);
        }

        public static int[] OrdinaryEquipmentIds
        {
            get
            {
                EnsureLoaded();
                return _equipments.Values
                    .Where(e => e.Id >= 300000 && e.Id < 400000 && e.Rank >= 1)
                    .OrderBy(e => e.Id)
                    .Select(e => e.Id)
                    .ToArray();
            }
        }

        public static BattleItemDropInfo ChooseItemDrop(int encounter, Random rng)
        {
            EnsureLoaded();
            if (_itemDrops.Count == 0) return null;
            int maxRound = _itemDrops.Keys.Max();
            if (encounter > maxRound) encounter = maxRound;
            if (!_itemDrops.TryGetValue(encounter, out var entries) || entries.Count == 0)
            {
                Console.WriteLine($"[MASTER] item_drop miss encounter={encounter}");
                return null;
            }
            int total = 0;
            foreach (var e in entries) total += e.RelativeOdds;
            int draw = rng.Next(total);
            int boundary = 0;
            foreach (var e in entries)
            {
                boundary += e.RelativeOdds;
                if (draw < boundary) return e;
            }
            return entries[entries.Count - 1];
        }

        public static int NpcIdForRound(int round)
        {
            EnsureLoaded();
            return GetRound(round)?.NpcId ?? 0;
        }

        public static BattleRoundInfo GetRound(int round)
        {
            EnsureLoaded();
            if (_rounds.TryGetValue(round, out var direct)) return direct;
            if (_loopRounds.Count == 0) return null;
            var loopStart = _loopRounds[0].Number;
            var index = ((round - loopStart) % _loopRounds.Count + _loopRounds.Count) % _loopRounds.Count;
            return _loopRounds[index];
        }

        public static (int unitId, int x, int y)[] NpcLayout(int npcId)
        {
            EnsureLoaded();
            return _npcLayouts.TryGetValue(npcId, out var layout) ? layout
                : Array.Empty<(int, int, int)>();
        }

        public static BattleRuleInfo GetRule(int battleRule)
        {
            EnsureLoaded();
            return _rules.TryGetValue(battleRule, out var r) ? r : null;
        }

        public static bool IsEmoteIdValid(int emoteId)
        {
            EnsureLoaded();
            return _emoteIds.Contains(emoteId);
        }

        public static bool IsRecommendPartyIdValid(int partyId)
        {
            EnsureLoaded();
            return partyId == 0 || _recommendPartyIds.Contains(partyId);
        }

        public static IReadOnlyList<BattleGuildBuffInfo> GuildBuffRows
        {
            get
            {
                EnsureLoaded();
                return _guildBuffs.Values.OrderBy(g => g.GuildId).ToArray();
            }
        }

        public static int[] GuildNonTokenUnitIds(int guildId)
        {
            EnsureLoaded();
            return _units.Values
                .Where(u => u.GuildId == guildId && !u.IsToken)
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .ToArray();
        }

        public static int[] SynergyTeamUnitIds(int teamId)
        {
            EnsureLoaded();
            return _units.Values
                .Where(u => !u.IsToken && Array.IndexOf(u.TeamTypes, teamId) >= 0)
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .ToArray();
        }

        private static int SynergyTier(BattleSynergyInfo def, int count)
        {
            if (count < def.LowCount) return 0;
            if (count < def.MiddleCount) return 1;
            if (count < def.HighCount) return 2;
            return def.MiddleCount == def.HighCount ? 2 : 3;
        }

        public static BattleSynergyMpo[] EvaluateFieldSynergies(IEnumerable<BattleUnitDataMpo> fieldUnits)
        {
            EnsureLoaded();
            if (fieldUnits == null) return Array.Empty<BattleSynergyMpo>();

            var distinct = new Dictionary<int, (BattleUnitInfo info, List<BattleEquipmentDataMpo> gears,
                ImmutableHashSet<UnitData.ROLE_TYPE> mirrorRoles, ImmutableHashSet<UnitData.TEAM_TYPE> mirrorTeams)>();
            foreach (var u in fieldUnits)
            {
                if (u == null || u.IsToken || u.IsNpc || u.IsHelp) continue;
                if (distinct.ContainsKey(u.UnitId)) continue;
                var info = GetUnit(u.UnitId);
                if (info == null || info.IsToken) continue;
                distinct[u.UnitId] = (info, u.EquipmentList, u.MirroringRoleType, u.MirroringTeamType);
            }

            var counts = new Dictionary<(int type, int id), int>();
            foreach (var entry in distinct.Values)
            {

                var memberships = new HashSet<(int type, int id)>();
                foreach (var r in entry.info.RoleTypes) memberships.Add((1, r));
                foreach (var t in entry.info.TeamTypes) memberships.Add((0, t));

                if (entry.mirrorRoles != null)
                    foreach (var r in entry.mirrorRoles) memberships.Add((1, (int)r));
                if (entry.mirrorTeams != null)
                    foreach (var t in entry.mirrorTeams) memberships.Add((0, (int)t));
                if (entry.gears != null)
                {
                    foreach (var g in entry.gears)
                    {
                        var ge = g != null ? GetEquipment(g.Id) : null;
                        if (ge == null) continue;
                        if (ge.RoleType > 0) memberships.Add((1, ge.RoleType));
                        if (ge.TeamType > 0) memberships.Add((0, ge.TeamType));
                    }
                }
                foreach (var key in memberships)
                {
                    if (!_synergies.ContainsKey(key)) continue;
                    counts[key] = counts.TryGetValue(key, out var c) ? c + 1 : 1;
                }
            }

            var list = new List<BattleSynergyMpo>(counts.Count);
            foreach (var kv in counts.OrderBy(k => k.Key.type).ThenBy(k => k.Key.id))
            {
                var def = _synergies[kv.Key];
                var tier = SynergyTier(def, kv.Value);

                var next = tier == 0 ? def.LowCount
                    : tier == 1 ? def.MiddleCount
                    : def.HighCount;
                list.Add(new BattleSynergyMpo
                {
                    SynergyType = (Byac.ServerShared.Enums.SynergyType)kv.Key.type,
                    Id = kv.Key.id,
                    CurrentFieldCount = kv.Value,
                    NextCount = next,
                    IsConfirmed = tier > 0,
                });
            }
            return list.ToArray();
        }

        public static BattleUnitMirroringPairDataMpo[] ApplyMirroring(IEnumerable<BattleUnitDataMpo> fieldUnits)
        {
            EnsureLoaded();
            if (fieldUnits == null) return Array.Empty<BattleUnitMirroringPairDataMpo>();

            var instances = new List<(BattleUnitDataMpo u, int unitId, int x, int y,
                List<int> roles, List<int> teams)>();
            foreach (var u in fieldUnits)
            {
                if (u == null || u.IsToken || u.IsNpc || u.IsHelp) continue;
                var info = GetUnit(u.UnitId);
                if (info == null || info.IsToken) continue;
                var roles = new List<int>(info.RoleTypes);
                var teams = new List<int>(info.TeamTypes);
                if (u.EquipmentList != null)
                {
                    foreach (var g in u.EquipmentList)
                    {
                        var ge = g != null ? GetEquipment(g.Id) : null;
                        if (ge == null) continue;
                        if (ge.RoleType > 0) roles.Add(ge.RoleType);
                        if (ge.TeamType > 0) teams.Add(ge.TeamType);
                    }
                }
                u.MirroringRoleType = ImmutableHashSet<UnitData.ROLE_TYPE>.Empty;
                u.MirroringTeamType = ImmutableHashSet<UnitData.TEAM_TYPE>.Empty;
                instances.Add((u, u.UnitId, u.PositionX, u.PositionY, roles, teams));
            }

            var pairs = new List<BattleUnitMirroringPairDataMpo>();
            for (int i = 0; i < instances.Count; i++)
            {
                var src = instances[i];
                if (!HasPassiveType(src.unitId, 10700111)) continue;
                int best = -1, bestDist = int.MaxValue, bestUk = int.MaxValue;
                for (int j = 0; j < instances.Count; j++)
                {
                    if (i == j) continue;
                    int d = HexDistance(src.x, src.y, instances[j].x, instances[j].y);
                    int uk = instances[j].u.UniqueKey;
                    if (d < bestDist || (d == bestDist && uk < bestUk))
                    {
                        bestDist = d; bestUk = uk; best = j;
                    }
                }
                if (best < 0) continue;
                var near = instances[best];
                src.u.MirroringRoleType = ImmutableHashSet.CreateRange(
                    near.roles.Select(r => (UnitData.ROLE_TYPE)r));
                src.u.MirroringTeamType = ImmutableHashSet.CreateRange(
                    near.teams.Select(t => (UnitData.TEAM_TYPE)t));
                pairs.Add(new BattleUnitMirroringPairDataMpo
                {
                    MainUniqueKey = src.u.UniqueKey,
                    TargetUniqueKey = near.u.UniqueKey,
                });
            }
            return pairs.ToArray();
        }

        private static int HexDistance(int x1, int y1, int x2, int y2)
        {
            int cx1 = x1 - ((y1 & ~1) >> 1);
            int cx2 = x2 - ((y2 & ~1) >> 1);
            int dcx = cx1 - cx2, dy = y1 - y2;
            return (Math.Abs(dcx) + Math.Abs(dy) + Math.Abs(dcx + dy)) >> 1;
        }

        private static bool HasPassiveType(int unitId, int passiveType)
        {
            EnsureLoaded();
            foreach (var pid in UnitPassiveSkillActionIds(unitId))
            {
                if (pid == 0) continue;
                var pa = GetPassiveSkillAction(pid);
                if (pa != null && pa.SkillActionType == passiveType) return true;
            }
            return false;
        }

        public static List<ActiveSynergyEffect> EvaluateActiveSynergyEffects(IEnumerable<BattleUnitDataMpo> fieldUnits)
        {
            var result = new List<ActiveSynergyEffect>();
            if (fieldUnits == null) return result;
            var synergies = EvaluateFieldSynergies(fieldUnits);
            foreach (var s in synergies)
            {
                if (s == null || !s.IsConfirmed) continue;
                var key = (Type: (int)s.SynergyType, Id: s.Id);
                if (!_synergies.TryGetValue(key, out var def)) continue;

                int count = s.CurrentFieldCount;
                int tier = SynergyTier(def, count);
                if (tier <= 0) continue;
                var values = def.ValuesForLevel(tier);
                if (values.Length == 0) continue;
                result.Add(new ActiveSynergyEffect
                {
                    SynergyType = key.Type,
                    Id = key.Id,
                    Level = tier,
                    Values = values,
                });
            }
            return result;
        }

        private static void LoadBandTable(JsonElement root, string key, List<(int min, int max, int gold)> bands)
        {
            bands.Clear();
            if (!root.TryGetProperty(key, out var rows) || rows.ValueKind != JsonValueKind.Array) return;
            foreach (var e in rows.EnumerateArray())
            {
                bands.Add((ReadInt(e, "MinAmount"), ReadInt(e, "MaxAmount"), ReadInt(e, "InterestGold")));
            }
        }

        private static int FindBand(List<(int min, int max, int gold)> bands, int amount)
        {
            if (bands.Count == 0) return 0;
            foreach (var band in bands)
            {
                if (amount >= band.min && amount <= band.max) return band.gold;
            }
            if (amount < bands[0].min) return bands[0].gold;
            return bands[bands.Count - 1].gold;
        }

        public static int FindInterest(int gold)
        {
            EnsureLoaded();
            return FindBand(_interestBands, gold);
        }

        public static int WinBonus(int winStreak)
        {
            EnsureLoaded();
            return FindBand(_winBonusBands, winStreak);
        }

        public static int LoseBonus(int loseStreak)
        {
            EnsureLoaded();
            return FindBand(_loseBonusBands, loseStreak);
        }

        public static int[] InitialPool
        {
            get { EnsureLoaded(); return _initialPool; }
        }

        public static int[] AvatarIds
        {
            get { EnsureLoaded(); return _avatarIds; }
        }

        public static int[] GradeIds
        {
            get { EnsureLoaded(); return _gradeIds; }
        }

        public static int GradeOf(int battlePoint)
        {
            EnsureLoaded();
            if (_gradeRanges.Length == 0) return 7;
            var topGrade = _gradeRanges[0].gradeId;
            var topMax = int.MinValue;
            var bottomGrade = _gradeRanges[0].gradeId;
            var bottomMin = int.MaxValue;
            foreach (var r in _gradeRanges)
            {
                if (battlePoint >= r.min && battlePoint <= r.max) return r.gradeId;
                if (r.max > topMax) { topMax = r.max; topGrade = r.gradeId; }
                if (r.min < bottomMin) { bottomMin = r.min; bottomGrade = r.gradeId; }
            }
            return battlePoint > topMax ? topGrade : bottomGrade;
        }

        public static float UnitRankMultiplier(int unitId, int rank)
        {
            if (rank <= 1) return 1.0f;
            EnsureLoaded();
            if (!_unitRankRates.TryGetValue(unitId, out var rates)) return 1.0f;
            if (rank == 2) return rates.rank2Rate / 100f;
            return rates.rank2Rate * rates.rank3Rate / 10000f;
        }

        public static int MaxAvatarAvailableIndex
        {
            get { EnsureLoaded(); return _maxAvatarAvailableIndex < 0 ? 0 : _maxAvatarAvailableIndex; }
        }

        public static int[] ShopLotteryRates(int patternId, int playerLevel)
        {
            EnsureLoaded();
            int[] best = null;
            var bestLevel = int.MinValue;
            foreach (var kv in _lotteryRates)
            {
                if (kv.Key.pattern != patternId) continue;
                if (kv.Key.level <= playerLevel && kv.Key.level > bestLevel)
                {
                    bestLevel = kv.Key.level;
                    best = kv.Value;
                }
            }
            return best ?? new[] { 1000, 0, 0, 0, 0 };
        }

        public static int SellUnitPrice(int rank, int tier)
        {
            EnsureLoaded();
            var r = Math.Max(1, Math.Min(3, rank));
            var t = Math.Max(1, Math.Min(5, tier));
            if (_sellPrices.TryGetValue(r, out var row)) return row[t - 1];
            return t;
        }

        public static int ExpToNext(int level)
        {
            EnsureLoaded();
            return _levelExp.TryGetValue(level + 1, out var exp) ? exp : -1;
        }

        public static int MaxLevel
        {
            get { EnsureLoaded(); return _levelExp.Count == 0 ? 0 : _levelExp.Keys.Max(); }
        }

        public static int[] ShopTierPool(int tier)
        {
            EnsureLoaded();
            return tier >= 1 && tier <= 5 ? _shopPools[tier] ?? Array.Empty<int>() : Array.Empty<int>();
        }

        public static int SkillActionId(int unitId)
        {
            EnsureLoaded();
            return _skillActions.TryGetValue(unitId, out var ids) && ids.Length > 0 ? ids[0] : 0;
        }

        public static int[] SkillActionIds(int unitId)
        {
            EnsureLoaded();
            return _skillActions.TryGetValue(unitId, out var ids) ? ids : Array.Empty<int>();
        }

        public static int[] UnitPassiveSkillActionIds(int unitId)
        {
            EnsureLoaded();
            return _skillPassives.TryGetValue(unitId, out var ids) ? ids : Array.Empty<int>();
        }

        public static float UnitSkillRange(int unitId, int rank)
        {
            EnsureLoaded();
            if (!_skillRanges.TryGetValue(unitId, out var r) || r.Length == 0) return 99f;
            float v = r[Math.Min(Math.Max(rank - 1, 0), r.Length - 1)];
            if (v > 0f) return v;
            float last = r[r.Length - 1];
            return last > 0f ? last : 1f;
        }

        public static BattleSkillActionInfo SkillAction(int actionId)
        {
            EnsureLoaded();
            return _skillActionInfos.TryGetValue(actionId, out var row) ? row : null;
        }

        public static BattleGuildBuffEffectInfo GuildBuffEffect(int buffId)
        {
            EnsureLoaded();
            return _guildBuffEffects.TryGetValue(buffId, out var row) ? row : null;
        }

        public static IReadOnlyCollection<BattleAiKindInfo> AiKinds
        {
            get { EnsureLoaded(); return _aiKinds.Values; }
        }

        public static int AiPartyPower(IEnumerable<BattleUnitDataMpo> fieldUnits)
        {
            EnsureLoaded();
            if (fieldUnits == null || _aiPowerUnitValues.Length == 0) return 0;
            int total = 0;
            var seen = new HashSet<int>();
            var counts = new Dictionary<(int type, int id), int>();
            foreach (var u in fieldUnits)
            {
                if (u == null || u.IsToken || u.IsNpc || u.IsHelp) continue;
                if (!seen.Add(u.UnitId)) continue;
                var info = GetUnit(u.UnitId);
                if (info == null || info.IsToken) continue;
                var rank = Math.Max(1, u.Rank);
                var rIdx = Math.Min(rank, _aiPowerUnitValues.Length) - 1;
                total += _aiPowerUnitValues[rIdx];
                foreach (var r in info.RoleTypes)
                {
                    var key = (1, r);
                    if (!_synergies.ContainsKey(key)) continue;
                    counts[key] = counts.TryGetValue(key, out var c) ? c + 1 : 1;
                }
                foreach (var t in info.TeamTypes)
                {
                    var key = (0, t);
                    if (!_synergies.ContainsKey(key)) continue;
                    counts[key] = counts.TryGetValue(key, out var c) ? c + 1 : 1;
                }
            }
            foreach (var kv in counts)
            {
                var tier = SynergyTier(_synergies[kv.Key], kv.Value);
                if (tier >= 1 && _aiPowerSynergyValues.Length > 0)
                    total += _aiPowerSynergyValues[Math.Min(tier, _aiPowerSynergyValues.Length) - 1];
            }
            return total;
        }
    }
}
