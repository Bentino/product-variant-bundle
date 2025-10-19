#!/bin/bash

# Development helper script for Product Variant Bundle API

set -e

echo "🚀 Product Variant Bundle API Development Helper"
echo "================================================"

case "$1" in
    "start")
        echo "Starting development environment..."
        docker compose up --build
        ;;
    "stop")
        echo "Stopping development environment..."
        docker compose down
        ;;
    "restart")
        echo "Restarting development environment..."
        docker compose down
        docker compose up --build
        ;;
    "logs")
        echo "Showing logs..."
        docker compose logs -f api
        ;;
    "db")
        echo "Connecting to PostgreSQL database..."
        docker compose exec postgres psql -U postgres -d ProductVariantBundle
        ;;
    "clean")
        echo "Cleaning up containers and volumes..."
        docker compose down -v
        docker system prune -f
        ;;
    "test")
        echo "Running tests..."
        docker compose exec api dotnet test
        ;;
    "migration")
        if [ -z "$2" ]; then
            echo "Usage: $0 migration <migration-name>"
            exit 1
        fi
        echo "Creating migration: $2"
        docker compose exec api dotnet ef migrations add "$2" --project /src/src/ProductVariantBundle.Infrastructure --startup-project /src/src/ProductVariantBundle.Api
        ;;
    "update-db")
        echo "Updating database..."
        docker compose exec api dotnet ef database update --project /src/src/ProductVariantBundle.Infrastructure --startup-project /src/src/ProductVariantBundle.Api
        ;;
    "build")
        echo "Building application..."
        docker compose build api
        ;;
    "shell")
        echo "Opening API container shell..."
        docker compose exec api bash
        ;;
    *)
        echo "Usage: $0 {start|stop|restart|logs|db|clean|test|migration|update-db|build|shell}"
        echo ""
        echo "Environment Commands:"
        echo "  start     - Start the development environment"
        echo "  stop      - Stop the development environment"
        echo "  restart   - Restart the development environment"
        echo "  logs      - Show API logs"
        echo "  clean     - Clean up containers and volumes"
        echo ""
        echo "Development Commands:"
        echo "  test      - Run tests"
        echo "  migration <name> - Create EF migration"
        echo "  update-db - Update database with migrations"
        echo "  build     - Build application"
        echo "  shell     - Open API container shell"
        echo "  db        - Connect to PostgreSQL database"
        exit 1
        ;;
esac