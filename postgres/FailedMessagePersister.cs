using System.Reflection;
using Kanafka.Storage;
using Npgsql;

namespace Kanafka.Postgres;

internal class FailedMessagePersister : IFailedMessagePersister
{
    private readonly ProviderConfiguration _providerConfiguration;
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public FailedMessagePersister(ProviderConfiguration providerConfiguration, NpgsqlDataSource npgsqlDataSource)
    {
        _providerConfiguration = providerConfiguration;
        _npgsqlDataSource = npgsqlDataSource;
    }

    public async Task PersistAsync(FailedMessage failedMessage, CancellationToken cancellationToken)
    {
        await using var connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
    }

    internal static void InitProviderObjectsAsync(string connectionString)
    {
        var psqlTablesScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init_tables.sql");
        var psqlTablesScript = File.ReadAllText(psqlTablesScriptPath, System.Text.Encoding.UTF8);
        var pgDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        using var connection = pgDataSource.CreateConnection();
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = psqlTablesScript;

        cmd.ExecuteNonQuery();
        connection.Close();
    }
}