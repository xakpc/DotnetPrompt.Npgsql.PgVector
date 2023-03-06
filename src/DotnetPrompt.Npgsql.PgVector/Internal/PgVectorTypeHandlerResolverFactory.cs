using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;

namespace DotnetPrompt.Npgsql.PgVector.Internal;

internal class PgVectorTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(NpgsqlConnector connector)
    {
        return new PgVectorTypeHandlerResolver(connector);
    }

    public override string? GetDataTypeNameByClrType(Type clrType)
    {
        return PgVectorTypeHandlerResolver.ClrTypeToDataTypeName(clrType);
    }

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
    {
        return PgVectorTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}