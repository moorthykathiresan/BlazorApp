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

        [Fact]
        public async Task Get_WeatherForecast_Performance()
        {
            var client = _factory.CreateClient();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await client.GetAsync("/api/WeatherForecast");
            stopwatch.Stop();
            response.EnsureSuccessStatusCode();
            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
            // Stricter threshold: 100ms
            Assert.True(elapsedMs < 100, $"Response time was {elapsedMs}ms, which is too slow.");
            System.Console.WriteLine($"WeatherForecast endpoint response time: {elapsedMs}ms");
        }

        [Fact]
        public async Task Get_WeatherForecast_ParallelRequests_Performance()
        {
            var client = _factory.CreateClient();
            int parallelCount = 10;
            var tasks = new System.Threading.Tasks.Task<double>[parallelCount];
            for (int i = 0; i < parallelCount; i++)
            {
                tasks[i] = System.Threading.Tasks.Task.Run(async () => {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    var resp = await client.GetAsync("/api/WeatherForecast");
                    sw.Stop();
                    resp.EnsureSuccessStatusCode();
                    return sw.Elapsed.TotalMilliseconds;
                });
            }
            var results = await System.Threading.Tasks.Task.WhenAll(tasks);
            var avg = results.Average();
            // Assert average response time is below 150ms
            Assert.True(avg < 150, $"Average response time for parallel requests was {avg}ms, which is too slow.");
            System.Console.WriteLine($"Average response time for {parallelCount} parallel requests: {avg}ms");
        }

        [Fact]
        public async Task Get_WeatherForecast_ColdStart_Performance()
        {
            // Simulate cold start by creating a new factory/client
            var factory = new WebApplicationFactory<TodoApi.Program>();
            var client = factory.CreateClient();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await client.GetAsync("/api/WeatherForecast");
            stopwatch.Stop();
            response.EnsureSuccessStatusCode();
            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
            // Cold start threshold: 300ms
            Assert.True(elapsedMs < 300, $"Cold start response time was {elapsedMs}ms, which is too slow.");
            System.Console.WriteLine($"WeatherForecast cold start response time: {elapsedMs}ms");
        }
    }
}
