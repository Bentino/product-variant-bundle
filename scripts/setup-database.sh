#!/bin/bash

echo "Starting PostgreSQL with Docker Compose..."
docker compose up -d postgres

echo "Waiting for PostgreSQL to be ready..."
sleep 10

echo "Applying database migrations..."
# Apply migrations using SQL scripts (more reliable for Docker)
echo "Creating database tables..."

# Create ProductMasters table
docker compose exec postgres psql -U postgres -d ProductVariantBundle -c "
CREATE TABLE IF NOT EXISTS \"ProductMasters\" (
    \"Id\" uuid NOT NULL,
    \"Name\" character varying(255) NOT NULL,
    \"Slug\" character varying(255) NOT NULL,
    \"Description\" text NOT NULL,
    \"Category\" character varying(100) NOT NULL,
    \"Attributes\" jsonb,
    \"CreatedAt\" timestamp with time zone NOT NULL,
    \"UpdatedAt\" timestamp with time zone NOT NULL,
    \"Status\" integer NOT NULL,
    CONSTRAINT \"PK_ProductMasters\" PRIMARY KEY (\"Id\")
);"

# Create other essential tables
docker compose exec postgres psql -U postgres -d ProductVariantBundle -c "
CREATE TABLE IF NOT EXISTS \"VariantOptions\" (
    \"Id\" uuid NOT NULL,
    \"Name\" character varying(100) NOT NULL,
    \"DisplayName\" character varying(100) NOT NULL,
    \"CreatedAt\" timestamp with time zone NOT NULL,
    \"UpdatedAt\" timestamp with time zone NOT NULL,
    \"Status\" integer NOT NULL,
    CONSTRAINT \"PK_VariantOptions\" PRIMARY KEY (\"Id\")
);

CREATE TABLE IF NOT EXISTS \"Warehouses\" (
    \"Id\" uuid NOT NULL,
    \"Code\" character varying(50) NOT NULL,
    \"Name\" character varying(255) NOT NULL,
    \"Address\" text NOT NULL,
    \"IsDefault\" boolean NOT NULL,
    \"CreatedAt\" timestamp with time zone NOT NULL,
    \"UpdatedAt\" timestamp with time zone NOT NULL,
    \"Status\" integer NOT NULL,
    CONSTRAINT \"PK_Warehouses\" PRIMARY KEY (\"Id\")
);"

echo "Database tables created successfully!"

echo "Database setup completed!"
echo "PostgreSQL is running on localhost:5432"
echo "Database: ProductVariantBundle"
echo "Username: postgres"
echo "Password: postgres123"