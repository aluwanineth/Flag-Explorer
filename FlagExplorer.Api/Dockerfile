FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FlagExplorer.Api/FlagExplorer.Api.csproj", "FlagExplorer.Api/"]
COPY ["FlagExplorer.Application/FlagExplorer.Application.csproj", "FlagExplorer.Application/"]
COPY ["FlagExplorer.Infrastructure/FlagExplorer.Infrastructure.csproj", "FlagExplorer.Infrastructure/"]
COPY ["FlagExplorer.Shared/FlagExplorer.Shared.csproj", "FlagExplorer.Shared/"]
RUN dotnet restore "FlagExplorer.Api/FlagExplorer.Api.csproj"
COPY . .
WORKDIR "/src/FlagExplorer.Api"
RUN dotnet build "FlagExplorer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlagExplorer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlagExplorer.Api.dll"]