# Product Variant Bundle API

> **Technical Assignment Submission**  
> A comprehensive .NET 8 Web API demonstrating Clean Architecture, Docker containerization, and advanced e-commerce functionality.

## 🎯 Assignment Overview

This project showcases:
- **Clean Architecture** implementation with .NET 8
- **Docker-first development** approach
- **Complex business logic** for product variants and bundles
- **Comprehensive testing** and documentation
- **Production-ready** containerization

## 🚀 Quick Demo (2 minutes)

```bash
# 1. Clone and start
git clone git@github.com:Bentino/product-variant-bundle.git
cd product-variant-bundle
docker compose up -d

# 2. Set up demo data
./scripts/demo-setup.sh

# 3. Test API
./scripts/test-api.sh

# 4. Open Swagger UI
open http://localhost:8080
```

## 🏗️ Technical Highlights

### Architecture & Design Patterns
- **Clean Architecture** with clear layer separation
- **Repository Pattern** for data access abstraction
- **Domain-Driven Design** with rich domain models
- **CQRS-like** separation with DTOs and mapping

### Advanced Features
- **Flexible Schema Design** using PostgreSQL JSONB
- **Global SKU Management** across variants and bundles
- **Complex Validation Logic** with business rules
- **Inventory Management** with reservations
- **Bundle Availability Calculation** from component stock

### DevOps & Containerization
- **Multi-stage Dockerfile** for development and production
- **Docker Compose** orchestration
- **Container-based development** workflow
- **Production-ready** image building

### Testing & Quality
- **Comprehensive API testing** with automated scripts
- **Demo data scenarios** for realistic testing
- **Error handling** with RFC 7807 compliance
- **Input validation** and business rule enforcement

## 📊 Project Metrics

- **~2,500 lines** of C# code
- **15+ API endpoints** with full CRUD operations
- **3-layer architecture** (API, Core, Infrastructure)
- **PostgreSQL** with complex relationships
- **Docker** containerization with hot reload
- **Comprehensive documentation** and testing

## 🛠️ Technology Stack

| Category | Technology |
|----------|------------|
| **Backend** | .NET 8, ASP.NET Core Web API |
| **Database** | PostgreSQL 15, Entity Framework Core |
| **Architecture** | Clean Architecture, Repository Pattern |
| **Containerization** | Docker, Docker Compose |
| **Documentation** | Swagger/OpenAPI, Comprehensive README |
| **Testing** | Custom API testing scripts, Demo scenarios |

## 📚 Documentation Structure

- **[README.md](README.md)** - Main project documentation
- **[QUICK_START.md](QUICK_START.md)** - Get started in 2 minutes
- **[DEVELOPMENT.md](DEVELOPMENT.md)** - Development workflow
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Distribution and deployment
- **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)** - Complete API reference
- **[DEMO_GUIDE.md](DEMO_GUIDE.md)** - Comprehensive demo scenarios

## 🎬 Demo Scenarios

The project includes realistic e-commerce scenarios:

1. **Product Catalog Management** - Electronics store with laptops, accessories
2. **Variant Management** - Memory/storage configurations with pricing
3. **Bundle Creation** - Gaming bundles with dynamic availability
4. **Inventory Tracking** - Multi-warehouse stock management
5. **Complex Validation** - Business rules and error handling

## 🔍 Code Quality Highlights

### Clean Architecture Implementation
```
src/
├── ProductVariantBundle.Api/          # Controllers, DTOs, Configuration
├── ProductVariantBundle.Core/         # Domain Models, Interfaces, Business Logic
└── ProductVariantBundle.Infrastructure/ # Data Access, External Services
```

### Domain-Rich Models
- **Product Master** with flexible attributes
- **Product Variants** with option values
- **Bundles** with component relationships
- **Inventory** with reservation logic

### Comprehensive Error Handling
- RFC 7807 Problem Details
- Validation error aggregation
- Business rule violations
- Graceful failure modes

## 🚀 Getting Started

### Prerequisites
- Docker and Docker Compose (that's it!)

### Quick Start
```bash
git clone git@github.com:Bentino/product-variant-bundle.git
cd product-variant-bundle
docker compose up -d
./scripts/demo-setup.sh
```

### Access Points
- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/health

## 💼 Assignment Context

This project was developed as a technical assignment to demonstrate:
- **Full-stack API development** capabilities
- **Clean code** and architecture principles
- **Docker and containerization** expertise
- **Database design** and ORM usage
- **Testing and documentation** practices
- **Production readiness** considerations

## 📞 Contact

For questions about this implementation or technical discussions, please feel free to reach out.

---

**⭐ If this project demonstrates the technical skills you're looking for, I'd love to discuss it further in an interview!**