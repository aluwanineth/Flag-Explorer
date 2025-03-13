using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlagExplorer.Application;
public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR
             (cfg =>
                   cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
    }
}
