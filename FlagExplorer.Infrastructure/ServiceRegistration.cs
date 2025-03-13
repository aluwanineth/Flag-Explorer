using FlagExplorer.Application.Interfaces.Services;
using FlagExplorer.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlagExplorer.Infrastructure;
public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<ICountryServiceAsync, CountryServiceAsync>();
    }
}
