using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardDebugMessageEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public string Message { get; set; }

		public BattleBoardDebugMessageEventResponse() { }
		public BattleBoardDebugMessageEventResponse(int matchId, float raisedAt, string message)
		{
		}

	}
}
