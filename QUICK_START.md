# Quick Start Guide

Get the Product Variant Bundle API up and running in minutes!

## 🚀 One-Command Setup

```bash
# Start the system and set up demo data
docker compose up -d && ./scripts/demo-setup.sh
```

## 📋 Step-by-Step Setup

### 1. Start the System
```bash
docker compose up -d
```

### 2. Verify Everything is Running
```bash
./scripts/test-api.sh
```

### 3. Set Up Demo Data (Optional)
```bash
./scripts/demo-setup.sh
```

### 4. Explore the API
- **Swagger UI**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/health
- **Products**: http://localhost:8080/api/products
- **Bundles**: http://localhost:8080/api/bundles

## 🎯 Quick Demo Commands

### View Products
```bash
curl http://localhost:8080/api/products
```

### Search Products
```bash
curl "http://localhost:8080/api/products?search=gaming"
```

### View Bundles
```bash
curl http://localhost:8080/api/bundles
```

### Check Bundle Availability
```bash
# Get bundle ID from bundles list, then:
curl "http://localhost:8080/api/bundles/{bundle-id}/availability?warehouseCode=MAIN"
```

## 🧪 Test Key Features

### 1. Create a Product
```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "My Test Product",
    "slug": "my-test-product",
    "category": "Test",
    "description": "A test product"
  }'
```

### 2. Create a Variant
```bash
# Use product ID from step 1
curl -X POST http://localhost:8080/api/products/{product-id}/variants \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "TEST-SKU-001",
    "price": 99.99,
    "optionValues": [
      {
        "optionName": "Color",
        "optionSlug": "color",
        "value": "Blue"
      }
    ]
  }'
```

### 3. Set Inventory
```bash
curl -X PUT http://localhost:8080/api/inventory/TEST-SKU-001/stock \
  -H "Content-Type: application/json" \
  -d '{
    "onHand": 10,
    "reserved": 0,
    "warehouseCode": "MAIN"
  }'
```

### 4. Create a Bundle
```bash
# Use sellable item ID from variant creation
curl -X POST http://localhost:8080/api/bundles \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Bundle",
    "sku": "BUNDLE-001",
    "price": 89.99,
    "description": "A test bundle",
    "items": [
      {
        "sellableItemId": "{sellable-item-id}",
        "quantity": 1
      }
    ]
  }'
```

## 🔧 Troubleshooting

### API Not Responding
```bash
# Check container status
docker compose ps

# Check logs
docker compose logs api

# Restart
docker compose restart
```

### Reset Everything
```bash
# Stop and remove all data
docker compose down -v

# Start fresh
docker compose up -d

# Set up demo data again
./scripts/demo-setup.sh
```

## 📚 Next Steps

- **Full Demo Guide**: See [DEMO_GUIDE.md](DEMO_GUIDE.md)
- **API Documentation**: See [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Test Scenarios**: See [TEST_SCENARIOS.md](TEST_SCENARIOS.md)
- **Development Guide**: See [DEVELOPMENT.md](DEVELOPMENT.md)

## 🎬 Demo Scenarios

After running the demo setup, you can demonstrate:

1. **Product Catalog Management**
   - Flexible product attributes (JSONB)
   - Variant combinations and uniqueness
   - Global SKU management

2. **Bundle System**
   - Virtual bundle inventory
   - Real-time availability calculation
   - Component-level stock tracking

3. **Business Logic**
   - Validation and error handling
   - Batch operations with idempotency
   - Multi-warehouse inventory

4. **API Design**
   - Consistent response format
   - Comprehensive error responses
   - Search, filtering, and pagination

## ✨ Key Features Demonstrated

- **Clean Architecture**: Layered design with SOLID principles
- **Flexible Schema**: JSONB attributes without migrations
- **Complex Business Logic**: Bundle availability calculations
- **Data Integrity**: Comprehensive validation and constraints
- **Performance**: Optimized queries and batch operations
- **API Excellence**: RESTful design with proper error handling

Ready to impress! 🚀