using AeroMetrics.Core.Entities;

namespace AeroMetrics.Core.Response
{
    public class AnalyzeTelemetryDataResult
    {
        public double Time { get; set; }
        public List<TelemetryData> Telemetry { get; set; }

        public AnalyzeTelemetryDataResult()
        {
            Telemetry = new List<TelemetryData>();
        }
    }
}
