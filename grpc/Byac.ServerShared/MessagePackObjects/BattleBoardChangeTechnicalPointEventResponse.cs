using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardChangeTechnicalPointEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int TechnicalPoint { get; set; }

		[Key(4)]
		public int Tp { get; set; }

		[Key(5)]
		public bool IsSynergy { get; set; }

		[Key(6)]
		public int PreviousTp { get; set; }

		public BattleBoardChangeTechnicalPointEventResponse() { }
		public BattleBoardChangeTechnicalPointEventResponse(int matchId, float raisedAt, int uniqueKey, int technicalPoint, int tp, bool isSynergy, int previousTp)
		{
		}

	}
}
