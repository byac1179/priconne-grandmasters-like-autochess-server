using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class UnitPickupItemDataMpo : PickupItemDataMpoBase
    {
        [Key(0)]
        public int UniqueKey { get; set; }

        [Key(1)]
        public int UnitId { get; set; }

    }
}
