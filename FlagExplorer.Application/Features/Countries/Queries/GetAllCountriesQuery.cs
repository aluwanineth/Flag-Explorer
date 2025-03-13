using FlagExplorer.Shared.Models;
using MediatR;

namespace FlagExplorer.Application.Features.Countries.Queries;

public record GetAllCountriesQuery() : IRequest<IEnumerable<CountryDto>>;
