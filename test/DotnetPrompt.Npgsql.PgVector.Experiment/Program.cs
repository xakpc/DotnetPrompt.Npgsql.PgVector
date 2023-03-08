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
            dataSourceBuilder.UseLoggerFactory(loggerFactory);

            await using var dataSource = dataSourceBuilder.Build();

            //await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS items (embedding vector(3))"))
            //{
            //    await cmd.ExecuteNonQueryAsync();
            //}

            //await using (var cmd = dataSource.CreateCommand("INSERT INTO items VALUES ('[1,2,3]')"))
            //{
            //    cmd.ExecuteNonQuery();
            //}

            //await using (var cmd = dataSource.CreateCommand("INSERT INTO items VALUES ($1)"))
            //{
            //    cmd.Parameters.AddWithValue(new Vector(new[] { 4f, 5f, 7f }));
            //    cmd.ExecuteNonQuery();
            //}

            await using (var cmd = dataSource.CreateCommand("SELECT * FROM items"))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetFieldValue<Vector>(0));
                    }
                }
            }

            await using (var cmd = dataSource.CreateCommand("SELECT * FROM items ORDER BY embedding <-> '[1,2,3]' LIMIT 5"))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetFieldValue<Vector>(0));
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