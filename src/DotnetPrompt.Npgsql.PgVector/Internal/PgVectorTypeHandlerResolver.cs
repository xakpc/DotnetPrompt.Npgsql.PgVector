using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;

namespace DotnetPrompt.Npgsql.PgVector.Internal;

internal class PgVectorTypeHandlerResolver : TypeHandlerResolver
{
    private readonly PgVectorTypeHandler? _handler;

    private const string PgType = "vector";

    internal PgVectorTypeHandlerResolver(NpgsqlConnector connector)
    {
        var databaseInfo = connector.DatabaseInfo;

        var type = databaseInfo.TryGetPostgresTypeByName(PgType, out var pgType) ? pgType : null;

        if (type is not null)
            _handler = new PgVectorTypeHandler(type);
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName) => typeName.Equals(PgType) ? _handler : null;

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        return ClrTypeToDataTypeName(type) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;
    }

    internal static string? ClrTypeToDataTypeName(Type type)
    {
        return type != typeof(Vector)
            ? null
            : PgType;
    }

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
    {
        return DoGetMappingByDataTypeName(dataTypeName);
    }

    internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
    {
        return dataTypeName.Equals(PgType) ? new TypeMappingInfo(null, PgType, typeof(Vector)) : null;
    }
}