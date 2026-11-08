using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Byac.ServerShared.Hub
{
    public interface IRoomHub : IStreamingHub<IRoomHub, IRoomHubReceiver>, IStreamingHubMarker, IServiceMarker
    {

    }
}
