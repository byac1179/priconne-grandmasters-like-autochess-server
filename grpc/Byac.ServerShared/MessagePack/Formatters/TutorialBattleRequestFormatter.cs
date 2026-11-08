using System;
using MessagePack;
using MessagePack.Formatters;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.ServerShared.MessagePack.Formatters
{

    public sealed class TutorialBattleRequestFormatter : IMessagePackFormatter<TutorialBattleRequest>
    {
        public void Serialize(ref MessagePackWriter writer, TutorialBattleRequest value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            var unitFormatter = r.GetFormatter<TutorialBattleRequest.Unit>();
            writer.WriteMapHeader(5);
            writer.Write("MatchId");
            writer.Write(value.MatchId);
            writer.Write("PrimaryViewerId");
            writer.Write(value.PrimaryViewerId);
            writer.Write("SecondaryViewerId");
            writer.Write(value.SecondaryViewerId);
            writer.Write("PrimaryUnits");
            WriteUnits(ref writer, value.PrimaryUnits, unitFormatter, options);
            writer.Write("SecondaryUnits");
            WriteUnits(ref writer, value.SecondaryUnits, unitFormatter, options);
        }

        private static void WriteUnits(ref MessagePackWriter writer, TutorialBattleRequest.Unit[] units,
            IMessagePackFormatter<TutorialBattleRequest.Unit> formatter, MessagePackSerializerOptions options)
        {
            if (units == null) { writer.WriteNil(); return; }
            writer.WriteArrayHeader(units.Length);
            for (int i = 0; i < units.Length; i++)
                formatter.Serialize(ref writer, units[i], options);
        }

        public TutorialBattleRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var unitFormatter = options.Resolver.GetFormatter<TutorialBattleRequest.Unit>();
            int count = reader.ReadMapHeader();
            var v = new TutorialBattleRequest();
            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                switch (key)
                {
                    case "MatchId": v.MatchId = reader.ReadInt32(); break;
                    case "PrimaryViewerId": v.PrimaryViewerId = reader.ReadInt32(); break;
                    case "SecondaryViewerId": v.SecondaryViewerId = reader.ReadInt32(); break;
                    case "PrimaryUnits": v.PrimaryUnits = ReadUnits(ref reader, unitFormatter, options); break;
                    case "SecondaryUnits": v.SecondaryUnits = ReadUnits(ref reader, unitFormatter, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }

        private static TutorialBattleRequest.Unit[] ReadUnits(ref MessagePackReader reader,
            IMessagePackFormatter<TutorialBattleRequest.Unit> formatter, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return Array.Empty<TutorialBattleRequest.Unit>();
            int count = reader.ReadArrayHeader();
            var units = new TutorialBattleRequest.Unit[count];
            for (int i = 0; i < count; i++)
                units[i] = formatter.Deserialize(ref reader, options);
            return units;
        }
    }

    public sealed class TutorialBattleUnitFormatter : IMessagePackFormatter<TutorialBattleRequest.Unit>
    {
        public void Serialize(ref MessagePackWriter writer, TutorialBattleRequest.Unit value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            writer.WriteMapHeader(6);
            writer.Write("UnitId");
            writer.Write(value.UnitId);
            writer.Write("UniqueKey");
            writer.Write(value.UniqueKey);
            writer.Write("X");
            writer.Write(value.X);
            writer.Write("Y");
            writer.Write(value.Y);
            writer.Write("Rank");
            writer.Write(value.Rank);
            writer.Write("Equipments");
            if (value.Equipments == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(value.Equipments.Length);
                for (int i = 0; i < value.Equipments.Length; i++)
                    writer.Write(value.Equipments[i]);
            }
        }

        public TutorialBattleRequest.Unit Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            int count = reader.ReadMapHeader();
            var v = new TutorialBattleRequest.Unit { Equipments = Array.Empty<int>() };
            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                switch (key)
                {
                    case "UnitId": v.UnitId = reader.ReadInt32(); break;
                    case "UniqueKey": v.UniqueKey = reader.ReadInt32(); break;
                    case "X": v.X = reader.ReadInt32(); break;
                    case "Y": v.Y = reader.ReadInt32(); break;
                    case "Rank": v.Rank = reader.ReadInt32(); break;
                    case "Equipments":
                        if (reader.TryReadNil()) { v.Equipments = Array.Empty<int>(); break; }
                        int n = reader.ReadArrayHeader();
                        var equipments = new int[n];
                        for (int j = 0; j < n; j++)
                            equipments[j] = reader.ReadInt32();
                        v.Equipments = equipments;
                        break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }
}
