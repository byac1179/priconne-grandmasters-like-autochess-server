using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardStunEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int OperatorUniqueKey { get; set; }

		[Key(3)]
		public int TargetUniqueKey { get; set; }

		[Key(4)]
		public float Duration { get; set; }

		[Key(5)]
		public bool HideTargetUnit { get; set; }

		[Key(6)]
		public int SkillActionId { get; set; }

		public BattleBoardStunEventResponse()
        {
        }

		public BattleBoardStunEventResponse(int matchId, float raisedAt, int operatorUniqueKey, int targetUniqueKey, float duration, bool hideTargetUnit, int skillActionId)
		{
		}

	}
}
