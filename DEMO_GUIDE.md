# Product Variant Bundle API - Demo Guide

## Overview

This guide provides a comprehensive demonstration of the Product Variant Bundle API, showcasing key features, business logic, and complete workflows. Perfect for technical interviews and system presentations.

## Quick Start

### Prerequisites
```bash
# Ensure the system is running
docker compose up -d

# Verify API is healthy
curl http://localhost:8080/api/health

# Or run the automated demo setup script
./scripts/demo-setup.sh
```

### Demo Environment
- **API Base URL**: http://localhost:8080
- **Interactive Documentation**: http://localhost:8080 (Swagger UI)
- **Database**: PostgreSQL with sample data
- **Architecture**: Clean Architecture with .NET 8

## Demo Scenarios

### Scenario 1: E-commerce Product Catalog
**Business Case**: Online electronics store managing laptops with multiple configurations

### Scenario 2: Bundle Promotions
**Business Case**: Holiday bundle deals with automatic availability calculation

### Scenario 3: Multi-Warehouse Inventory
**Business Case**: Distributed inventory across multiple fulfillment centers

### Scenario 4: Bulk Operations
**Business Case**: Importing large product catalogs efficiently

## Complete Demo Workflow

### Step 1: Create Product Masters

#### 1.1 Gaming Laptop Product
```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TechCorp Gaming Laptop Pro",
    "slug": "techcorp-gaming-laptop-pro",
    "description": "High-performance gaming laptop with RGB keyboard and advanced cooling system",
    "category": "Electronics",
    "attributes": {
      "brand": "TechCorp",
      "warranty": "3 years",
      "weight": "2.8kg",
      "screenSize": "15.6 inches",
      "processor": "Intel i7-12700H",
      "graphics": "NVIDIA RTX 4060"
    }
  }'
```

#### 1.2 Wireless Mouse Product
```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TechCorp Gaming Mouse Pro",
    "slug": "techcorp-gaming-mouse-pro", 
    "description": "Wireless gaming mouse with RGB lighting and programmable buttons",
    "category": "Accessories",
    "attributes": {
      "brand": "TechCorp",
      "warranty": "2 years",
      "weight": "95g",
      "dpi": "16000",
      "connectivity": "Wireless 2.4GHz + Bluetooth",
      "batteryLife": "70 hours"
    }
  }'
```

#### 1.3 Gaming Headset Product
```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TechCorp Gaming Headset Pro",
    "slug": "techcorp-gaming-headset-pro",
    "description": "7.1 surround sound gaming headset with noise cancellation",
    "category": "Accessories", 
    "attributes": {
      "brand": "TechCorp",
      "warranty": "2 years",
      "weight": "320g",
      "connectivity": "USB + 3.5mm",
      "frequency": "20Hz-20kHz",
      "microphone": "Detachable boom mic"
    }
  }'
```

### Step 2: Create Product Variants

#### 2.1 Laptop Variants (Memory + Storage combinations)
```bash
# 16GB RAM + 512GB SSD
curl -X POST http://localhost:8080/api/products/{LAPTOP_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "LAPTOP-PRO-16GB-512GB",
    "price": 1299.99,
    "optionValues": [
      {
        "optionName": "Memory",
        "optionSlug": "memory",
        "value": "16GB DDR5"
      },
      {
        "optionName": "Storage",
        "optionSlug": "storage",
        "value": "512GB NVMe SSD"
      }
    ],
    "attributes": {
      "color": "Midnight Black",
      "model": "Pro-16-512"
    }
  }'

# 32GB RAM + 1TB SSD
curl -X POST http://localhost:8080/api/products/{LAPTOP_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "LAPTOP-PRO-32GB-1TB",
    "price": 1699.99,
    "optionValues": [
      {
        "optionName": "Memory", 
        "optionSlug": "memory",
        "value": "32GB DDR5"
      },
      {
        "optionName": "Storage",
        "optionSlug": "storage", 
        "value": "1TB NVMe SSD"
      }
    ],
    "attributes": {
      "color": "Midnight Black",
      "model": "Pro-32-1TB"
    }
  }'

# 16GB RAM + 1TB SSD
curl -X POST http://localhost:8080/api/products/{LAPTOP_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "LAPTOP-PRO-16GB-1TB",
    "price": 1499.99,
    "optionValues": [
      {
        "optionName": "Memory",
        "optionSlug": "memory",
        "value": "16GB DDR5"
      },
      {
        "optionName": "Storage",
        "optionSlug": "storage",
        "value": "1TB NVMe SSD"
      }
    ],
    "attributes": {
      "color": "Midnight Black", 
      "model": "Pro-16-1TB"
    }
  }'
```

#### 2.2 Mouse Variants (Color variations)
```bash
# Black Mouse
curl -X POST http://localhost:8080/api/products/{MOUSE_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "MOUSE-PRO-BLACK",
    "price": 79.99,
    "optionValues": [
      {
        "optionName": "Color",
        "optionSlug": "color",
        "value": "Matte Black"
      }
    ],
    "attributes": {
      "rgbLighting": "16.7M colors",
      "buttons": "8 programmable"
    }
  }'

# White Mouse
curl -X POST http://localhost:8080/api/products/{MOUSE_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "MOUSE-PRO-WHITE",
    "price": 79.99,
    "optionValues": [
      {
        "optionName": "Color",
        "optionSlug": "color", 
        "value": "Arctic White"
      }
    ],
    "attributes": {
      "rgbLighting": "16.7M colors",
      "buttons": "8 programmable"
    }
  }'
```

#### 2.3 Headset Variants (Color variations)
```bash
# Black Headset
curl -X POST http://localhost:8080/api/products/{HEADSET_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "HEADSET-PRO-BLACK",
    "price": 129.99,
    "optionValues": [
      {
        "optionName": "Color",
        "optionSlug": "color",
        "value": "Stealth Black"
      }
    ],
    "attributes": {
      "ledLighting": "RGB zones",
      "cableLength": "2m braided"
    }
  }'

# White Headset  
curl -X POST http://localhost:8080/api/products/{HEADSET_ID}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "HEADSET-PRO-WHITE",
    "price": 129.99,
    "optionValues": [
      {
        "optionName": "Color",
        "optionSlug": "color",
        "value": "Pearl White"
      }
    ],
    "attributes": {
      "ledLighting": "RGB zones",
      "cableLength": "2m braided"
    }
  }'
```

### Step 3: Set Up Inventory

#### 3.1 Stock Laptop Variants
```bash
# Stock 16GB + 512GB model
curl -X PUT http://localhost:8080/api/inventory/LAPTOP-PRO-16GB-512GB/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 25,
    "reserved": 3,
    "warehouseCode": "MAIN"
  }'

# Stock 32GB + 1TB model (premium, lower stock)
curl -X PUT http://localhost:8080/api/inventory/LAPTOP-PRO-32GB-1TB/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 8,
    "reserved": 1,
    "warehouseCode": "MAIN"
  }'

# Stock 16GB + 1TB model
curl -X PUT http://localhost:8080/api/inventory/LAPTOP-PRO-16GB-1TB/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 15,
    "reserved": 2,
    "warehouseCode": "MAIN"
  }'
```

#### 3.2 Stock Accessories
```bash
# Stock mice
curl -X PUT http://localhost:8080/api/inventory/MOUSE-PRO-BLACK/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 50,
    "reserved": 5,
    "warehouseCode": "MAIN"
  }'

curl -X PUT http://localhost:8080/api/inventory/MOUSE-PRO-WHITE/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 30,
    "reserved": 2,
    "warehouseCode": "MAIN"
  }'

# Stock headsets
curl -X PUT http://localhost:8080/api/inventory/HEADSET-PRO-BLACK/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 40,
    "reserved": 4,
    "warehouseCode": "MAIN"
  }'

curl -X PUT http://localhost:8080/api/inventory/HEADSET-PRO-WHITE/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 25,
    "reserved": 1,
    "warehouseCode": "MAIN"
  }'
```

### Step 4: Create Product Bundles

#### 4.1 Complete Gaming Setup Bundle
```bash
curl -X POST http://localhost:8080/api/bundles \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Complete Gaming Setup Pro",
    "description": "Everything you need for professional gaming: laptop, mouse, and headset",
    "sku": "GAMING-SETUP-COMPLETE",
    "price": 1399.99,
    "items": [
      {
        "sellableItemId": "{LAPTOP_16GB_512GB_SELLABLE_ID}",
        "quantity": 1
      },
      {
        "sellableItemId": "{MOUSE_BLACK_SELLABLE_ID}",
        "quantity": 1
      },
      {
        "sellableItemId": "{HEADSET_BLACK_SELLABLE_ID}",
        "quantity": 1
      }
    ],
    "metadata": {
      "promotion": "Holiday Special 2024",
      "discount": "Save $110",
      "validUntil": "2024-12-31",
      "category": "Gaming Bundles"
    }
  }'
```

#### 4.2 Premium Gaming Bundle
```bash
curl -X POST http://localhost:8080/api/bundles \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Premium Gaming Setup Pro",
    "description": "Top-tier gaming setup with 32GB laptop and premium accessories",
    "sku": "GAMING-SETUP-PREMIUM",
    "price": 1799.99,
    "items": [
      {
        "sellableItemId": "{LAPTOP_32GB_1TB_SELLABLE_ID}",
        "quantity": 1
      },
      {
        "sellableItemId": "{MOUSE_WHITE_SELLABLE_ID}",
        "quantity": 1
      },
      {
        "sellableItemId": "{HEADSET_WHITE_SELLABLE_ID}",
        "quantity": 1
      }
    ],
    "metadata": {
      "promotion": "Premium Collection",
      "discount": "Save $210",
      "validUntil": "2024-12-31",
      "category": "Premium Bundles"
    }
  }'
```

#### 4.3 Accessories Bundle
```bash
curl -X POST http://localhost:8080/api/bundles \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Accessories Bundle",
    "description": "Perfect gaming accessories combo - mouse and headset",
    "sku": "GAMING-ACCESSORIES",
    "price": 179.99,
    "items": [
      {
        "sellableItemId": "{MOUSE_BLACK_SELLABLE_ID}",
        "quantity": 1
      },
      {
        "sellableItemId": "{HEADSET_BLACK_SELLABLE_ID}",
        "quantity": 1
      }
    ],
    "metadata": {
      "promotion": "Accessories Deal",
      "discount": "Save $30",
      "validUntil": "2024-12-31",
      "category": "Accessory Bundles"
    }
  }'
```

### Step 5: Demonstrate Key Features

#### 5.1 Bundle Availability Calculation
```bash
# Check complete gaming setup availability
curl -X GET "http://localhost:8080/api/bundles/{COMPLETE_BUNDLE_ID}/availability?warehouseCode=MAIN"

# Check premium bundle availability (should be limited by laptop stock)
curl -X GET "http://localhost:8080/api/bundles/{PREMIUM_BUNDLE_ID}/availability?warehouseCode=MAIN"
```

#### 5.2 Stock Reservations
```bash
# Reserve stock for an order
curl -X POST http://localhost:8080/api/inventory/LAPTOP-PRO-16GB-512GB/reserve \
  -H "Content-Type: application/json" \
  -d '{
    "quantity": 2,
    "warehouseCode": "MAIN"
  }'

# Check how this affects bundle availability
curl -X GET "http://localhost:8080/api/bundles/{COMPLETE_BUNDLE_ID}/availability?warehouseCode=MAIN"
```

#### 5.3 Filtering and Search
```bash
# Search products by name
curl -X GET "http://localhost:8080/api/products?search=gaming&sortBy=name&sortDirection=asc"

# Filter by category
curl -X GET "http://localhost:8080/api/products?category=Electronics"

# Get bundles with pagination
curl -X GET "http://localhost:8080/api/bundles?page=1&pageSize=5"
```

#### 5.4 Batch Operations
```bash
# Create multiple variants at once
curl -X POST http://localhost:8080/api/batch/variants \
  -H "Content-Type: application/json" \
  -d '{
    "idempotencyKey": "demo-batch-2024-001",
    "onConflict": "skip",
    "items": [
      {
        "productMasterId": "{LAPTOP_ID}",
        "sku": "LAPTOP-PRO-8GB-256GB",
        "price": 999.99,
        "optionValues": [
          {
            "optionName": "Memory",
            "optionSlug": "memory",
            "value": "8GB DDR5"
          },
          {
            "optionName": "Storage",
            "optionSlug": "storage",
            "value": "256GB NVMe SSD"
          }
        ],
        "attributes": {
          "color": "Midnight Black",
          "model": "Pro-8-256"
        }
      }
    ]
  }'
```

## Key Features Demonstrated

### 1. Flexible Schema Design
- **JSONB Attributes**: Products store custom attributes without schema changes
- **Extensible**: Add new properties like warranty, weight, specifications
- **Indexed**: PostgreSQL JSONB indexing for efficient queries

### 2. Global SKU Management
- **Unique Namespace**: All sellable items share the same SKU space
- **Validation**: Prevents duplicate SKUs across variants and bundles
- **Efficient Lookups**: Optimized database indexes

### 3. Virtual Bundle System
- **No Physical Inventory**: Bundles calculate availability from components
- **Real-time Updates**: Availability changes when component stock changes
- **Formula**: `min(floor((on_hand - reserved) / required_quantity))`

### 4. Advanced Inventory Management
- **Multi-Warehouse**: Support for multiple distribution centers
- **Reservations**: Reserve stock for pending orders
- **Transaction Safety**: Row-level locking for concurrent operations

### 5. Business Logic Validation
- **Variant Uniqueness**: Prevents duplicate option combinations
- **Bundle Validation**: Ensures all components exist and are active
- **SKU Constraints**: Global uniqueness across all sellable items

### 6. API Design Excellence
- **Consistent Responses**: Envelope format with data, meta, errors
- **RFC 7807 Errors**: Standardized error response format
- **Comprehensive Validation**: Detailed error messages

## Demo Presentation Flow

### 1. Architecture Overview (2 minutes)
- Clean Architecture layers
- .NET 8 + PostgreSQL + Docker
- Repository pattern and dependency injection

### 2. Core Features Demo (8 minutes)

#### Product Management
- Create product masters with flexible attributes
- Add variants with option combinations
- Show slug normalization and uniqueness

#### Bundle System
- Create bundles with multiple components
- Demonstrate availability calculation
- Show real-time updates when stock changes

#### Inventory Management
- Multi-warehouse support
- Stock reservations and releases
- Transaction safety

#### Batch Operations
- Bulk variant creation
- Idempotency and conflict resolution
- Performance benefits

### 3. Business Logic Highlights (3 minutes)
- Global SKU uniqueness validation
- Variant combination uniqueness
- Bundle availability algorithm
- Error handling and validation

### 4. API Design (2 minutes)
- Consistent response format
- Comprehensive error responses
- Interactive documentation

## Testing Scenarios

### Scenario A: Happy Path
1. Create products → variants → bundles → inventory
2. Check availability → reserve stock → release stock
3. Demonstrate filtering and pagination

### Scenario B: Validation Testing
1. Try duplicate SKUs (should fail)
2. Try duplicate variant combinations (should fail)
3. Try invalid bundle components (should fail)

### Scenario C: Edge Cases
1. Bundle with out-of-stock components
2. Concurrent stock reservations
3. Batch operations with conflicts

## Performance Highlights

### Database Optimizations
- JSONB indexing for flexible attributes
- Optimized queries with proper joins
- Connection pooling and query optimization

### API Performance
- Pagination to limit response sizes
- Efficient filtering and sorting
- Batch operations reduce round trips

## Production Readiness

### Current Features
- Comprehensive validation
- Error handling and logging
- Health checks
- Docker containerization

### Production Considerations
- Add authentication/authorization
- Implement rate limiting
- Add monitoring and observability
- Enable HTTPS and security headers

## Conclusion

This API demonstrates:
- **Clean Architecture**: Maintainable and testable code
- **Business Logic**: Complex inventory and bundle calculations
- **API Design**: RESTful, consistent, well-documented
- **Database Design**: Flexible schema with strong constraints
- **Performance**: Optimized queries and batch operations

Perfect for e-commerce platforms, inventory management systems, and any application requiring flexible product catalogs with complex business rules.