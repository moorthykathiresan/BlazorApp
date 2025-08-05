using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;

class Program
{
    static void Main(string[] args)
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
                Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30)),
                Simulation.RampConstant(copies: 50, during: TimeSpan.FromSeconds(30))
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}