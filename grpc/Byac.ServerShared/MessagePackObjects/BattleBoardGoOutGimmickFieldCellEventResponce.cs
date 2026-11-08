using System;
using MessagePack;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public class BattleBoardGoOutGimmickFieldCellEventResponce : BattleBoardEventResponse
	{
		[Key(2)]
		public int UniqueKey { get; set; }

		[Key(3)]
		public FieldCellDataMpo FieldCellDataMpo { get; set; }

		public BattleBoardGoOutGimmickFieldCellEventResponce() { }
		public BattleBoardGoOutGimmickFieldCellEventResponce(int matchId, float raisedAt, int uniqueKey, FieldCellDataMpo fieldCellDataMpo)
		{
		}

	}
}
