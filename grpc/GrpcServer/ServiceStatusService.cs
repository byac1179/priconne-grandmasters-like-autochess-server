using MagicOnion;
using MagicOnion.Server;
using MessagePack;
using Byac.ServerShared.Services;
using Byac.Services;
using System;

namespace Byac.Server
{
    public class ServiceStatusService : ServiceBase<IServiceStatusService>, IServiceStatusService
    {
        public async UnaryResult<Nil> SendPreflightAsync()
        {
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[REQH] {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            return new Nil();
        }
    }
}
