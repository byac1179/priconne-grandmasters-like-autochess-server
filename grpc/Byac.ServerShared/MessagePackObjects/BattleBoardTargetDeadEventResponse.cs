using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardTargetDeadEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		public BattleBoardTargetDeadEventResponse() { }
		public BattleBoardTargetDeadEventResponse(int matchId, float raisedAt, int uniqueKey)
		{
		}
	}
}
