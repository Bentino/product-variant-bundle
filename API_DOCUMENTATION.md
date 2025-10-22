# Product Variant Bundle API Documentation

## ⚠️ **MVP Demo Version**

**This is a Minimum Viable Product (MVP) demonstration** showcasing the architectural design and core concepts for a product variant and bundle management system. 

### **Current Status: Proof of Concept**
- ✅ **Core functionality implemented** - Basic CRUD operations and data management
- ✅ **Architecture validated** - Clean Architecture with proper separation of concerns  
- ✅ **Scalable design** - Flexible schema and extensible structure
- ⚠️ **Limited endpoints** - Not all REST operations are implemented
- ⚠️ **Demo purposes** - Suitable for concept validation and further development

### **Production Readiness: ~30%**
This system demonstrates the foundational architecture and can be extended to a full production system.

## Overview

The Product Variant Bundle API is a RESTful API for managing product catalogs with variants and bundles. Built with .NET 8 and PostgreSQL, it provides flexible schema design, inventory management, and batch operations.

## Quick Start

### Prerequisites
- Docker and Docker Compose
- Git

### Running the API

1. **Clone and start the services:**
   ```bash
   git clone git@github.com:Bentino/product-variant-bundle.git
   cd product-variant-bundle
   docker compose up -d
   ```

2. **Verify the API is running:**
   ```bash
   curl http://localhost:8080/health
   ```

3. **Access the interactive documentation:**
   - Open http://localhost:8080 in your browser
   - Or access the OpenAPI spec at http://localhost:8080/swagger/v1/swagger.json

## API Endpoints

### ✅ **Implemented Endpoints**

#### **Products**
- `GET /api/products` - List products with filtering and pagination
- `POST /api/products` - Create a new product master

#### **Bundles**
- `GET /api/bundles` - List bundles with filtering and pagination
- `POST /api/bundles` - Create a new bundle

#### **Inventory**
- `GET /api/inventory` - List inventory records with filtering
- `POST /api/inventory` - Create inventory record

#### **Batch Operations**
- `POST /api/batch/variants` - Create multiple variants
- `PUT /api/batch/variants` - Update multiple variants
- `POST /api/batch/bundles` - Create multiple bundles

#### **Health & Admin**
- `GET /api/health` - API health status
- `POST /api/admin/reset-data` - Reset database with sample data
- `DELETE /api/admin/clear-data` - Clear all data

### ⚠️ **Not Yet Implemented (Future Development)**

#### **Individual Resource Management**
- `GET /api/products/{id}` - Get product by ID
- `PUT /api/products/{id}` - Update product  
- `DELETE /api/products/{id}` - Delete product
- `GET /api/bundles/{id}` - Get bundle by ID
- `PUT /api/bundles/{id}` - Update bundle
- `DELETE /api/bundles/{id}` - Delete bundle

#### **Variant Management**
- `GET /api/products/{id}/variants` - Get product variants
- `POST /api/products/{id}/variants` - Create variant
- `PUT /api/products/{id}/variants/{variantId}` - Update variant
- `DELETE /api/products/{id}/variants/{variantId}` - Delete variant

#### **Advanced Inventory**
- `GET /api/inventory/{sku}` - Get inventory by SKU
- `PUT /api/inventory/{sku}/stock` - Update stock levels
- `POST /api/inventory/{sku}/reserve` - Reserve stock
- `POST /api/inventory/{sku}/release` - Release reserved stock

#### **Bundle Availability**
- `GET /api/bundles/{id}/availability` - Calculate bundle availability
- `POST /api/bundles/{id}/availability/reserve-check` - Check availability with locking

### 🔧 **Workarounds for Missing Endpoints**

#### **Product Management**
- **Edit/Delete Products**: Use frontend interface or database direct access
- **Individual Product Details**: Filter list endpoint by ID or slug

#### **Inventory Updates**  
- **Stock Management**: Use inventory list endpoint to find records, then update via database
- **Stock Tracking**: Available through inventory records, but no real-time updates

#### **Bundle Operations**
- **Bundle Availability**: Calculate manually from component inventory
- **Bundle Management**: Create new bundles instead of updating existing ones

## Key Features

### ✅ **Implemented Features**

#### **1. Flexible Schema Design**
- **JSONB Attributes**: Products, variants, and bundles support extensible JSON attributes
- **No Schema Migrations**: Add new properties without database changes
- **PostgreSQL Optimization**: Leverages JSONB indexing and constraints

#### **2. Global SKU Management**
- **Unique Namespace**: All sellable items (variants and bundles) share the same SKU space
- **Automatic Validation**: Prevents duplicate SKUs across different item types
- **Efficient Lookups**: Optimized database indexes for SKU searches

#### **3. Batch Operations**
- **Idempotency**: Prevent duplicate processing with idempotency keys
- **Conflict Resolution**: Skip, update, or fail strategies for conflicts
- **Atomic Operations**: All-or-nothing processing with rollback support
- **Performance**: Efficient bulk operations for large datasets

#### **4. Data Management**
- **Sample Data Generation**: Reset and populate database with sample data
- **Clean Architecture**: Proper separation of concerns and testability
- **Comprehensive Validation**: Business rules and data integrity validation

### ⚠️ **Partially Implemented**

#### **5. Inventory Management**
- ✅ **Basic Inventory**: Create and list inventory records
- ✅ **Stock Tracking**: Track on-hand and reserved quantities
- ❌ **Real-time Updates**: No API endpoints for stock updates
- ❌ **Reservations**: No reservation system implemented
- ❌ **Multi-warehouse Operations**: Limited warehouse support

#### **6. Bundle System**
- ✅ **Bundle Creation**: Create bundles with multiple items
- ✅ **Component Tracking**: Track bundle items and quantities
- ❌ **Availability Calculation**: No real-time availability checking
- ❌ **Dynamic Pricing**: No automatic price calculations

### 🚧 **Future Development**

#### **7. Advanced Features (Not Implemented)**
- **Individual Resource Management**: CRUD operations for single items
- **Real-time Inventory**: Live stock updates and reservations
- **Bundle Availability**: Dynamic availability calculation
- **Advanced Search**: Full-text search and complex filtering
- **Authentication**: User management and access control
- **Audit Trail**: Complete change tracking and history

## Data Models

### Product Master
```json
{
  "id": "uuid",
  "name": "Product Name",
  "slug": "product-name",
  "description": "Product description",
  "category": "Category",
  "attributes": {
    "brand": "Brand Name",
    "custom_field": "value"
  },
  "variants": [],
  "status": 0,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Product Variant
```json
{
  "id": "uuid",
  "productMasterId": "uuid",
  "sku": "UNIQUE-SKU",
  "price": 99.99,
  "optionValues": [
    {
      "optionName": "Color",
      "optionSlug": "color",
      "value": "Red"
    }
  ],
  "attributes": {
    "weight": "1.5kg"
  },
  "status": 0,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Product Bundle
```json
{
  "id": "uuid",
  "name": "Bundle Name",
  "description": "Bundle description",
  "sku": "BUNDLE-SKU",
  "price": 199.99,
  "items": [
    {
      "sellableItemId": "uuid",
      "quantity": 2
    }
  ],
  "metadata": {
    "promotion": "Holiday Special"
  },
  "status": 0,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Inventory Record
```json
{
  "id": "uuid",
  "sellableItemId": "uuid",
  "warehouseId": "uuid",
  "sku": "ITEM-SKU",
  "onHand": 100,
  "reserved": 10,
  "available": 90,
  "warehouseCode": "MAIN",
  "lastUpdated": "2024-01-01T00:00:00Z"
}
```

## Response Format

### Success Response
```json
{
  "data": {
    // Actual response data
  },
  "meta": {
    "page": 1,
    "pageSize": 20,
    "total": 100,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  },
  "errors": []
}
```

### Error Response (RFC 7807)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/products",
  "errors": {
    "slug": ["Product slug must be unique"],
    "variants[0].sku": ["SKU already exists"]
  }
}
```

### Batch Operation Response
```json
{
  "data": {
    "successCount": 8,
    "failureCount": 2,
    "totalProcessed": 10,
    "idempotencyKey": "batch-2024-001",
    "onConflict": "skip",
    "results": [
      {
        "index": 0,
        "success": true,
        "data": { /* created item */ },
        "errors": []
      },
      {
        "index": 1,
        "success": false,
        "data": null,
        "errors": ["SKU already exists"]
      }
    ]
  },
  "meta": {
    "operation": "batch_create_variants",
    "timestamp": "2024-01-01T10:00:00Z"
  },
  "errors": []
}
```

## Configuration

### Environment Variables
```bash
# Database
POSTGRES_DB=ProductVariantBundle
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_PORT=5432

# API
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
API_PORT=8080

# Connection String
ConnectionStrings__DefaultConnection=Host=postgres;Database=ProductVariantBundle;Username=postgres;Password=postgres123;Port=5432
```

### Application Settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Database=ProductVariantBundle;Username=postgres;Password=postgres123;Port=5432"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:8080"],
    "AllowCredentials": false,
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"],
    "AllowedHeaders": ["Content-Type", "Authorization", "X-Requested-With"]
  },
  "Api": {
    "Title": "Product Variant Bundle API",
    "Version": "v1",
    "Description": "API for managing product variants and bundles"
  },
  "Database": {
    "CommandTimeout": 30,
    "MaxRetryCount": 3,
    "MaxRetryDelay": "00:00:30",
    "EnableSensitiveDataLogging": false
  }
}
```

## Development

### Container Commands
```bash
# Build the application
docker compose exec api dotnet build

# Run tests
docker compose exec api dotnet test

# Create migration
docker compose exec api dotnet ef migrations add MigrationName

# Update database
docker compose exec api dotnet ef database update

# Access database
docker compose exec postgres psql -U postgres -d ProductVariantBundle

# View logs
docker compose logs api -f
```

### Helper Scripts
```bash
# Use the dev script for common operations
./scripts/dev.sh build
./scripts/dev.sh test
./scripts/dev.sh migration AddNewFeature
./scripts/dev.sh update-db
./scripts/dev.sh shell
./scripts/dev.sh db
```

## Testing

### Manual Testing
1. **Use Swagger UI**: http://localhost:8080
2. **Use curl**: See examples throughout this document
3. **Use Postman**: Import OpenAPI spec from http://localhost:8080/swagger/v1/swagger.json

### Unit Tests
```bash
# Run all tests
docker compose exec api dotnet test

# Run specific test project
docker compose exec api dotnet test src/ProductVariantBundle.Tests

# Run with coverage
docker compose exec api dotnet test --collect:"XPlat Code Coverage"
```

## Architecture

### Clean Architecture Layers
1. **API Layer**: Controllers, DTOs, Mappings, Middleware
2. **Core Layer**: Entities, Interfaces, Services, Business Logic
3. **Infrastructure Layer**: Repositories, Database Context, External Services

### Key Patterns
- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic encapsulation
- **DTO Pattern**: Data transfer objects for API boundaries
- **Dependency Injection**: Loose coupling and testability
- **CQRS-like**: Separate read and write operations where beneficial

## Security Considerations

### Current State
- **No Authentication**: Suitable for internal APIs or development
- **CORS Configured**: Allows cross-origin requests from specified domains
- **Input Validation**: Comprehensive validation on all endpoints
- **SQL Injection Protection**: Entity Framework parameterized queries

### Production Recommendations
- Implement authentication (JWT, OAuth2, etc.)
- Add authorization policies
- Enable HTTPS
- Implement rate limiting
- Add request logging and monitoring
- Use secrets management for connection strings

## Performance

### Database Optimizations
- **JSONB Indexing**: Efficient queries on flexible attributes
- **Connection Pooling**: Optimized database connections
- **Query Optimization**: Efficient Entity Framework queries
- **Batch Operations**: Reduced database round trips

### API Optimizations
- **Pagination**: Limit response sizes
- **Filtering**: Reduce data transfer
- **Caching Headers**: Enable client-side caching
- **Compression**: Reduce payload sizes

## Monitoring and Observability

### Health Checks
- **API Health**: `/api/health` endpoint
- **Database Health**: Connection and query validation
- **Dependency Health**: External service checks

### Logging
- **Structured Logging**: JSON format with correlation IDs
- **Error Tracking**: Detailed exception information
- **Performance Metrics**: Request timing and throughput
- **Business Events**: Audit trail for important operations

## MVP Limitations & Workarounds

### **Current Limitations**

#### **1. Individual Resource Management**
- ❌ **No GET/PUT/DELETE by ID** - Cannot manage single products/bundles
- **Workaround**: Use list endpoints with filters, or direct database access
- **Impact**: Limited frontend functionality, no detail pages

#### **2. Real-time Inventory**
- ❌ **No stock update endpoints** - Cannot update inventory via API
- **Workaround**: Update inventory records directly in database
- **Impact**: No real-time stock management, manual inventory updates

#### **3. Bundle Availability**
- ❌ **No availability calculation** - Cannot check if bundle can be sold
- **Workaround**: Calculate manually from component inventory
- **Impact**: No automated bundle availability, manual checking required

#### **4. Advanced Search**
- ❌ **Limited filtering** - Basic search only
- **Workaround**: Use existing filters or database queries
- **Impact**: Limited discovery features

### **Production Readiness Checklist**

#### **✅ Ready for Production**
- Data models and relationships
- Basic CRUD operations
- Batch processing
- Error handling and validation
- Database optimization

#### **🚧 Needs Development**
- Individual resource endpoints
- Real-time inventory management
- Bundle availability system
- Advanced search and filtering
- Authentication and authorization

## Troubleshooting

### Common Issues

1. **Database Connection Errors**
   ```bash
   # Check database status
   docker compose exec postgres pg_isready -U postgres
   
   # Check connection string
   docker compose logs api | grep "connection"
   ```

2. **Migration Issues**
   ```bash
   # Reset database
   docker compose down -v
   docker compose up -d postgres
   docker compose exec api dotnet ef database update
   ```

3. **API Not Responding**
   ```bash
   # Check container status
   docker compose ps
   
   # Check API logs
   docker compose logs api --tail=50
   ```

4. **Missing Endpoints (404)**
   ```bash
   # Check implemented endpoints
   curl http://localhost:8080/swagger/v1/swagger.json
   
   # Verify endpoint exists in documentation
   # Many endpoints in docs are not yet implemented
   ```

### Debug Mode
```bash
# Enable detailed logging
export ASPNETCORE_ENVIRONMENT=Development

# Enable sensitive data logging
# Set Database:EnableSensitiveDataLogging to true in appsettings.Development.json
```

## Contributing

### Code Style
- Follow .NET coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for business logic

### Pull Request Process
1. Create feature branch from main
2. Implement changes with tests
3. Update documentation
4. Submit pull request with description

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For questions or issues:
1. Check this documentation
2. Review the interactive API documentation at http://localhost:8080
3. Check the troubleshooting section
4. Create an issue in the repository

## Changelog

### v1.0.0
- Initial release
- Product and variant management
- Bundle system with availability calculation
- Inventory management with reservations
- Batch operations with idempotency
- Multi-warehouse support
- Comprehensive API documentation