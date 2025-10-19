-- Initialize database schema for Product Variant Bundle system
-- This script will be executed when PostgreSQL container starts

-- Create database if not exists (handled by POSTGRES_DB environment variable)

-- Basic health check
SELECT 'Database initialized successfully' as status;