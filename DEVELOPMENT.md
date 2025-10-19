# Development Guide

## Container-Based Development

This project runs entirely in Docker containers. Use these command mappings:

### .NET Commands
| Native Command | Container Command | Helper Script |
|---|---|---|
| `dotnet build` | `docker compose exec api dotnet build` | `./scripts/dev.sh build` |
| `dotnet test` | `docker compose exec api dotnet test` | `./scripts/dev.sh test` |
| `dotnet run` | `docker compose up api` | `./scripts/dev.sh start` |
| `dotnet ef migrations add <name>` | `docker compose exec api dotnet ef migrations add <name> --project /src/src/ProductVariantBundle.Infrastructure --startup-project /src/src/ProductVariantBundle.Api` | `./scripts/dev.sh migration <name>` |
| `dotnet ef database update` | `docker compose exec api dotnet ef database update --project /src/src/ProductVariantBundle.Infrastructure --startup-project /src/src/ProductVariantBundle.Api` | `./scripts/dev.sh update-db` |

### Database Commands
| Native Command | Container Command | Helper Script |
|---|---|---|
| `psql -U postgres -d ProductVariantBundle` | `docker compose exec postgres psql -U postgres -d ProductVariantBundle` | `./scripts/dev.sh db` |
| `pg_dump` | `docker compose exec postgres pg_dump -U postgres ProductVariantBundle` | - |

### Development Workflow
```bash
# 1. Start development environment
./scripts/dev.sh start

# 2. Run tests
./scripts/dev.sh test

# 3. Create migration
./scripts/dev.sh migration AddNewFeature

# 4. Update database
./scripts/dev.sh update-db

# 5. View logs
./scripts/dev.sh logs

# 6. Access database
./scripts/dev.sh db
```

## Access Points
- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger  
- **PostgreSQL**: localhost:5432
- **PgAdmin**: http://localhost:8082 (admin@admin.com / admin123)

## Troubleshooting
```bash
# Check container logs for errors
./scripts/dev.sh logs

# Rebuild from scratch
docker compose down -v
docker compose build --no-cache
docker compose up

# Check network connectivity
docker compose exec api ping postgres

# Clean up unused images/containers
docker system prune
```

## Why Containers?
- **Consistent environment** across different machines
- **Isolated dependencies** - no need to install .NET, PostgreSQL locally
- **Easy setup** - `docker compose up` and you're ready
- **Production parity** - same environment as production