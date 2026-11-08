using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.Enums
{
	public enum BattleOption
	{
		None = 0,
		RoundReloadFree = 1,
		DefeatReloadFree = 2,
		DropBoost = 4,
		GimmickCells = 8,
		SkipInitialUnits = 16,
		GuildMode = 32,
		All = 63
	}
}
