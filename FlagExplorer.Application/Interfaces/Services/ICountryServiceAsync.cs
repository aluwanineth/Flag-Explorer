using FlagExplorer.Shared.Models;

namespace FlagExplorer.Application.Interfaces.Services;
public interface ICountryServiceAsync
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    Task<CountryDetailsDto> GetCountryByNameAsync(string name, CancellationToken cancellationToken = default);
}
