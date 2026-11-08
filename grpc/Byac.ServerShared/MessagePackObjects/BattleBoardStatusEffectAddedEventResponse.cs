using System;
using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardStatusEffectAddedEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int TargetUniqueKey { get; set; }

		[Key(4)]
		public int SkillActionId { get; set; }

		[Key(5)]
		public StatusEffectVfxType StatusEffectVfxType { get; set; }

		[Key(6)]
		public int SynergyId { get; set; }

		public BattleBoardStatusEffectAddedEventResponse()
        {

        }

		public BattleBoardStatusEffectAddedEventResponse(int matchId, float raisedAt, int uniqueKey, int targetUniqueKey, int skillActionId, StatusEffectVfxType statusEffectVfxType)
		{
		}

	}
}
