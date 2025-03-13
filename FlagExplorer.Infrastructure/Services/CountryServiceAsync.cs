using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FlagExplorer.Infrastructure.Services;

public class CountryServiceAsync(HttpClient httpClient,
    IConfiguration configuration,
    ILogger<CountryServiceAsync> logger,
    IMemoryCache cache) : ICountryServiceAsync
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<CountryServiceAsync> _logger = logger;
    private readonly string _apiUrl = configuration["ExternalApis:CountriesApi"] ?? "https://restcountries.com/v3.1";
    private readonly IMemoryCache _cache = cache;
    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue("AllCountries", out List<CountryDto>? cachedCountries) && cachedCountries != null)
        {
            _logger.LogInformation("Retrieved {Count} countries from cache", cachedCountries.Count);
            return cachedCountries;
        }

        var countries = await GetCountriesFromApi(cancellationToken);

        _cache.Set("AllCountries", countries, TimeSpan.FromHours(1));

        return countries;
    }
    public async Task<IEnumerable<CountryDto>> GetCountriesFromApi(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/all?fields=name,flags", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var countries = JsonDocument.Parse(content).RootElement;

            var result = new List<CountryDto>();

            foreach (var country in countries.EnumerateArray())
            {
                result.Add(new CountryDto
                {
                    Name = country.GetProperty("name").GetProperty("common").GetString() ?? string.Empty,
                    Flag = country.GetProperty("flags").GetProperty("png").GetString() ?? string.Empty
                });
            }
            return result;
        }
        catch (HttpRequestException ex) when (ex.InnerException is IOException)
        {
            _logger.LogError(ex, "SSL connection error occurred. Please check your certificates and URL.");
            throw new ApplicationException("Unable to connect to the external API due to an SSL connection issue.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving countries from external API");
            throw;
        }
    }
    public async Task<CountryDetailsDto> GetCountryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var sanitizedName = Uri.EscapeDataString(name); 
            _logger.LogInformation("Getting country details for {Name}", sanitizedName);

            var response = await _httpClient.GetAsync($"{_apiUrl}/name/{sanitizedName}?fullText=true", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Response content: {Content}", content);

            var countries = JsonDocument.Parse(content).RootElement;

            var country = countries.EnumerateArray().FirstOrDefault();

            var countryDetails = new CountryDetailsDto
            {
                Name = TryGetStringProperty(country, "name", "common") ?? name,
                Population = TryGetIntProperty(country, "population") ?? 0,
                Capital = TryGetStringFromArray(country, "capital") ?? "Unknown",
                Flag = TryGetStringProperty(country, "flags", "png") ?? ""
            };

            return countryDetails;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error retrieving country details for {Name}", name);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for country {Name}", name);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving country details for {Name}", name);
            throw;
        }
    }
    private static string? TryGetStringProperty(JsonElement element, string property, string? subProperty = null)
    {
        try
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            if (!element.TryGetProperty(property, out var prop))
                return null;

            if (subProperty == null)
                return prop.GetString();

            if (!prop.TryGetProperty(subProperty, out var subProp))
                return null;

            return subProp.GetString();
        }
        catch
        {
            return null;
        }
    }
    private static int? TryGetIntProperty(JsonElement element, string property)
    {
        try
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            if (!element.TryGetProperty(property, out var prop))
                return null;

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetInt32();

            return null;
        }
        catch
        {
            return null;
        }
    }
    private static string? TryGetStringFromArray(JsonElement element, string property)
    {
        try
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            if (!element.TryGetProperty(property, out var prop))
                return null;

            if (prop.ValueKind != JsonValueKind.Array || prop.GetArrayLength() == 0)
                return null;

            return prop[0].GetString();
        }
        catch
        {
            return null;
        }
    }
}
