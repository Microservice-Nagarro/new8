using BHF.MS.new8.Models;
using BHF.MS.new8.Models.Settings;
using BHF.MS.new8.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace BHF.MS.new8.Tests.Services
{
    public class ExampleServiceTests
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new(MockBehavior.Strict);
        private readonly IOptions<ServiceSettings> _settings = Options.Create(new ServiceSettings
        {
            Endpoint1Uri = "/abc"
        });
        private readonly ExampleService _sut;

        public ExampleServiceTests()
        {
            _httpClient = _httpMessageHandlerMock.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.test.com");

            _sut = new ExampleService(_httpClient, _settings);
        }

        [Fact]
        public async Task GetSomething_ReturnsResponse()
        {
            // Arrange
            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get,
                    new Uri(_httpClient.BaseAddress!, _settings.Value.Endpoint1Uri))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await _sut.GetSomething();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostSomething_ReturnsResponse()
        {
            // Arrange
            var model = new ExampleModel { Title = "title", Email = "abc@abc.com", Phone = "123123123" };
            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Post,
                    new Uri(_httpClient.BaseAddress!, _settings.Value.Endpoint1Uri))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await _sut.PostSomething(model);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

