using Microsoft.EntityFrameworkCore;
using TransactionAggregation.Api.Domain.Entities;
using static Dapper.SqlMapper;

namespace TransactionAggregation.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<AggregatedTransaction> AggregatedTransactions => Set<AggregatedTransaction>();
    public DbSet<AggregationRun> AggregationRuns => Set<AggregationRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        modelBuilder.Entity<AggregatedTransaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.IdempotencyKey)
                .HasMaxLength(128)
                .IsRequired();
            e.Property(x => x.CustomerId).HasMaxLength(100).IsRequired();
            e.Property(x => x.ExternalTransactionId).HasMaxLength(100).IsRequired();
            e.Property(x => x.Source).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500).IsRequired();
            e.Property(x => x.Merchant).HasMaxLength(200);
            e.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18, 2);

            e.HasIndex(x => x.IdempotencyKey).IsUnique();
            e.HasIndex(x => new { x.Source, x.ExternalTransactionId }).IsUnique();
            e.HasIndex(x => x.CustomerId);
            e.HasIndex(x => x.Category);
            e.HasIndex(x => x.TransactionDateUtc);
        });

        modelBuilder.Entity<AggregationRun>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.TriggeredBy).HasMaxLength(50).IsRequired();
            e.Property(x => x.ErrorMessage).HasMaxLength(4000);
        });
    }
}
