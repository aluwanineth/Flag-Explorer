using FlagExplorer.Api.Extensions;
using FlagExplorer.Infrastructure;
using FlagExplorer.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    ContentRootPath = Directory.GetCurrentDirectory(),
    Args = args
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddSwaggerExtension();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddCors();
builder.Services.AddMemoryCache();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) 
                .AllowCredentials()); 
app.UseAuthorization();
app.UseSwaggerExtension();
app.MapControllers();

app.Run();

public partial class Program { }
