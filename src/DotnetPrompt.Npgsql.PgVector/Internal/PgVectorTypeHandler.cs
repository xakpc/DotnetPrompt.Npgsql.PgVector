using Npgsql;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace DotnetPrompt.Npgsql.PgVector.Internal;

internal class PgVectorTypeHandler : NpgsqlSimpleTypeHandler<Vector>
{
    public PgVectorTypeHandler(PostgresType postgresType) : base(postgresType)
    {
    }

    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        return ValidateAndGetLength((Vector)value, parameter);
    }

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
        NpgsqlParameter parameter, bool async, CancellationToken cancellationToken = new CancellationToken())
    {
        Write(value as Vector ?? throw new InvalidOperationException("value type is invalid"), buf, parameter);
        return Task.CompletedTask;
    }

    public override Vector Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
    {
        var dim = buf.ReadUInt16();
        var unused = buf.ReadUInt16();

        var values = new float[dim];
        for (var i = 0; i < dim; i++)
        {
            values[i] = buf.ReadSingle();
        }

        return new Vector(values);
    }

    public override int ValidateAndGetLength(Vector value, NpgsqlParameter? parameter)
    {
        return sizeof(ushort) + sizeof(ushort) + value.ByteLength;
    }

    public override void Write(Vector value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        //buf.WriteString(value.ToString());
        buf.WriteUInt16((ushort)value.Values.Length);
        buf.WriteUInt16(0);

        buf.WriteSingle(value.Values[0]);
        buf.WriteSingle(value.Values[1]);
        buf.WriteSingle(value.Values[2]);
    }
}