using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardResetSkillRecastTimeEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		public BattleBoardResetSkillRecastTimeEventResponse() { }
		public BattleBoardResetSkillRecastTimeEventResponse(int matchId, float raisedAt, int uniqueKey)
		{
		}
	}
}
