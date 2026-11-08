using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardEmptySkillOperateEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int SkillActionId { get; set; }

		public BattleBoardEmptySkillOperateEventResponse() { }
		public BattleBoardEmptySkillOperateEventResponse(int matchId, float raisedAt, int uniqueKey, int skillActionId)
		{
		}

	}
}
