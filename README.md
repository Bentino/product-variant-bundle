# Product Variant Bundle API

> **Technical Assignment - .NET 8 Web API**  
> A comprehensive e-commerce API demonstrating Clean Architecture, Docker containerization, and advanced product management functionality.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue.svg)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A .NET 8 Web API for managing product variants and bundles with flexible schema design, built as a technical assignment to demonstrate modern software development practices.

## Architecture

This project follows Clean Architecture principles with the following layers:

- **API Layer** (`ProductVariantBundle.Api`): Web API controllers and configuration
- **Core Layer** (`ProductVariantBundle.Core`): Business logic and domain entities
- **Infrastructure Layer** (`ProductVariantBundle.Infrastructure`): Data access and external services
- **Tests** (`ProductVariantBundle.Tests`): Unit and integration tests

## Technology Stack

- .NET 8 Web API
- Entity Framework Core
- PostgreSQL Database
- AutoMapper
- Swagger/OpenAPI
- Docker & Docker Compose

## Getting Started

### Prerequisites

- Docker and Docker Compose
- No local .NET SDK required (runs entirely in containers)

### Running with Docker

#### Option 1: From Source Code (Development)

1. **Clone the repository:**
   ```bash
   git clone git@github.com:Bentino/product-variant-bundle.git
   cd product-variant-bundle
   ```

2. **Start the application:**
   ```bash
   docker compose up -d
   ```
   > This builds the Docker image from source code using the Dockerfile

3. **Set up demo data (optional):**
   ```bash
   ./scripts/demo-setup.sh
   ```

4. **Validate the setup:**
   ```bash
   ./scripts/test-api.sh
   ```

#### Option 2: Pre-built Image (For Review/Testing)

If you have received a pre-built Docker image:

1. **Load the Docker image:**
   ```bash
   # If you received a .tar file:
   docker load -i product-variant-bundle-api.tar
   
   # Or if it's in a registry:
   docker pull <registry>/product-variant-bundle-api:latest
   ```

2. **Create a simple docker-compose.yml:**
   ```yaml
   version: '3.8'
   services:
     postgres:
       image: postgres:15
       environment:
         POSTGRES_DB: ProductVariantBundle
         POSTGRES_USER: postgres
         POSTGRES_PASSWORD: postgres123
       ports:
         - "5432:5432"
     
     api:
       image: product-variant-bundle-api:latest  # Use pre-built image
       ports:
         - "8080:8080"
       depends_on:
         - postgres
       environment:
         - ConnectionStrings__DefaultConnection=Host=postgres;Database=ProductVariantBundle;Username=postgres;Password=postgres123
   ```

3. **Start the services:**
   ```bash
   docker compose up -d
   ```

#### Access Points
- **Swagger UI**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/health
- **API Base URL**: http://localhost:8080/api

### Container-Based Development

This project runs entirely in Docker containers. Use these commands:

1. **Build the application:**
   ```bash
   docker compose exec api dotnet build
   # Or use helper script:
   ./scripts/dev.sh build
   ```

2. **Run tests:**
   ```bash
   docker compose exec api dotnet test
   # Or use helper script:
   ./scripts/dev.sh test
   ```

3. **Access container shell:**
   ```bash
   docker compose exec api bash
   # Or use helper script:
   ./scripts/dev.sh shell
   ```

4. **Access database:**
   ```bash
   docker compose exec postgres psql -U postgres -d ProductVariantBundle
   # Or use helper script:
   ./scripts/dev.sh db
   ```

## API Documentation

The API documentation is available via Swagger UI at the root URL when running in development mode.

## Database

The application uses PostgreSQL as the primary database. The connection is configured via Docker Compose with the following default credentials:

- Host: localhost:5432
- Database: ProductVariantBundle
- Username: postgres
- Password: postgres123

## Project Structure

```
├── src/
│   ├── ProductVariantBundle.Api/          # Web API layer
│   ├── ProductVariantBundle.Core/         # Business logic layer
│   ├── ProductVariantBundle.Infrastructure/ # Data access layer
│   └── ProductVariantBundle.Tests/        # Test projects
├── scripts/
│   └── init.sql                          # Database initialization
├── docker-compose.yml                    # Docker Compose services configuration
├── Dockerfile                           # API container configuration
└── ProductVariantBundle.sln             # Solution file
```

## Development

### Adding New Features

1. Define domain entities in the Core layer
2. Implement repositories in the Infrastructure layer
3. Create controllers in the API layer
4. Add appropriate tests

### Database Migrations

All EF Core migrations run through containers:

```bash
# Create new migration
docker compose exec api dotnet ef migrations add <MigrationName> \
  --project /src/src/ProductVariantBundle.Infrastructure \
  --startup-project /src/src/ProductVariantBundle.Api

# Update database
docker compose exec api dotnet ef database update \
  --project /src/src/ProductVariantBundle.Infrastructure \
  --startup-project /src/src/ProductVariantBundle.Api

# Or use helper scripts:
./scripts/dev.sh migration <MigrationName>
./scripts/dev.sh update-db
```

## Testing

### Unit Tests
Run tests through containers:

```bash
# Run all tests
docker compose exec api dotnet test

# Or use helper script:
./scripts/dev.sh test
```

### API Testing
Validate API functionality:

```bash
# Quick API validation
./scripts/test-api.sh

# Comprehensive test scenarios
# See TEST_SCENARIOS.md for detailed examples
```

## Development Workflow

### Container Commands
| Task | Container Command | Helper Script |
|------|------------------|---------------|
| Build | `docker compose exec api dotnet build` | `./scripts/dev.sh build` |
| Test | `docker compose exec api dotnet test` | `./scripts/dev.sh test` |
| Migration | `docker compose exec api dotnet ef migrations add <name>` | `./scripts/dev.sh migration <name>` |
| Update DB | `docker compose exec api dotnet ef database update` | `./scripts/dev.sh update-db` |
| Shell Access | `docker compose exec api bash` | `./scripts/dev.sh shell` |
| Database Access | `docker compose exec postgres psql -U postgres -d ProductVariantBundle` | `./scripts/dev.sh db` |

### Quick Development Cycle
```bash
# 1. Start services
docker compose up -d

# 2. Make code changes
# (edit files directly - they're mounted as volumes)

# 3. Build and test
./scripts/dev.sh build
./scripts/dev.sh test

# 4. Test API
./scripts/test-api.sh

# 5. Create migration (if needed)
./scripts/dev.sh migration AddNewFeature
./scripts/dev.sh update-db
```

## Deployment & Distribution

### Building Production Image

To create a production-ready Docker image:

```bash
# Build production image
docker build -t product-variant-bundle-api:latest --target final .

# Test the production image
docker run -d --name test-api -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=ProductVariantBundle;Username=postgres;Password=postgres123" \
  product-variant-bundle-api:latest
```

### Sharing the Project

#### For Code Review:
1. **Share the repository** (recommended)
2. **Include .env file** with default values
3. **Provide setup instructions** from this README

#### For Demo/Testing:
1. **Export Docker image:**
   ```bash
   docker save -o product-variant-bundle-api.tar product-variant-bundle-api:latest
   ```

2. **Share the .tar file** along with:
   - Simple docker-compose.yml (see Option 2 above)
   - Basic setup instructions

#### For Production Deployment:
1. **Push to container registry:**
   ```bash
   docker tag product-variant-bundle-api:latest your-registry/product-variant-bundle-api:latest
   docker push your-registry/product-variant-bundle-api:latest
   ```

2. **Use production docker-compose.yml** with proper environment variables

## Contributing

1. Follow Clean Architecture principles
2. Use container-based development workflow
3. Write unit tests for business logic
4. Update API documentation
5. Follow C# coding conventions

## Documentation

- **[Quick Start Guide](QUICK_START.md)** - Get started in minutes
- **[Development Guide](DEVELOPMENT.md)** - Container-based development
- **[Deployment Guide](DEPLOYMENT.md)** - Distribution and deployment options
- **[Demo Guide](DEMO_GUIDE.md)** - Comprehensive demo scenarios
- **[API Documentation](API_DOCUMENTATION.md)** - Complete API reference
- **[Test Scenarios](TEST_SCENARIOS.md)** - Testing examples