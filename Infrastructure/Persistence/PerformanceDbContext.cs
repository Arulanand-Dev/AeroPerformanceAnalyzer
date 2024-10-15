using AeroMetrics.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroMetrics.Infrastructure.Persistence
{
    public class PerformanceDbContext: DbContext
    {
        public DbSet<TelemetryData> TelemetryData { get; set; }

        public PerformanceDbContext(DbContextOptions<PerformanceDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelemetryData>()
                .HasKey(t => new { t.Time, t.Channel });
        }
    }
}
