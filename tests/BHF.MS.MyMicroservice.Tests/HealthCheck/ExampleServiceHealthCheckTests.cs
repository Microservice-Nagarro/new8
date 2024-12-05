using BHF.MS.new8.HealthCheck;
using BHF.MS.new8.Models.Settings;
using BHF.MS.new8.Services;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace BHF.MS.new8.Tests.HealthCheck
{
    public sealed class ExampleServiceHealthCheckTests
    {
        private readonly ExampleServiceHealthCheck _sut;
        private readonly Mock<IExampleService> _exampleServiceMock = new();
        private readonly Mock<ILogger<ExampleServiceHealthCheck>> _loggerMock = new();
        private readonly IOptions<HealthCheckSettings> _settings = Options.Create(new HealthCheckSettings
        {
            AttemptsCount = 2
        });

        public ExampleServiceHealthCheckTests()
        {
            _sut = new ExampleServiceHealthCheck(_exampleServiceMock.Object, _loggerMock.Object, _settings);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenSuccess_ReturnsHealthyResult_ShouldCallDoSomethingAsyncOnce()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _exampleServiceMock.Setup(x => x.GetSomething()).ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.CheckHealthAsync(new HealthCheckContext());

            // Assert
            _exampleServiceMock.Verify(x => x.GetSomething(), Times.Once);
            result.Should().NotBeNull();
            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenFailure_LogsFailureStatusCodes_ReturnsUnhealthyResult_ShouldCallCanConnectTwice()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _exampleServiceMock.Setup(x => x.GetSomething()).ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.CheckHealthAsync(new HealthCheckContext());

            // Assert
            _exampleServiceMock.Verify(x => x.GetSomething(), Times.Exactly(2));
            _loggerMock.VerifyLog(x => x.LogWarning("Failure status code returned: {StatusCode}", (int)responseMessage.StatusCode), Times.Exactly(2));
            result.Should().NotBeNull();
            result.Status.Should().Be(HealthStatus.Unhealthy);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenAttemptsCountEquals0_ReturnsUnhealthyResult()
        {
            // Assert
            _settings.Value.AttemptsCount = 0;

            // Act
            var result = await _sut.CheckHealthAsync(new HealthCheckContext());

            // Assert
            result.Should().NotBeNull();
            _exampleServiceMock.Verify(x => x.GetSomething(), Times.Never);
            result.Status.Should().Be(HealthStatus.Unhealthy);
        }
    }
}

