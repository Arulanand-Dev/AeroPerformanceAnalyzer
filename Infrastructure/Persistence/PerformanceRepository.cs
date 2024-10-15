using AeroMetrics.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroMetrics.Infrastructure.Persistence
{
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly PerformanceDbContext _dbContext;

        public PerformanceRepository(PerformanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTelemetryDataAsync(List<TelemetryData> telemetryData)
        {

            try
            {
                await _dbContext.TelemetryData.AddRangeAsync(telemetryData);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding telemetry data.", ex);
            }
        }

        public async Task<List<TelemetryData>> GetAllTelemetryDataAsync()
        {
            try
            {
                return await _dbContext.TelemetryData.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all telemetry data.", ex);
            }
        }

        public async Task ClearTelemetryDataAsync()
        {
            try
            {
                _dbContext.TelemetryData.RemoveRange(_dbContext.TelemetryData);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while clearing telemetry data.", ex);
            }
        }
    }
}
