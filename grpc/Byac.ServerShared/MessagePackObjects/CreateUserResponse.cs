using MessagePack;
using Byac.ServerShared.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.MessagePackObjects.User
{
    [MessagePackObject]
    public class CreateUserResponse
    {
        [Key(1)]
        public string AuthenticationToken { get; set; }

        [Key(2)]
        public int ViewerId { get; set; }

        [Key(3)]
        public byte[] DeviceIdentifier { get; set; }

        [Key(4)]
        public string Name { get; set; }
    }
}
