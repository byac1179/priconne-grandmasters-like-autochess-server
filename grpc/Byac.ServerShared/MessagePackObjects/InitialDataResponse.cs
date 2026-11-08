using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Byac.ServerShared.MessagePack.Formatters;

namespace Byac.ServerShared.MessagePackObjects.User
{

    [MessagePackObject]
    public class InitialDataResponse
    {

        [Key(0)]
        public UserResponse User { get; set; }

        [Key(1)]
        public int PrizeFeature { get; set; }

        public InitialDataResponse()
        {
        }

    }
}
