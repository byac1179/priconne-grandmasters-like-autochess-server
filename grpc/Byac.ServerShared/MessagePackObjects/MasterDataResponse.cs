using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Byac.ServerShared.MessagePack.Formatters;

namespace Byac.ServerShared.MessagePackObjects.User
{
    [MessagePackObject]
    public struct MasterDataResponse
    {
        [Key(0)]
        public byte[] Data { get; set; }

        [Key(1)]
        public string Hash { get; set; }
    }

}
