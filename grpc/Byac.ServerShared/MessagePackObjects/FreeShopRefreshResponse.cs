using MessagePack;
using Byac.ServerShared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class FreeShopRefreshResponse
    {
        [Key(0)]
        public int ViewerId { get; set; }

        [Key(1)]
        public BattleOption InvokerOption { get; set; }
    }
}
