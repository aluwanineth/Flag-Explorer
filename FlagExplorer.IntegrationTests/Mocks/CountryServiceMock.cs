using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FlagExplorer.IntegrationTests.Mocks;

public class TestCountryServiceAsync : ICountryServiceAsync
{
    private readonly ILogger<TestCountryServiceAsync> _logger;
    private readonly IMemoryCache _cache;
    private readonly List<CountryDto> _countries;
    private readonly List<CountryDetailsDto> _countryDetails;

    public TestCountryServiceAsync(ILogger<TestCountryServiceAsync> logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;

        _countries = new List<CountryDto>
        {
            new CountryDto { Name = "United States", Flag = "https://flagcdn.com/w320/us.png" },
            new CountryDto { Name = "Canada", Flag = "https://flagcdn.com/w320/ca.png" },
            new CountryDto { Name = "Mexico", Flag = "https://flagcdn.com/w320/mx.png" }
        };

        _countryDetails = new List<CountryDetailsDto>
        {
            new CountryDetailsDto
            {
                Name = "United States",
                Flag = "https://flagcdn.com/w320/us.png",
                Population = 331002651,
                Capital = "Washington, D.C."
            },
            new CountryDetailsDto
            {
                Name = "Canada",
                Flag = "https://flagcdn.com/w320/ca.png",
                Population = 38005238,
                Capital = "Ottawa"
            },
            new CountryDetailsDto
            {
                Name = "Mexico",
                Flag = "https://flagcdn.com/w320/mx.png",
                Population = 128932753,
                Capital = "Mexico City"
            }
        };

        _cache.Set("AllCountries", _countries, TimeSpan.FromHours(1));
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting all countries");

        if (_cache.TryGetValue("AllCountries", out List<CountryDto>? cachedCountries) && cachedCountries != null)
        {
            _logger.LogInformation("Mock: Retrieved {Count} countries from cache", cachedCountries.Count);
            return cachedCountries;
        }

        await Task.Delay(10, cancellationToken);

        _cache.Set("AllCountries", _countries, TimeSpan.FromHours(1));

        return _countries;
    }

    public async Task<CountryDetailsDto> GetCountryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting country by name: {Name}", name);

        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Mock: Empty or whitespace name provided");
            throw new ArgumentException("Country name cannot be empty", nameof(name));
        }

        await Task.Delay(10, cancellationToken);

        var country = _countryDetails.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (country == null)
        {
            _logger.LogWarning("Mock: Country not found: {Name}", name);
            throw new HttpRequestException(
                $"Country '{name}' not found",
                null,
                HttpStatusCode.NotFound);
        }

        return country;
    }
}

public static class CountryServiceAsyncMock
{
    public static ICountryServiceAsync CreateMockedCountryServiceAsync()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TestCountryServiceAsync>();

        var cache = new MemoryCache(new MemoryCacheOptions());

        return new TestCountryServiceAsync(logger, cache);
    }
}
