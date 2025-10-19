# Deployment & Distribution Guide

This guide explains how to share and deploy the Product Variant Bundle API project.

## 🎯 Distribution Options

### Option 1: Source Code Sharing (Recommended for Development)

**Best for:** Code review, development collaboration, full project access

**What to share:**
```
project-folder/
├── src/                    # Source code
├── scripts/               # Helper scripts
├── docker-compose.yml     # Development compose file
├── Dockerfile            # Build instructions
├── .env.example          # Environment template
├── README.md             # Setup instructions
└── ... (all project files)
```

**Recipient setup:**
1. Clone/extract the project
2. Copy `.env.example` to `.env`
3. Run `docker compose up -d`
4. Run `./scripts/demo-setup.sh` (optional)

### Option 2: Pre-built Image Sharing (For Testing/Demo)

**Best for:** Quick testing, demos, when source code access isn't needed

**Steps to create distribution package:**

1. **Build production image:**
   ```bash
   docker build -t product-variant-bundle-api:latest --target final .
   ```

2. **Export image:**
   ```bash
   docker save -o product-variant-bundle-api.tar product-variant-bundle-api:latest
   ```

3. **Create distribution package:**
   ```
   distribution-package/
   ├── product-variant-bundle-api.tar    # Docker image
   ├── docker-compose.simple.yml         # Simple compose file
   ├── scripts/
   │   ├── demo-setup.sh                 # Demo data script
   │   └── test-api.sh                   # API testing script
   ├── QUICK_START.md                    # Quick setup guide
   └── README-DISTRIBUTION.md            # Distribution-specific instructions
   ```

**Recipient setup:**
1. Load the Docker image: `docker load -i product-variant-bundle-api.tar`
2. Start services: `docker compose -f docker-compose.simple.yml up -d`
3. Run demo setup: `./scripts/demo-setup.sh`

## 📦 Creating Distribution Packages

### For Code Review/Development

```bash
# Create source distribution
git archive --format=tar.gz --output=product-variant-bundle-source.tar.gz HEAD

# Or simply share the Git repository URL: git@github.com:Bentino/product-variant-bundle.git
```

### For Testing/Demo

```bash
#!/bin/bash
# create-distribution.sh

echo "Creating distribution package..."

# Build production image
docker build -t product-variant-bundle-api:latest --target final .

# Export image
docker save -o product-variant-bundle-api.tar product-variant-bundle-api:latest

# Create distribution directory
mkdir -p distribution
cp product-variant-bundle-api.tar distribution/
cp docker-compose.simple.yml distribution/
cp -r scripts distribution/
cp QUICK_START.md distribution/
cp Product_Variant_Bundle_API.postman_collection.json distribution/

# Create distribution README
cat > distribution/README-DISTRIBUTION.md << 'EOF'
# Product Variant Bundle API - Distribution Package

## Quick Start

1. Load Docker image:
   ```bash
   docker load -i product-variant-bundle-api.tar
   ```

2. Start services:
   ```bash
   docker compose -f docker-compose.simple.yml up -d
   ```

3. Set up demo data:
   ```bash
   ./scripts/demo-setup.sh
   ```

4. Test API:
   ```bash
   ./scripts/test-api.sh
   ```

5. Access Swagger UI: http://localhost:8080

## What's Included

- Pre-built Docker image
- PostgreSQL database
- Demo data setup script
- API testing script
- Postman collection

## Troubleshooting

- Check containers: `docker compose ps`
- View logs: `docker compose logs api`
- Reset: `docker compose down -v && docker compose up -d`
EOF

# Create archive
tar -czf product-variant-bundle-distribution.tar.gz distribution/

echo "Distribution package created: product-variant-bundle-distribution.tar.gz"
echo "Image size: $(du -h product-variant-bundle-api.tar | cut -f1)"
```

## 🚀 Container Registry Deployment

### Push to Registry

```bash
# Tag for registry
docker tag product-variant-bundle-api:latest your-registry.com/product-variant-bundle-api:v1.0.0

# Push to registry
docker push your-registry.com/product-variant-bundle-api:v1.0.0
```

### Production Docker Compose

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - app-network

  api:
    image: your-registry.com/product-variant-bundle-api:v1.0.0
    ports:
      - "80:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
    depends_on:
      - postgres
    networks:
      - app-network
    restart: unless-stopped

networks:
  app-network:
volumes:
  postgres_data:
```

## 📋 Distribution Checklist

### Before Sharing Source Code:
- [ ] Remove sensitive data from `.env`
- [ ] Include `.env.example` with default values
- [ ] Update README.md with setup instructions
- [ ] Test setup from scratch
- [ ] Include all necessary scripts

### Before Sharing Pre-built Image:
- [ ] Build production image
- [ ] Test image with simple compose file
- [ ] Include demo setup scripts
- [ ] Create distribution README
- [ ] Test complete distribution package

### For Production Deployment:
- [ ] Use production environment variables
- [ ] Configure proper database credentials
- [ ] Set up SSL/TLS certificates
- [ ] Configure logging and monitoring
- [ ] Set up backup procedures

## 🔧 Troubleshooting Common Issues

### Image Size Too Large
```bash
# Use multi-stage build optimization
docker build --target final -t product-variant-bundle-api:slim .

# Check image size
docker images product-variant-bundle-api
```

### Port Conflicts
```bash
# Change ports in docker-compose.yml
ports:
  - "8081:8080"  # Use different host port
```

### Database Connection Issues
```bash
# Check network connectivity
docker compose exec api ping postgres

# Check environment variables
docker compose exec api env | grep ConnectionStrings
```