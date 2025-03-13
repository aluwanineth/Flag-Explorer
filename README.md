# Flag Explorer

A modern web application that displays country flags and information, built with .NET Core 9 (backend) and Angular (frontend).

## Overview

This application consists of two main parts:
1. **Backend API**: REST API built with .NET Core 9 using Clean Architecture with CQRS pattern
2. **Frontend Application**: Angular application with a responsive UI for displaying country flags and details

## Features

- Home view with a grid of country flags
- Detail view showing country information (name, population, capital)
- Search functionality to filter countries by name
- Error handling and loading states
- Responsive design
- Clean architecture with CQRS pattern (backend)
- Unit and integration tests

## Prerequisites

- .NET SDK 9.0
- Node.js (v20+) and npm
- Angular CLI (`npm install -g @angular/cli`)

## Project Structure

### Backend Structure

```
FlagExplorer/
├── FlagExplorer.Api/                # API Controllers and configuration
├── FlagExplorer.Application/        # Application layer (business logic, CQRS handlers)
├── FlagExplorer.Infrastructure/     # External concerns (data access, external APIs)
├── FlagExplorer.Shared/             # DTOs and shared models
├── FlagExplorer.IntegrationTests/   # Integration tests
├── FlagExplorer.UnitTests/          # Unit tests
└── FlagExplorer.sln                 # Solution file
```

### Frontend Structure

```
flag-explorer-app/
├── src/
│   ├── app/
│   │   ├── core/                    # Core services and models
│   │   ├── features/                # Feature modules (countries)
│   │   │   └── countries/           # Country-related components
│   │   └── shared/                  # Shared components and utilities
│   └── e2e/                         # TestCafe integration tests
└── angular.json                     # Angular configuration
```

## Installation

### Backend

```bash
cd FlagExplorer
dotnet restore
dotnet build
```

### Frontend

```bash
cd flag-explorer-app
npm install
```

## Configuration

### Backend Configuration

The application's settings are configured in `appsettings.json`:

Key configuration items:
- **ExternalApis:CountriesApi**: External API URL for country data (default: "https://restcountries.com/v3.1")
- **Logging**: Configuration for built-in .NET logging
- **Serilog**: Structured logging configuration with console and file outputs (logs stored in "Logs/log-.txt")

### Frontend Configuration

API URLs are configured in environment files:

```typescript
// src/environments/environment.ts (development)
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7122/api'
};

// src/environments/environment.prod.ts (production)
export const environment = {
  production: true,
  apiUrl: 'https://localhost:7122/api'
};
```
## Running the Application

### Backend

```bash
cd FlagExplorer/FlagExplorer.Api
dotnet run --urls "https://localhost:7122"
```

The API will be available at `https://localhost:7122/api` with Swagger documentation at `https://localhost:7122/swagger`

### Frontend

```bash
cd flag-explorer-app
ng serve
```

The application will be available at `http://localhost:4200`

## API Documentation

The API follows REST principles and includes Swagger documentation. The main endpoints are:

- `GET /api/v1/countries` - Get all countries
- `GET /api/v1/countries/{name}` - Get details for a specific country

You can explore the full API through the Swagger UI at `https://localhost:7122/swagger`.

## Testing

This project includes both unit tests and integration tests for comprehensive test coverage.

### Running Backend Tests

```bash
cd FlagExplorer
dotnet test
```

To run specific test projects:

```bash
# Run unit tests only
dotnet test tests/FlagExplorer.UnitTests

# Run integration tests only
dotnet test tests/FlagExplorer.IntegrationTests
```

### Running Frontend Tests

#### Unit Tests

The frontend unit tests are implemented using Jasmine and Karma:

```bash
cd flag-explorer-app
npm test
```

For a single test run with coverage report:

```bash
npm test -- --no-watch --code-coverage
```

## Test Coverage

### Backend Tests

Backend tests follow the same clean architecture as the main application:

#### Unit Tests
- **Application Layer Tests**: Test CQRS queries and commands in isolation
- **Domain Logic Tests**: Test business rules and domain entities
- **Service Tests**: Test individual services with mocked dependencies

#### Integration Tests
- **API Controller Tests**: Test full HTTP request pipeline
- **Repository Tests**: Test data access if applicable
- **Service Integration Tests**: Test services with actual dependencies

### Frontend Tests

Unit tests cover:
- **Services**: API communication, error handling
- **Components**: Rendering, user interactions
- **Pipes & Directives**: Transformation and UI behavior

Integration tests cover:
- End-to-end user flows
- Cross-component interactions
- Visual rendering

## Development Workflow

1. Make changes to the code
2. Run unit tests to ensure core functionality works
3. Run integration tests to verify end-to-end flows
4. Fix any issues
5. Commit changes

## CI/CD Pipeline

The project includes a CI/CD pipeline in `.github/workflows/flag-explorer-ci.yml`:
- **Build & Test Backend**: Restores dependencies, builds the API, and runs unit/integration tests
- **Build & Test Frontend**: Installs dependencies, runs frontend tests, and builds the frontend
- **Deploy**: Deploys the backend to Azure and the frontend to Vercel when changes are pushed to the `main` branch

## Additional Commands

### Backend

```bash
# Run with specific filter
dotnet test --filter "Category=Integration"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Frontend

```bash
# Run specific test files
ng test --include="**/country.service.spec.ts"
ng test --include="**/country-list.component.spec.ts"
ng test --include="**/country-details.component.spec.ts"
ng test --include="**/error-handler.service.spec.ts"

## Deployment

This application is deployed to Azure using:
- Azure Container Registry for Docker images
- Azure App Service for backend API
- Azure Static Web Apps for frontend

The deployed applications are available at:
- Backend API: https://flagexplorer-api.azurewebsites.net
- Frontend: https://flagexplorer-app.azurewebsites.net

## Troubleshooting

### Common Issues

#### Backend
- **API not starting**: Check port conflicts or missing dependencies 
- **Test failures**: Ensure environment is properly set up for tests

#### Frontend
- **API connection issues**: Ensure the backend is running and check CORS configuration
- **TestCafe cannot find elements**: Check that your application is running before starting tests
- **Unit test failures**: Use `fdescribe` or `fit` to focus on specific tests and check the Karma browser console for errors

## Acknowledgments

- [RestCountries API](https://restcountries.com/) for providing the country data
- The .NET Core and Angular communities