using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.Enums
{
	public enum BattlePhase
	{
		Invalid,
		Initialized,
		SetupPlayerInfo,
		Preparing,
		Prepared,
		Running,
		Result,
		PreparePick,
		Pick,
		PrepareOverallBuffPick,
		OverallBuffPick,
		GuildBuffRoulette,
		Transitioning,
		Complete,
		Reconnect
	}
}
