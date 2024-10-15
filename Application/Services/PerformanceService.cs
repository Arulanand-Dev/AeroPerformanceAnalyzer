using AeroMetrics.Application.Exceptions;
using AeroMetrics.Application.Interfaces;
using AeroMetrics.Core.Entities;
using AeroMetrics.Core.Response;
using AeroMetrics.Infrastructure.Persistence;

namespace AeroMetrics.Application.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IPerformanceRepository _performanceRepository;

        public PerformanceService(IPerformanceRepository performanceRepository)
        {
            _performanceRepository = performanceRepository ?? throw new ArgumentNullException(nameof(performanceRepository));
        }

        public async Task StoreTelemetryDataAsync(Stream fileStream)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream), "File stream cannot be null.");

            try
            {
                var telemetryData = await ReadTelemetryData(fileStream);
                if (telemetryData == null || !telemetryData.Any())
                {
                    throw new NoTelemetryDataException("No telemetry data to store.");
                }

                var channel7 = CalculateChannel7(telemetryData);
                telemetryData.AddRange(channel7);

                await _performanceRepository.AddTelemetryDataAsync(telemetryData);
            }
            catch (NoTelemetryDataException)
            {
                throw;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TelemetryDataProcessingException("An error occurred while storing telemetry data.", ex);
            }
        }

        public async Task<List<AnalyzeTelemetryDataResult>> GetTelemetryDataAsync()
        {
            try
            {
                var telemetryData = await FetchTelemetryData();
                if (!telemetryData.Any()) return new List<AnalyzeTelemetryDataResult>();

                return telemetryData
                    .Where(t => t.Channel == 2 || t.Channel == 7)
                    .OrderBy(t => t.Time)
                    .GroupBy(t => t.Time)
                    .Select(telemetry => new AnalyzeTelemetryDataResult
                    {
                        Time = telemetry.Key,
                        Telemetry = telemetry.ToList()
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new TelemetryDataRetrievalException("An error occurred while retrieving telemetry data.", ex);
            }
        }

        public async Task<ConditionResult> GetTimeByConditionAsync(int channel, string condition, double value)
        {
            if (string.IsNullOrEmpty(condition)) throw new ArgumentNullException(nameof(condition), "Condition cannot be null or empty.");
            if (channel < 1) throw new ArgumentException("Invalid channel number.");

            var result = new ConditionResult { Condition = condition, Channel = channel };

            try
            {
                var telemetryData = await _performanceRepository.GetTelemetryDataByChannelAsync(channel);
                if (!telemetryData.Any()) return result;

                result.Time = EvaluateCondition(telemetryData, condition, value);
                return result;
            }
            catch (InvalidConditionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TelemetryDataRetrievalException("An error occurred while retrieving time by condition.", ex);
            }
        }

        public async Task<DefaultConditionResult> GetTimesForDefaultConditionAsync()
        {
            try
            {
                var telemetryData = await FetchTelemetryData();
                if (!telemetryData.Any()) return new DefaultConditionResult();

                return CalculateConditionTimes(telemetryData);
            }
            catch (Exception ex)
            {
                throw new TelemetryDataRetrievalException("An error occurred while retrieving times for default condition.", ex);
            }
        }

        public async Task ClearTelemetryDataAsync()
        {
            try
            {
                await _performanceRepository.ClearTelemetryDataAsync();
            }
            catch (Exception ex)
            {
                throw new TelemetryDataProcessingException("An error occurred while clearing telemetry data.", ex);
            }
        }

        private async Task<List<TelemetryData>> FetchTelemetryData()
        {
            var telemetryData = await _performanceRepository.GetAllTelemetryDataAsync();
            if (telemetryData == null || !telemetryData.Any()) return new List<TelemetryData>();
            return telemetryData;
        }

        private List<TelemetryData> CalculateChannel7(List<TelemetryData> telemetryData)
        {
            var groupedTelemetryData = telemetryData.Where(t => t.Channel == 4 || t.Channel == 5)
                .GroupBy(t => t.Time); //Grouped by Time to Calculate Channel7
             
              var channel7 = groupedTelemetryData.Select(t => new TelemetryData
                {
                    Time = t.Key,
                    Value = t.FirstOrDefault(x => x.Channel == 5)?.Value - t.FirstOrDefault(x => x.Channel == 4)?.Value ?? 0.0,
                    Channel = 7,
                    Outing = t.FirstOrDefault()?.Outing ?? 0
                })
                .ToList();

              return channel7;
        }

        private double EvaluateCondition(List<TelemetryData> telemetryData, string condition, double value)
        {
            return condition switch
            {
                "=" => telemetryData.FirstOrDefault(t => Math.Abs(t.Value - value) < double.Epsilon)?.Time ?? 0.0,
                ">" => telemetryData.FirstOrDefault(t => t.Value > value)?.Time ?? 0.0,
                "<" => telemetryData.FirstOrDefault(t => t.Value < value)?.Time ?? 0.0,
                ">=" => telemetryData.FirstOrDefault(t => t.Value >= value)?.Time ?? 0.0,
                "<=" => telemetryData.FirstOrDefault(t => t.Value <= value)?.Time ?? 0.0,
                _ => throw new InvalidConditionException($"Invalid condition '{condition}' specified.")
            };
        }

        private DefaultConditionResult CalculateConditionTimes(List<TelemetryData> telemetryData)
        {
            var firstConditionTime = double.MaxValue;
            var secondConditionTime = double.MaxValue;
            var bothConditionTime = 0.0;

            var groupedTelemetryByTime = telemetryData
                .Where(t => t.Channel == 2 || t.Channel == 7)
                .OrderBy(t => t.Time)
                .GroupBy(t => t.Time)
                .ToList();

            foreach (var telemetry in groupedTelemetryByTime)
            {
                var firstCondition = telemetry.FirstOrDefault(t => t.Channel == 2)?.Value < -0.5;
                var secondCondition = telemetry.FirstOrDefault(t => t.Channel == 7)?.Value < 0;

                if (firstCondition) firstConditionTime = Math.Min(telemetry.Key, firstConditionTime);
                if (secondCondition) secondConditionTime = Math.Min(telemetry.Key, secondConditionTime);
                if (firstCondition == true && secondCondition == true)
                {
                    bothConditionTime = telemetry.Key;
                    break;
                }
            }

            return new DefaultConditionResult
            {
                FirstConditionTime = firstConditionTime == double.MaxValue ? 0.0 : firstConditionTime,
                SecondConditionTime = secondConditionTime == double.MaxValue ? 0.0 : secondConditionTime,
                BothConditionTime = bothConditionTime
            };
        }

        private async Task<List<TelemetryData>> ReadTelemetryData(Stream fileStream)
        {
            var telemetryDataList = new List<TelemetryData>();

            // Dictionary to group data by time and channel
            var telemetryDataByTime = new Dictionary<double, List<TelemetryData>>();

            using var reader = new StreamReader(fileStream);
            string line;
            bool isFirstLine = true;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Skip the header line
                }

                var data = line.Split('\t'); // Assuming tab-separated values
                if (data.Length == 4 &&
                    double.TryParse(data[0], out var time) &&
                    double.TryParse(data[1], out var value) &&
                    int.TryParse(data[2], out var outing) &&
                    int.TryParse(data[3], out var channel))
                {
                    // Track data by time for interpolation
                    if (!telemetryDataByTime.ContainsKey(time))
                    {
                        telemetryDataByTime[time] = new List<TelemetryData>();
                    }

                    if (double.IsFinite(value))//if the value is not finite
                    {
                        telemetryDataByTime[time].Add(new TelemetryData
                        {
                            Time = time,
                            Value = value,
                            Outing = outing,
                            Channel = channel
                        });
                    } 
                }
                else
                {
                    throw new TelemetryDataFormatException("Invalid data format in the telemetry file.");
                }
            }

            // Now interpolate missing channel data for each time point
            var allChannelData = InterpolateMissingChannelData(telemetryDataByTime);

            return allChannelData;
        }

        private List<TelemetryData> InterpolateMissingChannelData(Dictionary<double, List<TelemetryData>> telemetryDataByTime)
        {
            var allTelemetryData = new List<TelemetryData>();

            // Define the channels you're working with (1 to 6 in this case)
            var channels = new int[] { 1, 2, 3, 4, 5, 6 };

            foreach (var timeEntry in telemetryDataByTime)
            {
                var time = timeEntry.Key;
                var dataAtTime = timeEntry.Value;

                foreach (var channel in channels)
                {
                    // Check if there's data for this channel at the current time
                    var channelData = dataAtTime.FirstOrDefault(t => t.Channel == channel);

                    if (channelData == null)
                    {
                        // No data for this channel at this time, so interpolate
                        var interpolatedValue = InterpolateValueForChannel(telemetryDataByTime, time, channel);

                        if (interpolatedValue.HasValue)
                        {
                            allTelemetryData.Add(new TelemetryData
                            {
                                Time = time,
                                Value = interpolatedValue.Value,
                                Channel = channel,
                                Outing = dataAtTime.FirstOrDefault()?.Outing ?? 0 // Reuse outing if available
                            });
                        }
                    }
                    else
                    {
                        // Add existing channel data
                        allTelemetryData.Add(channelData);
                    }
                }
            }

            return allTelemetryData;
        }

        // Interpolation method similar to what was used for Channel 7
        private double? InterpolateValueForChannel(Dictionary<double, List<TelemetryData>> telemetryDataByTime, double time, int channel)
        {
            // Find the nearest data points before and after this time for this channel
            var before = telemetryDataByTime
                .Where(t => t.Key < time && t.Value.Any(v => v.Channel == channel))
                .OrderByDescending(t => t.Key)
                .FirstOrDefault();

            var after = telemetryDataByTime
                .Where(t => t.Key > time && t.Value.Any(v => v.Channel == channel))
                .OrderBy(t => t.Key)
                .FirstOrDefault();

            if (before.Value != null && after.Value != null)
            {
                var beforeValue = before.Value.First(v => v.Channel == channel).Value;
                var afterValue = after.Value.First(v => v.Channel == channel).Value;

                // Linear interpolation
                var interpolatedValue = beforeValue + (afterValue - beforeValue) * ((time - before.Key) / (after.Key - before.Key));
                return interpolatedValue;
            }

            // If only the "before" data is available, carry forward its value
            if (before.Value != null)
            {
                return before.Value.First(v => v.Channel == channel).Value; // Last observation carried forward (LOCF)
            }

            // If only the "after" data is available, assume the value was 0 before
            if (after.Value != null)
            {
                return 0.0; // Assume missing value is 0 if no "before" data
            }

            // No before or after data available, return null or a fallback
            return 0.0;
        }

    }
}
