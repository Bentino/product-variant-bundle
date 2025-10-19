# Scripts Directory

This directory contains utility scripts for the Product Variant Bundle API project.

## Available Scripts

### 🚀 demo-setup.sh
**Purpose**: Creates comprehensive sample data for demonstration and testing

**Usage**:
```bash
./scripts/demo-setup.sh
```

**What it creates**:
- 3 Product Masters (Gaming Laptop, Mouse, Headset)
- 7 Product Variants with different configurations
- 3 Product Bundles (Complete, Premium, Accessories)
- Realistic inventory levels with stock reservations
- Sample data for all major API features

**Prerequisites**: API must be running (`docker compose up -d`)

### 🧪 test-api.sh
**Purpose**: Runs quick validation tests on API endpoints

**Usage**:
```bash
./scripts/test-api.sh
```

### 📦 create-distribution.sh
**Purpose**: Creates a distribution package with pre-built Docker image for sharing

**Usage**:
```bash
./scripts/create-distribution.sh
```

**What it creates**:
- Pre-built Docker image (no source code needed)
- Simple docker-compose.yml for easy deployment
- Complete documentation for recipients
- Demo setup and testing scripts
- Distribution archive ready for sharing

**What it tests**:
- API health checks
- Basic CRUD operations
- Search and filtering
- Error handling (validation, 404)
- Response format validation

**Output**: Pass/fail results with success rate

### 🔧 dev.sh (Existing)
**Purpose**: Development helper commands for container operations

**Usage**:
```bash
./scripts/dev.sh <command>
```

**Available commands**:
- `build` - Build the application
- `test` - Run unit tests
- `migration <name>` - Create EF migration
- `update-db` - Update database
- `shell` - Open container shell
- `db` - Open database shell

## Quick Start Workflow

1. **Start the system**:
   ```bash
   docker compose up -d
   ```

2. **Set up demo data**:
   ```bash
   ./scripts/demo-setup.sh
   ```

3. **Validate everything works**:
   ```bash
   ./scripts/test-api.sh
   ```

4. **Start exploring**:
   - API Documentation: http://localhost:8080
   - Health Check: http://localhost:8080/api/health
   - Products: http://localhost:8080/api/products
   - Bundles: http://localhost:8080/api/bundles

## Demo Scenarios

After running `demo-setup.sh`, you can demonstrate:

### 1. Product Catalog Management
```bash
# View all products
curl http://localhost:8080/api/products

# Search for gaming products
curl "http://localhost:8080/api/products?search=gaming"

# Filter by category
curl "http://localhost:8080/api/products?category=Electronics"
```

### 2. Bundle Availability Calculation
```bash
# Get bundle availability (replace {bundle-id} with actual ID from demo-setup output)
curl "http://localhost:8080/api/bundles/{bundle-id}/availability?warehouseCode=MAIN"
```

### 3. Inventory Management
```bash
# Check stock levels
curl http://localhost:8080/api/inventory/LAPTOP-PRO-16GB-512GB

# Reserve stock
curl -X POST http://localhost:8080/api/inventory/LAPTOP-PRO-16GB-512GB/reserve \
  -H "Content-Type: application/json" \
  -d '{"quantity": 1, "warehouseCode": "MAIN"}'
```

### 4. Error Handling
```bash
# Try creating duplicate product
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","slug":"techcorp-gaming-laptop-pro"}'
```

## Troubleshooting

### API Not Responding
```bash
# Check container status
docker compose ps

# Check API logs
docker compose logs api

# Restart services
docker compose restart
```

### Database Issues
```bash
# Check database logs
docker compose logs postgres

# Reset database (WARNING: destroys all data)
docker compose down -v
docker compose up -d
```

### Permission Issues
```bash
# Make scripts executable
chmod +x scripts/*.sh
```

## Integration with Documentation

These scripts work together with the project documentation:

- **DEMO_GUIDE.md**: Comprehensive demo walkthrough
- **API_DOCUMENTATION.md**: Complete API reference with examples
- **TEST_SCENARIOS.md**: Complete test scenarios
- **README.md**: Main project documentation

## Development Workflow

For active development:

1. **Make changes to code**
2. **Build and test**:
   ```bash
   ./scripts/dev.sh build
   ./scripts/dev.sh test
   ```
3. **Reset demo data**:
   ```bash
   ./scripts/demo-setup.sh
   ```
4. **Validate changes**:
   ```bash
   ./scripts/test-api.sh
   ```

This ensures your changes work with the complete demo scenario.