using TelemetryStash.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TelemetryStash.Database;

public class TelemetryDbContext : DbContext
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<RegisterKey> RegisterKeys { get; set; }
    public DbSet<Register> Registers { get; set; }
    public DbSet<RegisterSet> RegisterSets { get; set; }
    public DbSet<TelemetryHistory> TelemetryHistory { get; set; }
    public DbSet<Telemetry> Telemetries { get; set; }
    public DbSet<TimestampHistory> Timestamps { get; set; }
    public DbSet<Timestamp> TimestampHistory { get; set; }

    public TelemetryDbContext() { }

    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options)
    {
        base.ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryDbContext).Assembly);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("");
    //}

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<string>()
            .HaveMaxLength(450);

        configurationBuilder.Properties<decimal>()
            .HavePrecision(19, 4);

        configurationBuilder.Properties<DateTimeOffset>()
            .HavePrecision(4);
    }
}

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasIndex(entity => entity.DeviceId).IsUnique();
    }
}

public class DeviceRegistrationConfiguration : IEntityTypeConfiguration<Register>
{
    public void Configure(EntityTypeBuilder<Register> builder)
    {
        builder.HasIndex(entity => new { entity.RegisterSetId, entity.RegisterIdentifier }).IsUnique();
    }
}

public class RegisterKeyRegistrationConfiguration : IEntityTypeConfiguration<RegisterKey>
{
    public void Configure(EntityTypeBuilder<RegisterKey> builder)
    {
        builder.HasIndex(entity => new { entity.RegisterId, entity.Subset }).IsUnique();
    }
}

public class RegisterSetRegistrationConfiguration : IEntityTypeConfiguration<RegisterSet>
{
    public void Configure(EntityTypeBuilder<RegisterSet> builder)
    {
        builder.HasIndex(entity => new { entity.DeviceId, entity.Identifier }).IsUnique();
    }
}

public class TelemetryHistoryRegistrationConfiguration : IEntityTypeConfiguration<Telemetry>
{
    public void Configure(EntityTypeBuilder<Telemetry> builder)
    {
        builder.HasIndex(entity => new { entity.RegisterKeyId, entity.TimestampId }).IsUnique();
    }
}

public class TimestampHistoryRegistrationConfiguration : IEntityTypeConfiguration<Timestamp>
{
    public void Configure(EntityTypeBuilder<Timestamp> builder)
    {
        builder.HasIndex(entity => new { entity.DeviceId, entity.Ts }).IsClustered(false).IsUnique();
    }
}
