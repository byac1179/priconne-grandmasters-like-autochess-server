using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardResetRecastTimeEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		public BattleBoardResetRecastTimeEventResponse() { }
		public BattleBoardResetRecastTimeEventResponse(int matchId, float raisedAt, int uniqueKey)
		{
		}

	}
}
