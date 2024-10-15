using AeroMetrics.Core.Entities;
using AeroMetrics.Application.Services;
using AeroMetrics.Infrastructure.Persistence;
using Moq;
using AeroMetrics.Application.Exceptions;

namespace AeroMetrics.Tests.Application.Services
{
    public class PerformanceServiceTests
    {
        private readonly Mock<IPerformanceRepository> _performanceRepositoryMock;
        private readonly PerformanceService _service;

        public PerformanceServiceTests()
        {
            _performanceRepositoryMock = new Mock<IPerformanceRepository>();
            _service = new PerformanceService(_performanceRepositoryMock.Object);
        }

        [Fact]
        public async Task StoreTelemetryDataAsync_ValidData_ShouldStoreData()
        {
            // Arrange
            var telemetryData = "Time\tValue\tOuting\tChannel\n1.0\t0.5\t1\t2\n1.0\t0.2\t1\t4\n1.0\t0.3\t1\t5";
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(telemetryData);
            await writer.FlushAsync();
            stream.Position = 0;

            // Act
            await _service.StoreTelemetryDataAsync(stream);

            // Assert
            _performanceRepositoryMock.Verify(repo => repo.AddTelemetryDataAsync(It.IsAny<List<TelemetryData>>()), Times.Once);
        }

        [Fact]
        public async Task StoreTelemetryDataAsync_NullStream_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.StoreTelemetryDataAsync(null));
        }

        [Fact]
        public async Task GetTelemetryDataAsync_EmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.GetAllTelemetryDataAsync()).ReturnsAsync(new List<TelemetryData>());

            // Act
            var result = await _service.GetTelemetryDataAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTimesForDefaultConditionAsync_NoData_ShouldReturnDefaultValues()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.GetAllTelemetryDataAsync()).ReturnsAsync(new List<TelemetryData>());

            // Act
            var result = await _service.GetTimesForDefaultConditionAsync();

            // Assert
            Assert.Equal(0.0, result.FirstConditionTime);
            Assert.Equal(0.0, result.SecondConditionTime);
            Assert.Equal(0.0, result.BothConditionTime);
        }

        [Fact]
        public async Task ClearTelemetryDataAsync_ShouldCallRepository()
        {
            // Act
            await _service.ClearTelemetryDataAsync();

            // Assert
            _performanceRepositoryMock.Verify(repo => repo.ClearTelemetryDataAsync(), Times.Once);
        }

        [Fact]
        public async Task StoreTelemetryDataAsync_EmptyData_ShouldThrowNoTelemetryDataException()
        {
            // Arrange
            var telemetryData = "Time\tValue\tOuting\tChannel\n";
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(telemetryData);
            await writer.FlushAsync();
            stream.Position = 0;

            // Act & Assert
            await Assert.ThrowsAsync<NoTelemetryDataException>(() => _service.StoreTelemetryDataAsync(stream));
        }

        [Fact]
        public async Task GetTimeByCondition_NonExistentCondition_ShouldThrowInvalidConditionException()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.GetTelemetryDataByChannelAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<TelemetryData>
                {
                    new TelemetryData { Time = 1.0, Value = 0.5, Channel = 2 },
                });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidConditionException>(() => _service.GetTimeByConditionAsync(2, "INVALID_CONDITION", 0.5));
        }

        [Fact]
        public async Task GetTimesForDefaultConditionAsync_SingleConditionMet_ShouldReturnOnlyFirstConditionTime()
        {
            // Arrange
            var telemetryData = new List<TelemetryData>
            {
                new TelemetryData { Time = 1.0, Value = -0.6, Channel = 2 },
                new TelemetryData { Time = 1.0, Value = 0.5, Channel = 7 }
            };

            _performanceRepositoryMock.Setup(repo => repo.GetAllTelemetryDataAsync()).ReturnsAsync(telemetryData);

            // Act
            var result = await _service.GetTimesForDefaultConditionAsync();

            // Assert
            Assert.Equal(1.0, result.FirstConditionTime);
            Assert.Equal(0.0, result.SecondConditionTime);
            Assert.Equal(0.0, result.BothConditionTime);
        }

        [Fact]
        public async Task GetTimesForDefaultConditionAsync_BothConditionsMet_ShouldReturnBothConditionTime()
        {
            // Arrange
            var telemetryData = new List<TelemetryData>
            {
                new TelemetryData { Time = 1.0, Value = -0.6, Channel = 2 },
                new TelemetryData { Time = 1.0, Value = -0.1, Channel = 7 }
            };

            _performanceRepositoryMock.Setup(repo => repo.GetAllTelemetryDataAsync()).ReturnsAsync(telemetryData);

            // Act
            var result = await _service.GetTimesForDefaultConditionAsync();

            // Assert
            Assert.Equal(1.0, result.FirstConditionTime);
            Assert.Equal(1.0, result.SecondConditionTime);
            Assert.Equal(1.0, result.BothConditionTime);
        }

        [Fact]
        public async Task GetTimeByCondition_NoMatchingData_ShouldReturnDefaultTime()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.GetTelemetryDataByChannelAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<TelemetryData>());

            // Act
            var result = await _service.GetTimeByConditionAsync(2, ">", 0.5);

            // Assert
            Assert.Equal(0.0, result.Time);
        }

        [Fact]
        public async Task GetTimeByCondition_WhenChannelIsInvalid_ShouldThrowInvalidChannelException()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.GetTelemetryDataByChannelAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<TelemetryData>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetTimeByConditionAsync(-1, ">", 0.5));
        }

        [Fact]
        public async Task ClearTelemetryDataAsync_RepositoryThrowsException_ShouldThrowApplicationException()
        {
            // Arrange
            _performanceRepositoryMock.Setup(repo => repo.ClearTelemetryDataAsync()).ThrowsAsync(new ApplicationException("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<TelemetryDataProcessingException>(() => _service.ClearTelemetryDataAsync());
        }
    }
}
