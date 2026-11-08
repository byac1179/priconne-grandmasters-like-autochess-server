using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardSkillTPDamageEventResponse : BattleBoardTPDamageEventResponse
	{

        [Key(6)]
		public int SkillActionId { get; set; }

		public BattleBoardSkillTPDamageEventResponse() { }
		public BattleBoardSkillTPDamageEventResponse(int matchId, float raisedAt, int operatorUnitqueKey, int targetUniqueKey, int damage, int technicalPoint) : base(matchId, raisedAt, operatorUnitqueKey, targetUniqueKey, damage, technicalPoint)
		{
		}

	}
}
