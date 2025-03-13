using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using MediatR;

namespace FlagExplorer.Application.Features.Countries.Queries;
public class GetCountryByNameQueryHandler(ICountryServiceAsync countryServiceAsync) : IRequestHandler<GetCountryByNameQuery, CountryDetailsDto>
{
    public async Task<CountryDetailsDto> Handle(GetCountryByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await countryServiceAsync.GetCountryByNameAsync(request.Name, cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new KeyNotFoundException($"Country not found: {request.Name}");
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Invalid format in country data for {request.Name}");
        }
    }
}
