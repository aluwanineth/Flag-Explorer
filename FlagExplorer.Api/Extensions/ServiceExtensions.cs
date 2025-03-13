using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FlagExplorer.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Country API",
                Version = "v1.0.0",
                Description = "API for retrieving country information"
            });

            // Add XML comments if they exist
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
    }
}
