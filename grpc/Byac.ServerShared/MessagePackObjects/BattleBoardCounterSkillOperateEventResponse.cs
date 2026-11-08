using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardCounterSkillOperateEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int SkillActionId { get; set; }

		[Key(4)]
		public bool IsEnd { get; set; }

		[Key(5)]
		public bool IsCancel { get; set; }

		public BattleBoardCounterSkillOperateEventResponse() { }
		public BattleBoardCounterSkillOperateEventResponse(int matchId, float raisedAt, int uniqueKey, int skillActionId, bool isEnd, bool isCancel)
		{
		}

	}
}
