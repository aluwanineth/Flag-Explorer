using FlagExplorer.Api.Middlewares;

namespace FlagExplorer.Api.Extensions;

public static class AppExtensions
{
    public static void UseSwaggerExtension(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("../swagger/v1/swagger.json", "Country API v1");
        });
    }

    public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}

