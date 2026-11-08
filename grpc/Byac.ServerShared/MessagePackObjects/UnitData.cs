using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class UnitData
    {

		public enum ROLE_TYPE
		{
			NONE,
			WARRIOR = 101,
			HUNTER_OLD,
			BLADEMASTER,
			MAGE,
			KNIGHT,
			ASSASIN,
			LANCER,
			BATTLEMAGE,
			PRIEST,
			MYSTIC,
			DRAGON,
			MIRROR,
			HUNTER,
			INVINCIBLE = 999
		}

		public enum TEAM_TYPE
		{
			NONE,
			Team201 = 201,
			Team202,
			Team203,
			Team204,
			Team205,
			Team206,
			Team207,
			Team208,
			Team209,
			Team210,
			Team211,
			Team212,
			Team213,
			Team214,
			Team215,
			Team216
		}

	}
}
