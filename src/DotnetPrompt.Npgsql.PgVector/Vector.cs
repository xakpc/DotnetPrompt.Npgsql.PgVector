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
    /// <param name="vector">Parametric array of float</param>
    public Vector(params float[] vector)
    {
        Values = vector;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="vector">IEnumerable of float</param>
    public Vector(IEnumerable<float> vector)
    {
        Values = vector.ToArray();
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