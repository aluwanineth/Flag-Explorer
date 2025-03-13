using FlagExplorer.Application.Features.Countries.Queries;
using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using Moq;

namespace FlagExplorer.UnitTests.Countries.Queries;

[TestFixture]
public class GetCountryByNameQueryHandlerTests
{
    private Mock<ICountryServiceAsync> _mockCountryService = null!;
    private GetCountryByNameQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockCountryService = new Mock<ICountryServiceAsync>();
        _handler = new GetCountryByNameQueryHandler(_mockCountryService.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnCountryDetails()
    {
        var countryName = "Sweden";
        var expectedDetails = new CountryDetailsDto
        {
            Name = countryName,
            Population = 10_000_000,
            Capital = "Stockholm",
            Flag = "https://example.com/sweden.png"
        };

        _mockCountryService
            .Setup(x => x.GetCountryByNameAsync(countryName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDetails);

        var result = await _handler.Handle(new GetCountryByNameQuery(countryName), CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(expectedDetails.Name));
        Assert.That(result.Population, Is.EqualTo(expectedDetails.Population));
        Assert.That(result.Capital, Is.EqualTo(expectedDetails.Capital));
        Assert.That(result.Flag, Is.EqualTo(expectedDetails.Flag));
        _mockCountryService.Verify(x => x.GetCountryByNameAsync(countryName, It.IsAny<CancellationToken>()), Times.Once);
    }
}