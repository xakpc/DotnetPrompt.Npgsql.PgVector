# Npgsql.PgVector - dotnet mapping for PostgreSQL pg_vector extension.

What is Npgsql.PgVector?

`Npgsql.PgVector` gives you convinient way to work with `pg_vector` extension to store and work
with vectors like Embedding vectors for LLM and more.

Read more about `pg_vector` extension, how to install and work with it [here](https://github.com/pgvector/pgvector).

#Getting Started

Installation

```ps
> dotnet add package DotnetPrompt.Npgsql.PgVector --version 1.0.0-alpha.1
```

To use the mapping, first, ensure that you [installed](https://github.com/pgvector/pgvector#installation) and loaded `pg_vector` extension and set it up like this:

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.UsePgVector();
await using var dataSource = dataSourceBuilder.Build();
```

Once the mapping is set up, you can transparently read and write the `Vector` object.

# Getting Started

Create a vector column with 3 dimensions

```csharp
await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS items (embedding vector(3))"))
{
    await cmd.ExecuteNonQueryAsync();
}
```

Insert values

```csharp
await using var connection = await dataSource.OpenConnectionAsync();
await using (var cmd = new NpgsqlCommand("INSERT INTO items VALUES ($1), ($2)", connection))
{
    cmd.Parameters.AddWithValue(new Vector(1f, 2f, 3f));
    cmd.Parameters.AddWithValue(new Vector(4f, 5f, 6f));
    await cmd.ExecuteNonQueryAsync();
}
```

Get the nearest neighbor by L2 distance

```csharp
await using (var cmd = dataSource.CreateCommand("SELECT * FROM items ORDER BY embedding <-> ($1) LIMIT 1"))
{
    cmd.Parameters.AddWithValue(new Vector(1f, 2f, 3f));

    var value = await cmd.ExecuteScalarAsync();
    Console.WriteLine(value);
}
```

```
> [1,2,3]
```
Also supports inner product (<#>) and cosine distance (<=>)

Note: <#> returns the negative inner product since Postgres only supports ASC order index scans on operators

# Querying

Use a SELECT clause to get the distance

```csharp
await using (var cmd = dataSource.CreateCommand("SELECT embedding <-> ($1) AS distance FROM items"))
{
    cmd.Parameters.AddWithValue(new Vector(3f, 2f, 1f));

    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetDouble(0));
        }
    }
}
```

```
> 2.8284271247461903
> 5.916079783099616
```

Use a WHERE clause to get rows within a certain distance

```csharp
await using (var cmd = dataSource.CreateCommand("SELECT * FROM items WHERE embedding <-> ($1) < ($2)"))
{
    cmd.Parameters.AddWithValue(new Vector(3f, 1f, 2f));
    cmd.Parameters.AddWithValue(5);

    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetVector(0));
        }
    }
}
```

```
> [1,2,3]
```
Note: Combine with ORDER BY and LIMIT to use an [index](https://github.com/pgvector/pgvector#indexing)

Get the average of vectors

```
await using (var cmd = dataSource.CreateCommand("SELECT AVG(embedding) FROM items"))
{
    var value = await cmd.ExecuteScalarAsync();
    Console.WriteLine(value);
}
```
```
> [2.5,3.5,4.5]
```
Learn more about queries, indexes and other stuff on official [pg_vector repository](https://github.com/pgvector/pgvector).
