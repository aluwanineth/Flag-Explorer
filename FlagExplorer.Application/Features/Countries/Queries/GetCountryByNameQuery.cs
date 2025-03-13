using FlagExplorer.Shared.Models;
using MediatR;

namespace FlagExplorer.Application.Features.Countries.Queries;
public record GetCountryByNameQuery(string Name) : IRequest<CountryDetailsDto>;
