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

        public class WeatherForecast
        {
            public string Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }
            public int TemperatureF { get; set; }
        }
    }
}
