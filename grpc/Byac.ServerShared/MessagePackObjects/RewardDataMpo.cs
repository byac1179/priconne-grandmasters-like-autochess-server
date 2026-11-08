using MessagePack;
using Byac.ServerShared.Enums;
using System;

namespace Byac.ServerShared.MessagePackObjects
{
	[MessagePackObject]
	public struct RewardDataMpo
	{
		[Key(0)]
		public BattleResultType ResultType { get; set; }

		[Key(1)]
		public int BaseGold { get; set; }

		[Key(2)]
		public int DistributeGold { get; set; }

		[Key(3)]
		public int BonusGold { get; set; }

		[Key(4)]
		public int ResultRewardGold { get; set; }

		[Key(5)]
		public int InterestGold { get; set; }

		[Key(6)]
		public BattleUnitDataMpo[] Units { get; set; }

		[Key(7)]
		public BattleEquipmentDataMpo[] Equipments { get; set; }

		public RewardDataMpo(BattleResultType resultType, int baseGold, int distributeGold, int bonusGold, int resultRewardGold, int interestGold, BattleUnitDataMpo[] units, BattleEquipmentDataMpo[] equipments)
		{
			ResultType = resultType;
			BaseGold = baseGold;
			DistributeGold = distributeGold;
			BonusGold = bonusGold;
			ResultRewardGold = resultRewardGold;
			InterestGold = interestGold;
			Units = units;
			Equipments = equipments;
		}
	}
}
