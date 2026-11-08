using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardSendFieldUnitEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int? SendPlayerId { get; set; }

		[Key(3)]
		public int[] SendUnitUniqueKeys { get; set; }

		public BattleBoardSendFieldUnitEventResponse() { }
		public BattleBoardSendFieldUnitEventResponse(int matchId, float raisedAt, int? sendId, int[] sendUnitUniqueKeys)
		{
		}

	}
}
