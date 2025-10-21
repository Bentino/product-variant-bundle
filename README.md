# Product Variant Bundle API

> **E-commerce API with Product Variants & Bundles Management**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue.svg)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg)](https://www.postgresql.org/)

A modern .NET 8 Web API for managing product catalogs with variants and bundles, featuring Clean Architecture and full Docker containerization.

## Features

- **Product Management**: Create products with flexible attributes
- **Variant System**: Multiple variants per product with different options
- **Bundle Management**: Group products into bundles with pricing
- **Inventory Tracking**: Stock levels and reservations
- **Web UI**: Frontend interface for catalog management
- **REST API**: Complete API with Swagger documentation

## Tech Stack

- .NET 8 Web API + Vue.js Frontend
- Entity Framework Core + PostgreSQL
- Docker & Docker Compose
- Swagger/OpenAPI Documentation

## 🚀 Quick Start

### Prerequisites
- Docker Desktop
- Git
- 4GB RAM, 2GB free space

### Installation

```bash
# 1. Clone repository
git clone git@github.com:Bentino/product-variant-bundle.git
cd product-variant-bundle

# 2. Start services
docker compose up -d

# 3. Create sample data (wait 30 seconds first)
curl -X POST http://localhost:8080/api/admin/reset-data \
  -H "Content-Type: application/json"
```

### Access Points
- **Frontend UI**: http://localhost:3000
- **API Documentation**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/health

## 🛠️ Development

### Container Commands
```bash
# Build application
docker compose exec api dotnet build

# Run tests
docker compose exec api dotnet test

# Access database
docker compose exec postgres psql -U postgres -d ProductVariantBundle

# View logs
docker compose logs api
```

### Helper Scripts
```bash
./scripts/dev.sh build    # Build application
./scripts/dev.sh test     # Run tests
./scripts/dev.sh shell    # Container shell
./scripts/dev.sh db       # Database shell
```

## 📁 Project Structure

```
├── src/                    # Backend (.NET 8 API)
├── frontend/              # Frontend (Vue.js)
├── scripts/               # Development scripts
├── docker-compose.yml     # Services configuration
└── Dockerfile            # API container
```

## 🔧 Troubleshooting

```bash
# Check containers
docker compose ps

# View logs
docker compose logs api

# Restart services
docker compose restart

# Reset everything
docker compose down -v && docker compose up -d
```

## 🎯 System Management

```bash
# Start
docker compose up -d

# Stop
docker compose down

# Stop + remove data
docker compose down -v
```

## 📚 API Features

- **Products**: CRUD with flexible attributes
- **Variants**: Multiple options per product
- **Bundles**: Group products with pricing
- **Inventory**: Stock tracking and reservations
- **Categories**: Dynamic category management
- **Search & Filter**: Advanced product filtering

## 🏗️ Architecture

**Clean Architecture** with:
- **API Layer**: Controllers and configuration
- **Core Layer**: Business logic and entities  
- **Infrastructure Layer**: Data access and services
- **Frontend**: Vue.js web interface

Built with Entity Framework Core, PostgreSQL, and Docker containerization.