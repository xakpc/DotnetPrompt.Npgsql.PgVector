using Microsoft.Extensions.Logging;
using Npgsql;

namespace DotnetPrompt.Npgsql.PgVector.Experiment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var vector = new Vector(new[] { 1f, 2f, 3f });

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Current directory {0}", Directory.GetCurrentDirectory());

            const string connectionString = "Host=db;Username=docker;Password=docker;Database=vectortest";



            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UsePgVector();
            await using var dataSource = dataSourceBuilder.Build();

            //dataSourceBuilder.UseLoggerFactory(loggerFactory);

            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS items (embedding vector(3))"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            //await using (var cmd = dataSource.CreateCommand("INSERT INTO items VALUES ('[1,2,3]')"))
            //{
            //    cmd.ExecuteNonQuery();
            //}


            await using (var cmd = dataSource.CreateCommand("DELETE FROM items"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using var connection = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("INSERT INTO items VALUES ($1), ($2)", connection))
            {
                cmd.Parameters.AddWithValue(new Vector(new[] { 1f, 2f, 3f }));
                cmd.Parameters.AddWithValue(new Vector(4f, 5f, 6f));
                await cmd.ExecuteNonQueryAsync();
            }

            //
            await using (var cmd = dataSource.CreateCommand("SELECT * FROM items ORDER BY embedding <-> ($1) LIMIT 1"))
            {
                cmd.Parameters.AddWithValue(new Vector(1f, 2f, 3f));

                var value = await cmd.ExecuteScalarAsync();
                Console.WriteLine(value);
            }

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

            await using (var cmd = dataSource.CreateCommand("SELECT AVG(embedding) FROM items"))
            {
                var value = await cmd.ExecuteScalarAsync();
                Console.WriteLine(value);
            }
        }
    }
}