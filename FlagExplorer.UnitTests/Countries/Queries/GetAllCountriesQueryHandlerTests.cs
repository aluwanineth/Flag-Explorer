using FlagExplorer.Application.Features.Countries.Queries;
using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using Moq;

namespace FlagExplorer.UnitTests.Countries.Queries;

[TestFixture]
public class GetAllCountriesQueryHandlerTests
{
    private Mock<ICountryServiceAsync> _mockCountryService = null!;
    private GetAllCountriesQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockCountryService = new Mock<ICountryServiceAsync>();
        _handler = new GetAllCountriesQueryHandler(_mockCountryService.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnAllCountries()
    {
        var expectedCountries = new List<CountryDto>
    {
        new() { Name = "Sweden", Flag = "https://example.com/sweden.png" },
        new() { Name = "Norway", Flag = "https://example.com/norway.png" }
    };

        _mockCountryService
            .Setup(x => x.GetAllCountriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCountries);

        var result = await _handler.Handle(new GetAllCountriesQuery(), CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EquivalentTo(expectedCountries));
        _mockCountryService.Verify(x => x.GetAllCountriesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
