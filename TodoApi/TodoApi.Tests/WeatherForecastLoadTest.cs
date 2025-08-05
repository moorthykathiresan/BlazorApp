using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;

namespace TodoApi.Tests
{
    public class WeatherForecastLoadTest
    {
        public static void Main()
        {
            var httpClient = new HttpClient();
            var step = Step.Create("fetch_weather", async context =>
            {
                var response = await httpClient.GetAsync("http://localhost:5177/api/WeatherForecast");
                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder.CreateScenario("weather_load_test", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30)), // 20 concurrent users for 30s
                    Simulation.RampConstant(copies: 50, during: TimeSpan.FromSeconds(30))  // ramp up to 50 users for 30s
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
