﻿using AeroMetrics.Core.Entities;

namespace AeroMetrics.Infrastructure.Persistence
{
    public interface IPerformanceRepository
    {
        Task AddTelemetryDataAsync(List<TelemetryData> telemetryData);
        Task<List<TelemetryData>> GetAllTelemetryDataAsync();
        Task ClearTelemetryDataAsync();
    }
}
