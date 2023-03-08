using System.Text.Json;

namespace DotnetPrompt.Npgsql.PgVector;

/// <summary>
/// Database vector mapping class
/// </summary>
public class Vector 
{
    public float[] Values { get; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="vector">float list</param>
    public Vector(float[] vector)
    {
        Values = vector;
    }

    /// <summary>
    /// Length of the vector in bytes
    /// </summary>
    public int ByteLength => Values.Length * sizeof(float);

    public override string ToString()
    {
        return JsonSerializer.Serialize(Values);
    }
}