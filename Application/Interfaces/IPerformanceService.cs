using AeroMetrics.Core.Response;

namespace AeroMetrics.Application.Interfaces
{
    public interface IPerformanceService
    {
        Task StoreTelemetryDataAsync(Stream fileStream);

        Task<List<AnalyzeTelemetryDataResult>> GetTelemetryDataAsync();

        Task<ConditionResult> GetTimeByConditionAsync(
            int channel, string condition, double value);
        Task<DefaultConditionResult> GetTimesForDefaultConditionAsync();

        Task ClearTelemetryDataAsync();
    }
}
