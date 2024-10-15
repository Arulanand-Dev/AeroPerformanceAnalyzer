using AeroMetrics.Api.Service.Controllers;
using AeroMetrics.Application.Interfaces;
using AeroMetrics.Core.Request;
using AeroMetrics.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AeroMetrics.Tests.Api;

public class PerformanceControllerTests
{
    private readonly Mock<IPerformanceService> _mockPerformanceService;
    private readonly PerformanceController _controller;

    public PerformanceControllerTests()
    {
        _mockPerformanceService = new Mock<IPerformanceService>();
        _controller = new PerformanceController(_mockPerformanceService.Object);
    }

    [Fact]
    public async Task StoreTelemetryData_ShouldReturnBadRequest_WhenFileContentIsNull()
    {
        // Arrange
        var request = new StoreTelemetryRequest { FileContent = null };

        // Act
        var result = await _controller.StoreTelemetryData(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Base64 file string is empty or not provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task StoreTelemetryData_ShouldReturnOk_WhenDataIsStoredSuccessfully()
    {
        // Arrange
        var request = new StoreTelemetryRequest { FileContent = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };
        _mockPerformanceService.Setup(s => s.StoreTelemetryDataAsync(It.IsAny<Stream>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.StoreTelemetryData(request);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetTelemetryData_ShouldReturnOk_WithResults()
    {
        // Arrange
        var mockResults = new List<AnalyzeTelemetryDataResult>();
        _mockPerformanceService.Setup(s => s.GetTelemetryDataAsync()).ReturnsAsync(mockResults);

        // Act
        var result = await _controller.GetTelemetryData();

        // Assert
        if (result.Value is List<AnalyzeTelemetryDataResult> response)
        {
            Assert.Equal(mockResults, response);

        }
    }

    [Fact]
    public async Task ClearTelemetryData_ShouldReturnOk_WhenDataClearedSuccessfully()
    {
        // Arrange
        _mockPerformanceService.Setup(s => s.ClearTelemetryDataAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ClearTelemetryData();

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task StoreTelemetryData_ShouldReturnBadRequest_WhenBase64StringIsInvalid()
    {
        // Arrange
        var request = new StoreTelemetryRequest { FileContent = "InvalidBase64" };

        // Act
        var result = await _controller.StoreTelemetryData(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid base64 string.", badRequestResult.Value);
    }
}