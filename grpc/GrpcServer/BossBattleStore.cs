using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Byac.Services
{

    public static class BossBattleStore
    {

        public const int BossUnitId = 310100;
        public const int MaxDecks = 20;
        public const int MaxResults = 20;

        private static readonly object Gate = new object();
        private static readonly Dictionary<int, BossState> Cache = new();

        public sealed class SynRec
        {
            public int Id;
            public int Count;
        }

        public sealed class DeckUnitRec
        {
            public int Key;
            public int UnitId;
            public int Rank;
            public int X;
            public int Y;
            public int[] Equips = Array.Empty<int>();
        }

        public sealed class DeckRec
        {
            public int Id;
            public int Rank;
            public int Level;
            public int Avatar;
            public int Power;

            public int Damage;
            public string Utc = "";
            public List<SynRec> Roles = new List<SynRec>();
            public List<SynRec> Teams = new List<SynRec>();
            public List<DeckUnitRec> Units = new List<DeckUnitRec>();
        }

        public sealed class ResultRec
        {
            public int Max;
            public int Remain;
            public int Dmg;
            public bool Defeated;
            public string Utc = "";
        }

        internal sealed class BossState
        {
            public int Hp;
            public int NextDeckId = 1;
            public List<DeckRec> Decks = new List<DeckRec>();
            public Dictionary<string, ResultRec> Results = new Dictionary<string, ResultRec>();
        }

        public static int MaxHp()
        {
            var info = Byac.Services.Battle.BattleMaster.GetUnit(BossUnitId);
            return info != null && info.Life > 0 ? info.Life : 50000;
        }

        private static BossState Ensure(int viewerId)
        {
            if (Cache.TryGetValue(viewerId, out var st)) return st;
            var seeded = new BossState();
            try
            {
                var row = AccountStore.LoadBossState(viewerId);
                if (row.HasValue)
                {
                    seeded.Hp = row.Value.Hp;
                    seeded.NextDeckId = Math.Max(1, row.Value.NextDeckId);
                    seeded.Decks = JsonConvert.DeserializeObject<List<DeckRec>>(row.Value.DecksJson) ?? new List<DeckRec>();
                    seeded.Results = JsonConvert.DeserializeObject<Dictionary<string, ResultRec>>(row.Value.ResultsJson)
                                     ?? new Dictionary<string, ResultRec>();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[BossStore] load FAIL viewer={viewerId}: {ex.Message}");
            }
            if (seeded.Hp <= 0) seeded.Hp = MaxHp();
            seeded.Decks = seeded.Decks.Take(MaxDecks).ToList();
            Cache[viewerId] = seeded;
            return seeded;
        }

        private static void Persist(int viewerId, BossState st)
        {
            try
            {
                AccountStore.SaveBossState(viewerId, st.Hp, st.NextDeckId,
                    JsonConvert.SerializeObject(st.Decks),
                    JsonConvert.SerializeObject(st.Results));
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[BossStore] save FAIL viewer={viewerId}: {ex.Message}");
            }
        }

        public static int RemainHp(int viewerId)
        {
            lock (Gate) return Ensure(viewerId).Hp;
        }

        public static IReadOnlyList<DeckRec> DeckList(int viewerId)
        {
            lock (Gate) return Ensure(viewerId).Decks.ToArray();
        }

        public static ResultRec GetResult(int viewerId, string battleId)
        {
            lock (Gate)
            {
                return Ensure(viewerId).Results.TryGetValue(battleId, out var r) ? r : null;
            }
        }

        public static void SaveDeck(int viewerId, DeckRec deck)
        {
            if (deck == null || deck.Units == null || deck.Units.Count == 0) return;
            lock (Gate)
            {
                var st = Ensure(viewerId);
                deck.Id = st.NextDeckId++;
                deck.Damage = 0;
                deck.Utc = DateTime.UtcNow.ToString("o");
                st.Decks.Insert(0, deck);
                if (st.Decks.Count > MaxDecks) st.Decks = st.Decks.Take(MaxDecks).ToList();
                Persist(viewerId, st);
                Console.WriteLine($"[BossStore] saveDeck viewer={viewerId} deckId={deck.Id} units={deck.Units.Count} rank={deck.Rank}");
            }
        }

        public static void UpdateDeckLineup(int viewerId, int deckId, List<DeckUnitRec> units, List<SynRec> roles, List<SynRec> teams)
        {
            if (units == null || units.Count == 0) return;
            lock (Gate)
            {
                var st = Ensure(viewerId);
                var deck = st.Decks.FirstOrDefault(d => d.Id == deckId);
                if (deck == null) return;
                deck.Units = units;
                deck.Roles = roles ?? new List<SynRec>();
                deck.Teams = teams ?? new List<SynRec>();
                deck.Utc = DateTime.UtcNow.ToString("o");
                Persist(viewerId, st);
                Console.WriteLine($"[BossStore] updateDeckLineup viewer={viewerId} deckId={deckId} units={units.Count}");
            }
        }

        public static ResultRec RecordResult(int viewerId, string battleId, int deckId, long damage)
        {
            lock (Gate)
            {
                var st = Ensure(viewerId);
                var max = MaxHp();
                var dmg = (int)Math.Max(0, Math.Min((long)st.Hp, damage));
                st.Hp -= dmg;
                var defeated = st.Hp <= 0;
                var rec = new ResultRec
                {
                    Max = max,
                    Remain = Math.Max(0, st.Hp),
                    Dmg = dmg,
                    Defeated = defeated,
                    Utc = DateTime.UtcNow.ToString("o"),
                };
                st.Results[battleId] = rec;
                while (st.Results.Count > MaxResults)
                {
                    var oldest = st.Results.OrderBy(kv => kv.Value.Utc).First().Key;
                    st.Results.Remove(oldest);
                }
                var deck = st.Decks.FirstOrDefault(d => d.Id == deckId);
                if (deck != null) deck.Damage += dmg;
                if (defeated)
                {
                    st.Hp = max;

                    if (AccountStore.TutorialStepOf(viewerId) == 14)
                        AccountStore.TryAdvanceTutorialStep(viewerId, out _);
                }
                Persist(viewerId, st);
                Console.WriteLine($"[BossStore] recordResult viewer={viewerId} battle={battleId} deck={deckId} dmg={dmg} remain={rec.Remain} defeated={defeated}");
                return rec;
            }
        }
    }
}
