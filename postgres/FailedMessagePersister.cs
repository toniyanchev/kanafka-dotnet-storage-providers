using Kanafka.Storage;
using Npgsql;
using NpgsqlTypes;

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
        const string insertQuery =
            "insert into kanafka.failed_messages (id, failed_on, topic, message_id, message_body, message_headers, " +
            "exception_type, exception_message, exception_stack_trace, inner_exception_type, inner_exception_message, " +
            "retries) " +
            "values (@Id, @FailedOn, @Topic, @MessageId, @MessageBody, @MessageHeaders, @ExType, @ExMsg, " +
            "@ExStackTrace, @InrExType, @InrExMsg, @Retries)";

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        AddParameters(cmd, failedMessage);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        await connection.CloseAsync();
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

    private static void AddParameters(NpgsqlCommand command, FailedMessage failedMessage)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", Guid.NewGuid());
        parameters.AddWithValue("@FailedOn", failedMessage.CreatedOn);
        parameters.AddWithValue("@Topic", failedMessage.Topic);
        parameters.AddWithValue("@MessageId", failedMessage.MessageId);
        parameters.AddWithValue("@MessageBody", NpgsqlDbType.Jsonb, (object?)failedMessage.MessageBody ?? DBNull.Value);
        parameters.AddWithValue("@MessageHeaders", NpgsqlDbType.Jsonb,
            (object?)failedMessage.MessageHeaders ?? DBNull.Value);
        parameters.AddWithValue("@ExType", (object?)failedMessage.ExceptionType ?? DBNull.Value);
        parameters.AddWithValue("@ExMsg", (object?)failedMessage.ExceptionMessage ?? DBNull.Value);
        parameters.AddWithValue("@ExStackTrace", (object?)failedMessage.ExceptionStackTrace ?? DBNull.Value);
        parameters.AddWithValue("@InrExType", (object?)failedMessage.InnerExceptionType ?? DBNull.Value);
        parameters.AddWithValue("@InrExMsg", (object?)failedMessage.InnerExceptionMessage ?? DBNull.Value);
        parameters.AddWithValue("@Retries", failedMessage.Retries);
    }
}