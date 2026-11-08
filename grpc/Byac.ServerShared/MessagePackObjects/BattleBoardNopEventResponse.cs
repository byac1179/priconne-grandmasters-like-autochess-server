using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardNopEventResponse : BattleBoardEventResponse
	{
		public BattleBoardNopEventResponse() { }
		public BattleBoardNopEventResponse(int matchId, float raisedAt)
		{
		}
	}
}
