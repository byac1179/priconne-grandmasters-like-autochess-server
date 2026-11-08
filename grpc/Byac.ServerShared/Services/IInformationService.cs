using MagicOnion;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Byac.ServerShared.Services
{

    public interface IInformationService : IService<IInformationService>, IServiceMarker
    {
        UnaryResult<ServiceResult<InformationListResponse>> GetInformationListAsync();
    }
}
