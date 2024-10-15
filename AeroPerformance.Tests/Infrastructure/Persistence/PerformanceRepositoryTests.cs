using AeroMetrics.Core.Entities;
using AeroMetrics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AeroMetrics.Tests.Infrastructure.Persistence
{
    public class PerformanceRepositoryTests
    {
        private readonly PerformanceDbContext _dbContext;
        private readonly PerformanceRepository _repository;

        public PerformanceRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PerformanceDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _dbContext = new PerformanceDbContext(options);
            _repository = new PerformanceRepository(_dbContext);
        }

        [Fact]
        public async Task AddTelemetryDataAsync_ValidData_ShouldAddTelemetryData()
        {
            // Arrange
            var telemetryData = new List<TelemetryData>
            {
                new TelemetryData { Time = 1.0, Value = 1.0, Channel = 1, Outing = 1 },
                new TelemetryData { Time = 2.0, Value = 2.0, Channel = 1, Outing = 1 }
            };

            // Act
            await _repository.AddTelemetryDataAsync(telemetryData);

            // Assert
            var result = await _repository.GetAllTelemetryDataAsync();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllTelemetryDataAsync_ShouldReturnAllData()
        {
            // Arrange
            var telemetryData = new List<TelemetryData>
            {
                new TelemetryData { Time = 1.0, Value = 1.0, Channel = 2, Outing = 1 },
                new TelemetryData { Time = 2.0, Value = 2.0, Channel = 2, Outing = 1 }
            };
            await _repository.AddTelemetryDataAsync(telemetryData);

            // Act
            var result = await _repository.GetAllTelemetryDataAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ClearTelemetryDataAsync_ShouldRemoveAllData()
        {
            // Arrange
            var telemetryData = new List<TelemetryData>
            {
                new TelemetryData { Time = 1.0, Value = 1.0, Channel = 5, Outing = 1 },
                new TelemetryData { Time = 2.0, Value = 2.0, Channel = 5, Outing = 1 }
            };
            await _repository.AddTelemetryDataAsync(telemetryData);

            // Act
            await _repository.ClearTelemetryDataAsync();

            // Assert
            var result = await _repository.GetAllTelemetryDataAsync();
            Assert.Empty(result);
        }
    }
}
