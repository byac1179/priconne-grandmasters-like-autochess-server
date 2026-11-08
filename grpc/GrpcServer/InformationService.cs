using MagicOnion;
using MagicOnion.Server;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.Primitives;
using Byac.ServerShared.Services;
using System;
using System.Collections.Generic;

namespace Byac.Services
{
    public class InformationService : ServiceBase<IInformationService>, IInformationService
    {
        public async UnaryResult<ServiceResult<InformationListResponse>> GetInformationListAsync()
        {
            Console.WriteLine("[Info] GetInformationList");
            var result = new ServiceResult<InformationListResponse>();
            result.Succeeded = true;
            result.Result = new InformationListResponse { Informations = InformationStore.Build() };
            result.ErrorDetail = new GeneralError();
            return result;
        }
    }

    internal static class InformationStore
    {
        public static IReadOnlyList<InformationListItemMpo> Build()
        {
            return Array.Empty<InformationListItemMpo>();
        }
    }
}
