using FSH.Framework.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FSH.Framework.Persistence;

public static class OptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureHeroDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        string connectionString,
        string migrationsAssembly,
        bool isDevelopment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbProvider);

        switch (dbProvider.ToUpperInvariant())
        {
            case DbProviders.PostgreSQL:
                builder.ConfigureWarnings(warnings =>
                    warnings.Log(RelationalEventId.PendingModelChangesWarning));
                builder.UseNpgsql(connectionString, e =>
                {
                    e.MigrationsAssembly(migrationsAssembly);
                });
                break;

            case DbProviders.MSSQL:
                builder.ConfigureWarnings(warnings =>
                    warnings.Log(RelationalEventId.PendingModelChangesWarning));
                builder.UseSqlServer(connectionString, e =>
                {
                    e.MigrationsAssembly(migrationsAssembly);
                    e.EnableRetryOnFailure();
                });
                break;

            case DbProviders.InMemory:
                builder.UseInMemoryDatabase(string.IsNullOrEmpty(connectionString) ? "fsh" : connectionString);
                break;

            default:
                throw new InvalidOperationException(
                    $"Database Provider {dbProvider} is not supported.");
        }

        if (isDevelopment)
        {
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
        }

        return builder;
    }
}