using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class WeatherForecastController : BaseApiController
  {
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
    }

    [HttpGet("file-name-from-url")]
    public string GetFileNameFromUrl(string url)
    {
      Uri SomeBaseUri = new Uri("http://canbeanything");
      Uri uri;
      if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        uri = new Uri(SomeBaseUri, url);
      return Path.GetFileName(uri.LocalPath);
    }
  }
}
