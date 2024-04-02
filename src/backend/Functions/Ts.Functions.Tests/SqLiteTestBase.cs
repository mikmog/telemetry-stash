using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelemetryStash.Database;

namespace TelemetryStash.Functions.Tests;

public class SqLiteTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TelemetryDbContext> _contextOptions;

    public SqLiteTestBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var modelBuilder = SqliteConventionSetBuilder.CreateModelBuilder();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryDbContext).Assembly);

        // SqLite does not support DateTimeOffset.
        // Add conversion for DateTimeOffset properties.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType
                .GetProperties()
                .Where(p => p.PropertyType == typeof(DateTimeOffset));

            foreach (var property in properties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
        }

        var model = modelBuilder.Model.FinalizeModel();
        _contextOptions = new DbContextOptionsBuilder<TelemetryDbContext>()
            .UseModel(model)
            .UseSqlite(_connection)
            .Options;

        using var context = new TelemetryDbContext(_contextOptions);
        context.Database.EnsureCreated();
    }

    protected TelemetryDbContext GetDbContext() => new(_contextOptions);

    public void Dispose()
    {
        _connection.Dispose();
    }
}
