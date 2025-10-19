#!/bin/bash

# Product Variant Bundle API - Comprehensive Manual Testing Script
# This script tests all major API functionality

BASE_URL="http://localhost:3000"
TEST_LOG="manual-api-test-results.log"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counter
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Logging function
log() {
    echo -e "$1" | tee -a "$TEST_LOG"
}

# Test function
run_test() {
    local test_name="$1"
    local curl_command="$2"
    local expected_status="$3"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    log "${BLUE}[TEST $TOTAL_TESTS] $test_name${NC}"
    log "Command: $curl_command"
    
    # Execute the curl command and capture response
    response=$(eval "$curl_command" 2>&1)
    status_code=$(echo "$response" | tail -n1)
    
    if [[ "$status_code" == "$expected_status" ]]; then
        log "${GREEN}✓ PASSED${NC}"
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        log "${RED}✗ FAILED - Expected: $expected_status, Got: $status_code${NC}"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    
    log "Response: $response"
    log "---"
}

# Initialize log file
echo "Product Variant Bundle API - Manual Testing Results" > "$TEST_LOG"
echo "Test Date: $(date)" >> "$TEST_LOG"
echo "Base URL: $BASE_URL" >> "$TEST_LOG"
echo "=========================================" >> "$TEST_LOG"

log "${YELLOW}Starting Comprehensive API Testing...${NC}"

# 1. HEALTH CHECK TESTS
log "${YELLOW}=== 1. HEALTH CHECK TESTS ===${NC}"

run_test "API Health Check" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/health'" \
    "200"

# 2. PRODUCT MASTER CRUD TESTS
log "${YELLOW}=== 2. PRODUCT MASTER CRUD TESTS ===${NC}"

# Create a product master
PRODUCT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
    -H "Content-Type: application/json" \
    -d '{
        "name": "Test Gaming Laptop",
        "slug": "test-gaming-laptop",
        "description": "High-performance gaming laptop for testing",
        "category": "Electronics",
        "attributes": {
            "brand": "TestCorp",
            "warranty": "2 years",
            "weight": "2.5kg"
        }
    }')

PRODUCT_ID=$(echo "$PRODUCT_RESPONSE" | jq -r '.data.id // empty')

if [[ -n "$PRODUCT_ID" && "$PRODUCT_ID" != "null" ]]; then
    log "${GREEN}✓ Product created successfully with ID: $PRODUCT_ID${NC}"
    PASSED_TESTS=$((PASSED_TESTS + 1))
else
    log "${RED}✗ Failed to create product${NC}"
    log "Response: $PRODUCT_RESPONSE"
    FAILED_TESTS=$((FAILED_TESTS + 1))
fi
TOTAL_TESTS=$((TOTAL_TESTS + 1))

# Get the created product
if [[ -n "$PRODUCT_ID" ]]; then
    run_test "Get Product by ID" \
        "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products/$PRODUCT_ID'" \
        "200"
fi

# List products
run_test "List Products" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products'" \
    "200"

# 3. PRODUCT VARIANT TESTS
log "${YELLOW}=== 3. PRODUCT VARIANT TESTS ===${NC}"

if [[ -n "$PRODUCT_ID" ]]; then
    # Create a variant
    VARIANT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$PRODUCT_ID/variants" \
        -H "Content-Type: application/json" \
        -d '{
            "productMasterId": "'$PRODUCT_ID'",
            "sku": "TEST-LAPTOP-16GB-512GB",
            "price": 1299.99,
            "optionValues": [
                {
                    "optionName": "Memory",
                    "optionSlug": "memory",
                    "value": "16GB"
                },
                {
                    "optionName": "Storage",
                    "optionSlug": "storage",
                    "value": "512GB SSD"
                }
            ],
            "attributes": {
                "color": "Black",
                "model": "Test-16"
            }
        }')
    
    VARIANT_ID=$(echo "$VARIANT_RESPONSE" | jq -r '.data.id // empty')
    SELLABLE_ITEM_ID=$(echo "$VARIANT_RESPONSE" | jq -r '.data.sellableItem.id // empty')
    
    if [[ -n "$VARIANT_ID" && "$VARIANT_ID" != "null" ]]; then
        log "${GREEN}✓ Variant created successfully with ID: $VARIANT_ID${NC}"
        log "Sellable Item ID: $SELLABLE_ITEM_ID"
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        log "${RED}✗ Failed to create variant${NC}"
        log "Response: $VARIANT_RESPONSE"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    # Get variants for the product
    run_test "Get Product Variants" \
        "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products/$PRODUCT_ID/variants'" \
        "200"
fi

# 4. BUNDLE MANAGEMENT TESTS
log "${YELLOW}=== 4. BUNDLE MANAGEMENT TESTS ===${NC}"

if [[ -n "$SELLABLE_ITEM_ID" ]]; then
    # Create a bundle
    BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
        -H "Content-Type: application/json" \
        -d '{
            "name": "Test Gaming Bundle",
            "description": "Test bundle with gaming laptop",
            "sku": "TEST-GAMING-BUNDLE-001",
            "price": 1499.99,
            "items": [
                {
                    "sellableItemId": "'$SELLABLE_ITEM_ID'",
                    "quantity": 1
                }
            ],
            "metadata": {
                "promotion": "Test Promotion",
                "validUntil": "2024-12-31"
            }
        }')
    
    BUNDLE_ID=$(echo "$BUNDLE_RESPONSE" | jq -r '.data.id // empty')
    
    if [[ -n "$BUNDLE_ID" && "$BUNDLE_ID" != "null" ]]; then
        log "${GREEN}✓ Bundle created successfully with ID: $BUNDLE_ID${NC}"
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        log "${RED}✗ Failed to create bundle${NC}"
        log "Response: $BUNDLE_RESPONSE"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    # Get bundle availability
    if [[ -n "$BUNDLE_ID" ]]; then
        run_test "Get Bundle Availability" \
            "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/bundles/$BUNDLE_ID/availability'" \
            "200"
    fi
fi

# List bundles
run_test "List Bundles" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/bundles'" \
    "200"

# 5. INVENTORY MANAGEMENT TESTS
log "${YELLOW}=== 5. INVENTORY MANAGEMENT TESTS ===${NC}"

if [[ -n "$SELLABLE_ITEM_ID" ]]; then
    # Update inventory
    run_test "Update Inventory Stock" \
        "curl -s -w '%{http_code}' -o /dev/null -X PUT '$BASE_URL/api/inventory/TEST-LAPTOP-16GB-512GB/stock' -H 'Content-Type: application/json' -d '{\"sku\":\"TEST-LAPTOP-16GB-512GB\",\"onHand\":50,\"reserved\":5,\"warehouseCode\":\"MAIN\"}'" \
        "200"
    
    # Get inventory
    run_test "Get Inventory Record" \
        "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/inventory/TEST-LAPTOP-16GB-512GB'" \
        "200"
    
    # Reserve stock
    run_test "Reserve Stock" \
        "curl -s -w '%{http_code}' -o /dev/null -X POST '$BASE_URL/api/inventory/TEST-LAPTOP-16GB-512GB/reserve' -H 'Content-Type: application/json' -d '{\"sku\":\"TEST-LAPTOP-16GB-512GB\",\"quantity\":2,\"warehouseCode\":\"MAIN\"}'" \
        "200"
fi

# 6. FILTERING AND PAGINATION TESTS
log "${YELLOW}=== 6. FILTERING AND PAGINATION TESTS ===${NC}"

run_test "Filter Products by Category" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products?category=Electronics'" \
    "200"

run_test "Paginated Products" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products?page=1&pageSize=5'" \
    "200"

run_test "Search Products" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products?search=gaming'" \
    "200"

# 7. ERROR SCENARIO TESTS
log "${YELLOW}=== 7. ERROR SCENARIO TESTS ===${NC}"

# Test duplicate SKU
run_test "Create Variant with Duplicate SKU (should fail)" \
    "curl -s -w '%{http_code}' -o /dev/null -X POST '$BASE_URL/api/products/$PRODUCT_ID/variants' -H 'Content-Type: application/json' -d '{\"productMasterId\":\"$PRODUCT_ID\",\"sku\":\"TEST-LAPTOP-16GB-512GB\",\"price\":999.99,\"optionValues\":[{\"optionName\":\"Memory\",\"optionSlug\":\"memory\",\"value\":\"8GB\"}]}'" \
    "400"

# Test invalid product ID
run_test "Get Non-existent Product (should fail)" \
    "curl -s -w '%{http_code}' -o /dev/null '$BASE_URL/api/products/00000000-0000-0000-0000-000000000000'" \
    "404"

# Test invalid bundle creation
run_test "Create Bundle with Invalid Item (should fail)" \
    "curl -s -w '%{http_code}' -o /dev/null -X POST '$BASE_URL/api/bundles' -H 'Content-Type: application/json' -d '{\"name\":\"Invalid Bundle\",\"sku\":\"INVALID-BUNDLE\",\"price\":100,\"items\":[{\"sellableItemId\":\"00000000-0000-0000-0000-000000000000\",\"quantity\":1}]}'" \
    "400"

# Summary
log "${YELLOW}=== TEST SUMMARY ===${NC}"
log "Total Tests: $TOTAL_TESTS"
log "${GREEN}Passed: $PASSED_TESTS${NC}"
log "${RED}Failed: $FAILED_TESTS${NC}"

if [[ $FAILED_TESTS -eq 0 ]]; then
    log "${GREEN}🎉 All tests passed!${NC}"
    exit 0
else
    log "${RED}❌ Some tests failed. Check the log for details.${NC}"
    exit 1
fi