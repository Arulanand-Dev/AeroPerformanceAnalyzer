using AeroMetrics.Core.Response;

namespace AeroMetrics.Application.Interfaces
{
    public interface IPerformanceService
    {
        Task StoreTelemetryDataAsync(Stream fileStream);

        Task<List<AnalyzeTelemetryDataResult>> GetTelemetryDataAsync();

        Task<DefaultConditionResult> GetTimesForDefaultConditionAsync();

        Task ClearTelemetryDataAsync();
    }
}
