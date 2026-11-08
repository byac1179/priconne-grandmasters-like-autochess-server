using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class BossBattleDeckListResponse
    {
        [Key(0)]
        public BossBattleDeckDataMpo[] Decks { get; set; }

    }
}
