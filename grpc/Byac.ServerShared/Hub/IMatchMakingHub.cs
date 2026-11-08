using MagicOnion;
using Byac.ServerShared.MessagePackObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Byac.ServerShared.Hub
{
    public interface IMatchMakingHub : IStreamingHub<IMatchMakingHub, IMatchMakingHubReceiver>, IStreamingHubMarker, IServiceMarker
    {
        Task EntryAsync(MatchMakingEntryRequest request);

        Task CancelAsync(MatchMakingCancelRequest request);
    }
}
