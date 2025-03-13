using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Shared.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagExplorer.Application.Features.Countries.Queries;

public class GetAllCountriesQueryHandler(ICountryServiceAsync countryServiceAsync) : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
{
    public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        return await countryServiceAsync.GetAllCountriesAsync(cancellationToken);
    }
}
