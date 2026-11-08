using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardBoxDropEventResponse : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public NpcBoxDropResultMpo[] Boxes { get; set; }

		public BattleBoardBoxDropEventResponse() { }

		public BattleBoardBoxDropEventResponse(int matchId, float raisedAt, int uniqueKey, NpcBoxDropResultMpo[] boxes)
		{
			MatchId = matchId;
			RaisedAt = raisedAt;
			UniqueKey = uniqueKey;
			Boxes = boxes;
		}

	}
}
