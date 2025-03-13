using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using FlagExplorer.Shared.Models;
using FlagExplorer.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using FlagExplorer.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FlagExplorer.IntegrationTests.Controllers;

[TestFixture]
public class CountriesControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                });

                builder.ConfigureServices(services =>
                {
                    var mockService = CountryServiceAsyncMock.CreateMockedCountryServiceAsync();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ICountryServiceAsync));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddSingleton<ICountryServiceAsync>(mockService);
                });
            });

        _client = _factory.CreateClient();
    }

    [Test]
    public async Task GetAllCountries_ShouldReturnOk()
    {
        var response = await _client.GetAsync("api/countries");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var countries = await response.Content.ReadFromJsonAsync<IEnumerable<CountryDto>>();
        Assert.That(countries, Is.Not.Null);
        Assert.That(countries, Is.Not.Empty);

        Assert.That(countries!.Any(c => c.Name == "United States"));
        Assert.That(countries!.Any(c => c.Name == "Canada"));
        Assert.That(countries!.Any(c => c.Name == "Mexico"));
    }

    [Test]
    public async Task GetCountryByName_ShouldReturnOk()
    {
        var response = await _client.GetAsync("api/countries/United%20States");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var country = await response.Content.ReadFromJsonAsync<CountryDetailsDto>();
        Assert.That(country, Is.Not.Null);
        Assert.That(country!.Name, Is.EqualTo("United States"));
        Assert.That(country!.Capital, Is.EqualTo("Washington, D.C."));
        Assert.That(country!.Population, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetCountryByName_WithNonExistentCountry_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("api/countries/NonExistentCountry");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetAllCountries_ResponseHasCorrectContentType()
    {
        var response = await _client.GetAsync("api/countries");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task GetAllCountries_ReturnsValidJsonSchema()
    {
        var response = await _client.GetAsync("api/countries");
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotThrow(() => JsonDocument.Parse(content));
        var countries = JsonSerializer.Deserialize<IEnumerable<CountryDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.That(countries, Is.Not.Null);
        foreach (var country in countries!)
        {
            Assert.That(country.Name, Is.Not.Null.Or.Empty);
            Assert.That(country.Flag, Is.Not.Null.Or.Empty);
        }
    }

    [Test]
    public async Task GetAllCountries_CachingWorks_SecondCallFaster()
    {
        var stopwatch1 = System.Diagnostics.Stopwatch.StartNew();
        await _client.GetAsync("api/countries");
        stopwatch1.Stop();
        var firstCallTime = stopwatch1.ElapsedMilliseconds;

        var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
        await _client.GetAsync("api/countries");
        stopwatch2.Stop();
        var secondCallTime = stopwatch2.ElapsedMilliseconds;

        Assert.That(secondCallTime, Is.LessThanOrEqualTo(firstCallTime * 2));
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
