using MagicOnion;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byac.ServerShared.Services
{
    public interface IServiceStatusService : IService<IServiceStatusService>, IServiceMarker
    {
        UnaryResult<Nil> SendPreflightAsync();
    }
}
