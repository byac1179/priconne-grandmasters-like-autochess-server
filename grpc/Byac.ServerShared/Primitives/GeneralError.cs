using Byac.ServerShared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Byac.ServerShared.Primitives
{
    [MessagePackObject]
    public struct GeneralError
    {
        [Key(0)]
        public GeneralErrorCode Code { get; set; }

        [Key(1)]
        public string Reason { get; set; }

    }
}
