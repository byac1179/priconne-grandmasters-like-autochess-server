using System;
using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardStatusEffectRemovedEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int SkillActionId { get; set; }

		[Key(4)]
		public StatusEffectVfxType StatusEffectVfxType { get; set; }

		[Key(5)]
		public int SynergyId { get; set; }

		public BattleBoardStatusEffectRemovedEventResponse()
        {

        }

		public BattleBoardStatusEffectRemovedEventResponse(int matchId, float raisedAt, int uniqueKey, int skillActionId, StatusEffectVfxType statusEffectVfxType)
		{
		}
	}
}
