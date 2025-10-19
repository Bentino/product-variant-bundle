#!/bin/bash

# Product Variant Bundle API - Distribution Package Creator
# This script creates a distribution package with pre-built Docker image

set -e

echo "🚀 Creating Product Variant Bundle API Distribution Package..."
echo "=============================================================="

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi

echo -e "${BLUE}Step 1: Building production Docker image...${NC}"
docker build -t product-variant-bundle-api:latest --target final .

echo -e "${BLUE}Step 2: Exporting Docker image...${NC}"
docker save -o product-variant-bundle-api.tar product-variant-bundle-api:latest

echo -e "${BLUE}Step 3: Creating distribution directory...${NC}"
rm -rf distribution
mkdir -p distribution

# Copy necessary files
cp product-variant-bundle-api.tar distribution/
cp docker-compose.simple.yml distribution/
cp -r scripts distribution/
cp QUICK_START.md distribution/
cp Product_Variant_Bundle_API.postman_collection.json distribution/
cp .env.example distribution/

# Create distribution-specific README
cat > distribution/README.md << 'EOF'
# Product Variant Bundle API - Distribution Package

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- No other dependencies required

### Setup Steps

1. **Load the Docker image:**
   ```bash
   docker load -i product-variant-bundle-api.tar
   ```

2. **Start the services:**
   ```bash
   docker compose -f docker-compose.simple.yml up -d
   ```

3. **Wait for services to be ready (about 30 seconds):**
   ```bash
   # Check if API is ready
   curl http://localhost:8080/api/health
   ```

4. **Set up demo data (optional):**
   ```bash
   ./scripts/demo-setup.sh
   ```

5. **Test the API:**
   ```bash
   ./scripts/test-api.sh
   ```

### 🌐 Access Points

- **Swagger UI**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/health
- **API Base**: http://localhost:8080/api

### 📋 What's Included

- **Pre-built API Docker image** - No compilation needed
- **PostgreSQL database** - Automatically configured
- **Demo data setup** - Sample products, variants, and bundles
- **API testing script** - Comprehensive validation
- **Postman collection** - Ready-to-import API tests

### 🧪 Testing the API

#### Quick Health Check
```bash
curl http://localhost:8080/api/health
```

#### View Sample Data (after demo setup)
```bash
# List products
curl http://localhost:8080/api/products

# List bundles
curl http://localhost:8080/api/bundles

# Search products
curl "http://localhost:8080/api/products?search=gaming"
```

#### Import Postman Collection
1. Open Postman
2. Import `Product_Variant_Bundle_API.postman_collection.json`
3. Set base URL to `http://localhost:8080`

### 🔧 Troubleshooting

#### Services not starting
```bash
# Check container status
docker compose -f docker-compose.simple.yml ps

# View logs
docker compose -f docker-compose.simple.yml logs api
docker compose -f docker-compose.simple.yml logs postgres
```

#### Port conflicts
If port 8080 is already in use, edit `docker-compose.simple.yml`:
```yaml
api:
  ports:
    - "8081:8080"  # Change to available port
```

#### Reset everything
```bash
# Stop and remove all containers and data
docker compose -f docker-compose.simple.yml down -v

# Start fresh
docker compose -f docker-compose.simple.yml up -d
```

### 📚 Additional Documentation

- **QUICK_START.md** - Detailed setup guide
- **Postman Collection** - Complete API testing suite

### 🏗️ Architecture Overview

This is a .NET 8 Web API built with:
- **Clean Architecture** - Separation of concerns
- **Entity Framework Core** - Data access
- **PostgreSQL** - Database
- **Docker** - Containerization
- **Swagger/OpenAPI** - API documentation

The API manages product catalogs with variants and bundles, featuring:
- Flexible product attributes (JSONB)
- Complex variant management
- Bundle availability calculation
- Inventory tracking
- Comprehensive validation

---

**Need the source code?** This distribution contains only the pre-built application. For development access, request the full source code repository.
EOF

echo -e "${BLUE}Step 4: Creating distribution archive...${NC}"
tar -czf product-variant-bundle-distribution.tar.gz distribution/

# Get file sizes
IMAGE_SIZE=$(du -h product-variant-bundle-api.tar | cut -f1)
ARCHIVE_SIZE=$(du -h product-variant-bundle-distribution.tar.gz | cut -f1)

echo -e "${GREEN}✅ Distribution package created successfully!${NC}"
echo ""
echo "📦 Package Details:"
echo "  • Docker image size: $IMAGE_SIZE"
echo "  • Distribution archive: $ARCHIVE_SIZE"
echo "  • Archive file: product-variant-bundle-distribution.tar.gz"
echo ""
echo "📋 Distribution Contents:"
echo "  • Pre-built Docker image"
echo "  • Simple Docker Compose file"
echo "  • Demo setup script"
echo "  • API testing script"
echo "  • Postman collection"
echo "  • Setup documentation"
echo ""
echo -e "${YELLOW}📤 To share this project:${NC}"
echo "  1. Send: product-variant-bundle-distribution.tar.gz"
echo "  2. Recipient extracts and follows README.md"
echo "  3. No source code or build tools needed!"
echo ""
echo -e "${BLUE}🧪 To test the distribution locally:${NC}"
echo "  cd distribution"
echo "  docker load -i product-variant-bundle-api.tar"
echo "  docker compose -f docker-compose.simple.yml up -d"

# Cleanup
rm product-variant-bundle-api.tar

echo ""
echo -e "${GREEN}🎉 Ready for distribution!${NC}"