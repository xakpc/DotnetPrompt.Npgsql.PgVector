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

            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS items (embedding vector(3))"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var cmd = dataSource.CreateCommand("INSERT INTO items VALUES ('[1,2,3]')"))
            {
                cmd.ExecuteNonQuery();
            }

            await using (var cmd = dataSource.CreateCommand("INSERT INTO items VALUES (@vec)"))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = "vec";
                par.Value = vector;
                cmd.Parameters.Add(par);
                //cmd.Parameters.AddWithValue(new Vector(new[] { 4f, 5f, 6f }));
                cmd.ExecuteNonQuery();
            }

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
        }
    }
}