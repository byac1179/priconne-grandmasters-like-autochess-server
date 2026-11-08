using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardTeleportEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public int X { get; set; }

		[Key(4)]
		public int Y { get; set; }

		[Key(5)]
		public int PreviousX { get; set; }

		[Key(6)]
		public int PreviousY { get; set; }

		public BattleBoardTeleportEventResponse() { }
		public BattleBoardTeleportEventResponse(int matchId, float raisedAt, int uniqueKey, int x, int y, int previousX, int previousY)
		{
		}

	}
}
