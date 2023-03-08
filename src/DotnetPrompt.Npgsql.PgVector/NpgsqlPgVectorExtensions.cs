using DotnetPrompt.Npgsql.PgVector.Internal;
using Npgsql;
using Npgsql.TypeMapping;

namespace DotnetPrompt.Npgsql.PgVector;

/// <summary>
/// Extension allowing adding the PgVector plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlPgVectorExtensions
{
    /// <summary>
    /// Sets up PgVector mappings for the Vector types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    public static INpgsqlTypeMapper UsePgVector(this INpgsqlTypeMapper mapper)
    {
        mapper.AddTypeResolverFactory(new PgVectorTypeHandlerResolverFactory());
        return mapper;
    }

    /// <summary>
    /// Get Vector
    /// </summary>
    public static Vector GetVector(this NpgsqlDataReader reader, int ordinal) => reader.GetFieldValue<Vector>(ordinal);
}