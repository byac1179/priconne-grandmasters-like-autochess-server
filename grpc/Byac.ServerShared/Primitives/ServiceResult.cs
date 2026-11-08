using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.ServerShared.Primitives
{
    [MessagePackObject]
    public class ServiceResult<T, TError> : IServiceResult
    {

        [Key(0)]
        public T Result { get; set; }

        [Key(1)]
        public bool Succeeded { get; set; }

        [Key(2)]
        public TError ErrorDetail { get; set; }

    }

    [MessagePackObject]
    public sealed class ServiceResult<T> : ServiceResult<T, GeneralError>
    {

    }

    [MessagePackObject]
    public struct ServiceResultVoid : IEquatable<ServiceResultVoid>
    {
        public bool Equals(ServiceResultVoid other) { return true; }
        public override bool Equals(object obj) { return obj is ServiceResultVoid; }
        public override int GetHashCode() { return 0; }
    }

    [MessagePackObject]
    public sealed class ServiceResult : ServiceResult<ServiceResultVoid, GeneralError>
    {
        public static ServiceResult Success()
        {
            return new ServiceResult
            {
                Succeeded = true,
                Result = new ServiceResultVoid(),
                ErrorDetail = new GeneralError()
            };
        }

        public static ServiceResult Fail(string reason, GeneralErrorCode code = GeneralErrorCode.Unknown)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Result = new ServiceResultVoid(),
                ErrorDetail = new GeneralError { Code = code, Reason = reason }
            };
        }
    }
}
