using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TodoApi.Tests
{
    public class WeatherForecastControllerTests : IClassFixture<WebApplicationFactory<TodoApi.Program>>
    {
        private readonly WebApplicationFactory<TodoApi.Program> _factory;

        public WeatherForecastControllerTests(WebApplicationFactory<TodoApi.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsWeatherForecast()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/WeatherForecast");
            response.EnsureSuccessStatusCode();
            var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
            Assert.NotNull(forecasts);
            Assert.Equal(5, forecasts.Length);
        }

        [Fact]
        public async Task Get_ReturnsValidTemperatureRange()
        {
            var client = _factory.CreateClient();
            var forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/api/WeatherForecast");
            Assert.All(forecasts, f => Assert.InRange(f.TemperatureC, -20, 55));
        }

        [Fact]
        public async Task Get_ReturnsValidSummary()
        {
            var client = _factory.CreateClient();
            var forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/api/WeatherForecast");
            var validSummaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            Assert.All(forecasts, f => Assert.Contains(f.Summary, validSummaries));
        }

        [Fact]
        public async Task Get_InvalidEndpoint_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/InvalidEndpoint");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        public class WeatherForecast
        {
            public string Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }
            public int TemperatureF { get; set; }
        }
    }
}
