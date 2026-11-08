using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardTPDamageEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int OperatorUniqueKey { get; set; }

		[Key(3)]
		public int TargetUniqueKey { get; set; }

		[Key(4)]
		public int Damage { get; set; }

		[Key(5)]
		public int TechnicalPoint { get; set; }

		public BattleBoardTPDamageEventResponse() { }

		public BattleBoardTPDamageEventResponse(int matchId, float raisedAt, int operatorUnitqueKey, int targetUniqueKey, int damage, int technicalPoint)
		{
		}

	}
}
