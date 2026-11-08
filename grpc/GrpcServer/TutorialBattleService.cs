using System;
using System.Collections.Generic;
using MagicOnion;
using MagicOnion.Server;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.Services;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class TutorialBattleService : ServiceBase<ITutorialBattleService>, ITutorialBattleService
    {
        private const int TutorialPlayerLife = 40;

        public async UnaryResult<BattleBoardEventResponse[]> GetBattleResponseAsync(TutorialBattleRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Console.WriteLine($"[TUTBAT] ENTER match={request.MatchId} p={request.PrimaryUnits?.Length} s={request.SecondaryUnits?.Length}");
            var primary = BuildSide(request.PrimaryUnits, isNpc: false);
            var secondary = BuildSide(request.SecondaryUnits, isNpc: true);

            var sim = SimpleTickSimulator.Simulate(
                request.MatchId, primary.Units, primary.Status, secondary.Units, secondary.Status);

            SimpleTickSimulator.DiscretizeAxis(sim.Events);
            SimpleTickSimulator.EnforceFrameRevisions(sim.Events);

            var events = new List<BattleBoardEventResponse>(sim.Events.Count + 1);
            foreach (var e in sim.Events)
                events.Add(e.ev);

            float lastAt = sim.Events.Count > 0 ? sim.Events[sim.Events.Count - 1].at : 0f;
            float completeAt = sim.TimedOut
                ? Math.Max(SimpleTickSimulator.MaxSeconds, lastAt + 0.05f)
                : lastAt + 0.05f;

            bool primaryAlive = sim.PrimaryAlive > 0;
            bool secondaryAlive = sim.SecondaryAlive > 0;

            BattleResultType primaryResult, secondaryResult;
            if (primaryAlive && !secondaryAlive)
            {
                primaryResult = BattleResultType.Win;
                secondaryResult = BattleResultType.Lose;
            }
            else if (secondaryAlive && !primaryAlive)
            {
                primaryResult = BattleResultType.Lose;
                secondaryResult = BattleResultType.Win;
            }
            else if (sim.IsDraw || (!primaryAlive && !secondaryAlive))
            {
                primaryResult = BattleResultType.Draw;
                secondaryResult = BattleResultType.Draw;
            }
            else
            {
                primaryResult = sim.PrimaryWin ? BattleResultType.Win : BattleResultType.Lose;
                secondaryResult = sim.PrimaryWin ? BattleResultType.Lose : BattleResultType.Win;
            }

            events.Add(new BattleBoardCompleteEventResponse
            {
                MatchId = request.MatchId,
                RaisedAt = completeAt,
                LastMatchInRound = true,
                PrimaryResult = primaryResult,
                PrimaryDamage = primaryAlive ? 0 : TutorialPlayerLife,
                PrimaryLife = primaryAlive ? TutorialPlayerLife : 0,
                SecondaryResult = secondaryResult,
                SecondaryDamage = secondaryAlive ? 0 : TutorialPlayerLife,
                SecondaryLife = secondaryAlive ? TutorialPlayerLife : 0,
                TimedOut = sim.TimedOut,
            });

            Console.WriteLine($"[TUTBAT] match={request.MatchId} p={primary.Units.Length} s={secondary.Units.Length} events={events.Count} primary={primaryResult} secondary={secondaryResult} timedOut={sim.TimedOut} completeAt={completeAt:F2}");
            return events.ToArray();
        }

        private static (BattleUnitDataMpo[] Units, BattleUnitCalculatedStatusSetMpo Status) BuildSide(
            TutorialBattleRequest.Unit[] source, bool isNpc)
        {
            var units = new List<BattleUnitDataMpo>();
            int equipmentKey = 0;
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    var r = source[i];
                    var unit = BattleMpoFactory.MakeUnit(r.UniqueKey, r.UnitId, r.X, r.Y, UnitState.Field, i, isNpc);
                    unit.Rank = r.Rank < 1 ? 1 : r.Rank;
                    if (r.Equipments != null)
                    {
                        foreach (var equipmentId in r.Equipments)
                            unit.EquipmentList.Add(BattleMpoFactory.MakeEquipment(equipmentKey++, equipmentId));
                    }
                    units.Add(unit);
                }
            }
            var array = units.ToArray();
            return (array, BattleMpoFactory.MakeStatusSetForUnits(array));
        }
    }
}
