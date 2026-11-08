using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Byac.ServerShared.Enums;
using Byac.ServerShared.MessagePackObjects;
using Byac.ServerShared.MessagePackObjects.User;
using Byac.ServerShared.Primitives;

namespace Byac.ServerShared.MessagePack.Formatters
{

    public sealed class DeviceStaticResolver : IFormatterResolver
    {
        public static readonly DeviceStaticResolver Instance = new DeviceStaticResolver();

        static readonly Dictionary<Type, object> Map = new Dictionary<Type, object>();

        static void Reg<T>(IMessagePackFormatter<T> f) => Map[typeof(T)] = f;

        static DeviceStaticResolver()
        {

            Reg<ServiceResult<MasterDataResponse, MasterCacheResult>>(new ServiceResultOfTwoFormatter<MasterDataResponse, MasterCacheResult>());
            Reg<MasterDataResponse>(new MasterDataResponseStaticFormatter());

            Reg<ServiceResult<CreateUserResponse>>(new ServiceResultOfOneFormatter<CreateUserResponse>());
            Reg<CreateUserResponse>(new CreateUserResponseStaticFormatter());
            Reg<GeneralError>(new GeneralErrorStaticFormatter());
            Reg<ServiceResult<GetAccessTokenResponse, GetAccessTokenResult>>(new ServiceResultOfTwoFormatter<GetAccessTokenResponse, GetAccessTokenResult>());
            Reg<GetAccessTokenResponse>(new GetAccessTokenResponseStaticFormatter());
            Reg<ServiceResult<InitialDataResponse>>(new ServiceResultOfOneFormatter<InitialDataResponse>());
            Reg<InitialDataResponse>(new InitialDataResponseStaticFormatter());
            Reg<UserResponse>(new UserResponseStaticFormatter());
            Reg<Ulid>(new UlidMessagePackFormatter());

            Reg<TutorialBattleRequest>(new TutorialBattleRequestFormatter());
            Reg<TutorialBattleRequest.Unit>(new TutorialBattleUnitFormatter());
            Reg<BattleBoardEventResponse[]>(new ArrayFormatter<BattleBoardEventResponse>());
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (Map.TryGetValue(typeof(T), out var f))
                return (IMessagePackFormatter<T>)f;

            var generated = GeneratedResolver.Instance.GetFormatter<T>();
            if (generated != null)
                return generated;

            var serviceResult = ServiceResultBridge<T>.Formatter;
            if (serviceResult != null)
                return serviceResult;
            var argumentTuple = DynamicArgumentTupleBridge<T>.Formatter;
            if (argumentTuple != null)
                return argumentTuple;
            if (typeof(T).IsEnum)
                return EnumCache<T>.Formatter;
            var builtin = BuiltinResolver.Instance.GetFormatter<T>();
            if (builtin != null)
                return builtin;
            var collection = CollectionBridge<T>.Formatter;
            if (collection != null)
                return collection;
            throw new MessagePackSerializationException(
                "DeviceStaticResolver: no static formatter for " + typeof(T).FullName +
                " (add to DeviceStaticResolver.Reg) - refusing dynamic/Emit path (device deadlock)");
        }

        static class DynamicArgumentTupleBridge<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter = Create();

            static IMessagePackFormatter<T> Create()
            {
                try
                {
                    var t = typeof(T);
                    if (!t.IsGenericType || t.Namespace != "MagicOnion"
                            || !t.Name.StartsWith("DynamicArgumentTuple`", StringComparison.Ordinal))
                        return null;
                    var args = t.GetGenericArguments();
                    var definition = t.Assembly.GetType(
                        "MagicOnion.DynamicArgumentTupleFormatter`" + args.Length);
                    if (definition == null)
                        return null;
                    var defaults = args.Select(static a => a.IsValueType
                        ? Activator.CreateInstance(a) : null).ToArray();
                    return (IMessagePackFormatter<T>)Activator.CreateInstance(
                        definition.MakeGenericType(args), defaults);
                }
                catch
                {
                    return null;
                }
            }
        }

        static class CollectionBridge<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter = Create();

            static IMessagePackFormatter<T> Create()
            {
                try
                {
                    var t = typeof(T);
                    Type open;
                    Type[] args;
                    if (t.IsArray && t.GetArrayRank() == 1)
                    {
                        open = typeof(global::MessagePack.Formatters.ArrayFormatter<>);
                        args = new[] { t.GetElementType() };
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        open = typeof(global::MessagePack.Formatters.ListFormatter<>);
                        args = t.GetGenericArguments();
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        open = typeof(global::MessagePack.Formatters.DictionaryFormatter<,>);
                        args = t.GetGenericArguments();
                    }
                    else return null;
                    return (IMessagePackFormatter<T>)Activator.CreateInstance(
                        open.MakeGenericType(args));
                }
                catch
                {
                    return null;
                }
            }
        }

        static class ServiceResultBridge<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter = Create();

            static IMessagePackFormatter<T> Create()
            {
                if (!typeof(T).IsGenericType)
                    return null;
                var args = typeof(T).GetGenericArguments();
                Type? fmtType = null;
                var def = typeof(T).GetGenericTypeDefinition();
                if (def == typeof(ServiceResult<>))
                    fmtType = typeof(global::MessagePack.Formatters.Byac.ServerShared.Primitives.ServiceResultFormatter<>).MakeGenericType(args);
                else if (def == typeof(ServiceResult<,>))
                    fmtType = typeof(global::MessagePack.Formatters.Byac.ServerShared.Primitives.ServiceResultFormatter<,>).MakeGenericType(args);
                return fmtType == null ? null : (IMessagePackFormatter<T>)Activator.CreateInstance(fmtType);
            }
        }

        static class EnumCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter =
                (IMessagePackFormatter<T>)(object)new EnumIntFormatter<T>();
        }

        sealed class EnumIntFormatter<T> : IMessagePackFormatter<T>
        {
            public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
                => writer.Write(unchecked(Convert.ToInt32(value)));

            public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
                => (T)Enum.ToObject(typeof(T), reader.ReadInt32());
        }
    }

    public sealed class ServiceResultOfTwoFormatter<T, TError> : IMessagePackFormatter<ServiceResult<T, TError>>
    {
        public void Serialize(ref MessagePackWriter writer, ServiceResult<T, TError> value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(3);
            r.GetFormatter<T>().Serialize(ref writer, value.Result, options);
            writer.Write(value.Succeeded);
            r.GetFormatter<TError>().Serialize(ref writer, value.ErrorDetail, options);
        }

        public ServiceResult<T, TError> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new ServiceResult<T, TError>();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.Result = r.GetFormatter<T>().Deserialize(ref reader, options); break;
                    case 1: v.Succeeded = reader.ReadBoolean(); break;
                    case 2: v.ErrorDetail = r.GetFormatter<TError>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class ServiceResultOfOneFormatter<T> : IMessagePackFormatter<ServiceResult<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ServiceResult<T> value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(3);
            r.GetFormatter<T>().Serialize(ref writer, value.Result, options);
            writer.Write(value.Succeeded);
            r.GetFormatter<GeneralError>().Serialize(ref writer, value.ErrorDetail, options);
        }

        public ServiceResult<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new ServiceResult<T>();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.Result = r.GetFormatter<T>().Deserialize(ref reader, options); break;
                    case 1: v.Succeeded = reader.ReadBoolean(); break;
                    case 2: v.ErrorDetail = r.GetFormatter<GeneralError>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class GeneralErrorStaticFormatter : IMessagePackFormatter<GeneralError>
    {
        public void Serialize(ref MessagePackWriter writer, GeneralError value, MessagePackSerializerOptions options)
        {
            var r = options.Resolver;
            writer.WriteArrayHeader(2);
            r.GetFormatter<GeneralErrorCode>().Serialize(ref writer, value.Code, options);
            r.GetFormatter<string>().Serialize(ref writer, value.Reason, options);
        }

        public GeneralError Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return default;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = default(GeneralError);
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.Code = r.GetFormatter<GeneralErrorCode>().Deserialize(ref reader, options); break;
                    case 1: v.Reason = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class CreateUserResponseStaticFormatter : IMessagePackFormatter<CreateUserResponse>
    {
        public void Serialize(ref MessagePackWriter writer, CreateUserResponse value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.WriteNil();
            r.GetFormatter<string>().Serialize(ref writer, value.AuthenticationToken, options);
            r.GetFormatter<int>().Serialize(ref writer, value.ViewerId, options);
            r.GetFormatter<byte[]>().Serialize(ref writer, value.DeviceIdentifier, options);
            r.GetFormatter<string>().Serialize(ref writer, value.Name, options);
        }

        public CreateUserResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new CreateUserResponse();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: reader.Skip(); break;
                    case 1: v.AuthenticationToken = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    case 2: v.ViewerId = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 3: v.DeviceIdentifier = r.GetFormatter<byte[]>().Deserialize(ref reader, options); break;
                    case 4: v.Name = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class GetAccessTokenResponseStaticFormatter : IMessagePackFormatter<GetAccessTokenResponse>
    {
        public void Serialize(ref MessagePackWriter writer, GetAccessTokenResponse value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(5);
            r.GetFormatter<GetAccessTokenResult>().Serialize(ref writer, value.Result, options);
            r.GetFormatter<string>().Serialize(ref writer, value.AccessToken, options);
            r.GetFormatter<DateTimeOffset>().Serialize(ref writer, value.Expiration, options);
            r.GetFormatter<int>().Serialize(ref writer, value.ViewerId, options);
            r.GetFormatter<byte[]>().Serialize(ref writer, value.DeviceIdentifier, options);
        }

        public GetAccessTokenResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new GetAccessTokenResponse();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: reader.Skip(); break;
                    case 1: v.AccessToken = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    case 2: v.Expiration = r.GetFormatter<DateTimeOffset>().Deserialize(ref reader, options); break;
                    case 3: v.ViewerId = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 4: v.DeviceIdentifier = r.GetFormatter<byte[]>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class InitialDataResponseStaticFormatter : IMessagePackFormatter<InitialDataResponse>
    {
        public void Serialize(ref MessagePackWriter writer, InitialDataResponse value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(2);
            r.GetFormatter<UserResponse>().Serialize(ref writer, value.User, options);
            r.GetFormatter<int>().Serialize(ref writer, value.PrizeFeature, options);
        }

        public InitialDataResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new InitialDataResponse();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.User = r.GetFormatter<UserResponse>().Deserialize(ref reader, options); break;
                    case 1: v.PrizeFeature = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class UserResponseStaticFormatter : IMessagePackFormatter<UserResponse>
    {
        public void Serialize(ref MessagePackWriter writer, UserResponse value, MessagePackSerializerOptions options)
        {
            if (value == null) { writer.WriteNil(); return; }
            var r = options.Resolver;
            writer.WriteArrayHeader(9);
            r.GetFormatter<int>().Serialize(ref writer, value.ViewerId, options);
            r.GetFormatter<string>().Serialize(ref writer, value.Name, options);
            r.GetFormatter<int>().Serialize(ref writer, value.AvatarId, options);
            r.GetFormatter<int>().Serialize(ref writer, value.BoardId, options);
            r.GetFormatter<TutorialStep>().Serialize(ref writer, value.TutorialStep, options);
            r.GetFormatter<int>().Serialize(ref writer, value.CompletedBattleCount, options);
            r.GetFormatter<int>().Serialize(ref writer, value.Grade, options);
            r.GetFormatter<long>().Serialize(ref writer, value.BattlePoint, options);
            r.GetFormatter<DateTimeOffset>().Serialize(ref writer, value.LastLoggedInAt, options);
        }

        public UserResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = new UserResponse();
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.ViewerId = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 1: v.Name = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    case 2: v.AvatarId = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 3: v.BoardId = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 4: v.TutorialStep = r.GetFormatter<TutorialStep>().Deserialize(ref reader, options); break;
                    case 5: v.CompletedBattleCount = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 6: v.Grade = r.GetFormatter<int>().Deserialize(ref reader, options); break;
                    case 7: v.BattlePoint = r.GetFormatter<long>().Deserialize(ref reader, options); break;
                    case 8: v.LastLoggedInAt = r.GetFormatter<DateTimeOffset>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }

    public sealed class MasterDataResponseStaticFormatter : IMessagePackFormatter<MasterDataResponse>
    {
        public void Serialize(ref MessagePackWriter writer, MasterDataResponse value, MessagePackSerializerOptions options)
        {
            var r = options.Resolver;
            writer.WriteArrayHeader(2);
            r.GetFormatter<byte[]>().Serialize(ref writer, value.Data, options);
            r.GetFormatter<string>().Serialize(ref writer, value.Hash, options);
        }

        public MasterDataResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return default;
            var r = options.Resolver;
            int cnt = reader.ReadArrayHeader();
            var v = default(MasterDataResponse);
            for (int i = 0; i < cnt; i++)
            {
                switch (i)
                {
                    case 0: v.Data = r.GetFormatter<byte[]>().Deserialize(ref reader, options); break;
                    case 1: v.Hash = r.GetFormatter<string>().Deserialize(ref reader, options); break;
                    default: reader.Skip(); break;
                }
            }
            return v;
        }
    }
}
