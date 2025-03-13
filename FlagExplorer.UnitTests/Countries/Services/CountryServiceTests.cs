using FlagExplorer.Infrastructure.Services;
using FlagExplorer.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FlagExplorer.UnitTests.Countries.Services;

[TestFixture]
public class CountryServiceTests
{
    private CountryServiceAsync _service;
    private Mock<ILogger<CountryServiceAsync>> _loggerMock;
    private Mock<IMemoryCache> _cacheMock;
    private Mock<IConfiguration> _configMock;
    private Mock<HttpMessageHandler> _handlerMock;
    private HttpClient _httpClient;

    private string _testApiResponse;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CountryServiceAsync>>();

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["RestCountriesApi:BaseUrl"]).Returns("https://restcountries.com/v3.1");

        _cacheMock = new Mock<IMemoryCache>();
        var cacheEntry = new Mock<ICacheEntry>();
        _cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);

        _testApiResponse = JsonSerializer.Serialize(new[]
        {
            new {
                name = new { common = "United States", official = "United States of America" },
                flags = new { png = "https://flagcdn.com/w320/us.png" },
                population = 331002651,
                capital = new[] { "Washington, D.C." }
            },
            new {
                name = new { common = "Canada", official = "Canada" },
                flags = new { png = "https://flagcdn.com/w320/ca.png" },
                population = 38005238,
                capital = new[] { "Ottawa" }
            }
        });

        _handlerMock = new Mock<HttpMessageHandler>();
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(_testApiResponse)
            });

        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("https://restcountries.com/v3.1")
        };

        _service = new CountryServiceAsync(
            _httpClient,
            _configMock.Object,
            _loggerMock.Object,
            _cacheMock.Object);
    }

    [Test]
    public async Task GetAllCountriesAsync_CacheMiss_CallsApiAndReturnsData()
    {
        object? cachedValue = null;
        _cacheMock
            .Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(false);

        var result = await _service.GetAllCountriesAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri!.PathAndQuery.Contains("/all")),
            ItExpr.IsAny<CancellationToken>()
        );

        _cacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [Test]
    public async Task GetAllCountriesAsync_CacheHit_ReturnsDataFromCacheWithoutApiCall()
    {
        var cachedCountries = new List<CountryDto>
        {
            new CountryDto { Name = "Test Country", Flag = "test-flag.png" }
        };

        object? cachedValue = cachedCountries;
        _cacheMock
            .Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(true);

        var result = await _service.GetAllCountriesAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(cachedCountries));

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task GetCountryByNameAsync_ReturnsCorrectData()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.PathAndQuery.Contains("/name/Canada")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new[]
                {
                    new {
                        name = new { common = "Canada", official = "Canada" },
                        flags = new { png = "https://flagcdn.com/w320/ca.png" },
                        population = 38005238,
                        capital = new[] { "Ottawa" }
                    }
                }))
            });

        var result = await _service.GetCountryByNameAsync("Canada");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Canada"));
        Assert.That(result.Capital, Is.EqualTo("Ottawa"));
        Assert.That(result.Population, Is.EqualTo(38005238));
    }

    [Test]
    public void GetCountryByNameAsync_CountryNotFound_ThrowsHttpRequestException()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.PathAndQuery.Contains("/name/NonExistent")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            });

        var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _service.GetCountryByNameAsync("NonExistent"));

        Assert.That(ex.Message, Contains.Substring("404"));
    }

    [Test]
    public void GetCountryByNameAsync_ApiError_PropagatesException()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.PathAndQuery.Contains("/name/ErrorTest")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Server Error")
            });

        var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _service.GetCountryByNameAsync("ErrorTest"));

        Assert.That(ex.Message, Contains.Substring("500"));
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }
}
