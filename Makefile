# Product Variant Bundle API Makefile

.PHONY: help build up down restart logs clean test db

help: ## Show this help message
	@echo "Product Variant Bundle API - Development Commands"
	@echo "================================================"
	@awk 'BEGIN {FS = ":.*##"} /^[a-zA-Z_-]+:.*##/ { printf "  %-15s %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

build: ## Build the Docker containers
	docker compose build

up: ## Start the development environment
	docker compose up --build

down: ## Stop the development environment
	docker compose down

restart: ## Restart the development environment
	docker compose down && docker compose up --build

logs: ## Show API logs
	docker compose logs -f api

clean: ## Clean up containers and volumes
	docker compose down -v
	docker system prune -f

test: ## Run tests
	docker compose exec api dotnet test

db: ## Connect to PostgreSQL database
	docker compose exec postgres psql -U postgres -d ProductVariantBundle

# Legacy commands for systems with older docker-compose
build-legacy: ## Build using docker-compose (legacy syntax)
	docker-compose build

up-legacy: ## Start using docker-compose (legacy syntax)
	docker-compose up --build

down-legacy: ## Stop using docker-compose (legacy syntax)
	docker-compose down

restart-legacy: ## Restart using docker-compose (legacy syntax)
	docker-compose down && docker-compose up --build

up-alt: ## Start using docker compose (newer syntax)
	docker compose up --build

down-alt: ## Stop using docker compose (newer syntax)
	docker compose down