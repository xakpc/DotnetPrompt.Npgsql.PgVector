using Npgsql;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace DotnetPrompt.Npgsql.PgVector.Internal;

internal class PgVectorTypeHandler : NpgsqlTypeHandler<Vector>
{
    public PgVectorTypeHandler(PostgresType postgresType) : base(postgresType)
    {
    }

    public override ValueTask<Vector> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
    {
        var dim = buf.ReadUInt16();
        var unused = buf.ReadUInt16();

        var values = new float[dim];
        for (var i = 0; i < dim; i++)
        {
            values[i] = buf.ReadSingle();
        }

        return new ValueTask<Vector>(new Vector(values));
    }

    public override int ValidateAndGetLength(Vector value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        return sizeof(ushort) + sizeof(ushort) + value.ByteLength;
    }

    public override Task Write(Vector value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken = new CancellationToken())
    {
        buf.WriteUInt16((ushort)value.Values.Length);
        buf.WriteUInt16(0);

        foreach (var valueValue in value.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            buf.WriteSingle(valueValue);
        }

        return Task.CompletedTask;
    }

    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        return base.ValidateAndGetLength<Vector>(value as Vector, ref lengthCache, parameter);
    }

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.WriteWithLength(value as Vector, buf, lengthCache, parameter, async, cancellationToken);
    }
}