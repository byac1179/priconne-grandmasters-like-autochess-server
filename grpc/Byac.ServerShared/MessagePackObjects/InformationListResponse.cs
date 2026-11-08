using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class InformationListResponse
    {
        [Key(0)]
        public IReadOnlyList<InformationListItemMpo> Informations { get; set; }

    }
}
