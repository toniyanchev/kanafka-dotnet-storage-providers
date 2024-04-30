using Kanafka.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Kanafka.Postgres;

public static class ServiceResolver
{
    public static void AddKanafkaPostgres(this IServiceCollection services, ProviderConfiguration providerConfiguration)
    {
        services.AddKanafka();
        services.AddSingleton(_ => new ProviderConfiguration(providerConfiguration.ConnectionString));
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(providerConfiguration.ConnectionString);
        services.AddScoped(_ => dataSourceBuilder.Build());
        services.AddScoped<IFailedMessagePersister, FailedMessagePersister>();

        if (providerConfiguration.InstallDbObjectsOnStart)
            FailedMessagePersister.InitProviderObjectsAsync(providerConfiguration.ConnectionString);
    }
}