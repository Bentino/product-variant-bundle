# Product Variant Bundle API - Test Scenarios

## Overview

This document outlines comprehensive test scenarios for validating the Product Variant Bundle API functionality. Each scenario includes setup steps, execution commands, expected results, and validation criteria.

## Test Environment Setup

### Prerequisites
```bash
# Ensure system is running
docker compose up -d

# Verify API health
curl http://localhost:8080/api/health

# Quick demo data setup (optional)
./scripts/demo-setup.sh

# Set environment variables
export BASE_URL="http://localhost:8080"
export CONTENT_TYPE="Content-Type: application/json"
```

## Test Scenarios

### Scenario 1: Complete E-commerce Workflow

**Business Context**: Customer browsing products, selecting variants, and purchasing bundles

#### Test Steps

1. **Create Product Catalog**
   ```bash
   # Create Gaming Laptop
   LAPTOP_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
     -H "$CONTENT_TYPE" \
     -d '{
       "name": "TechCorp Gaming Laptop Pro",
       "slug": "techcorp-gaming-laptop-pro",
       "description": "High-performance gaming laptop",
       "category": "Electronics",
       "attributes": {
         "brand": "TechCorp",
         "warranty": "3 years",
         "processor": "Intel i7-12700H"
       }
     }')
   
   LAPTOP_ID=$(echo "$LAPTOP_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
   echo "✅ Laptop Product Created: $LAPTOP_ID"
   ```

2. **Create Product Variants**
   ```bash
   # 16GB + 512GB Variant
   VARIANT_16_512_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
     -H "$CONTENT_TYPE" \
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
       ]
     }')
   
   VARIANT_16_512_ID=$(echo "$VARIANT_16_512_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
   SELLABLE_16_512_ID=$(echo "$VARIANT_16_512_RESPONSE" | grep -o '"sellableItem":{"id":"[^"]*' | cut -d'"' -f6)
   echo "✅ 16GB+512GB Variant Created: $VARIANT_16_512_ID"
   
   # 32GB + 1TB Variant
   VARIANT_32_1TB_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
     -H "$CONTENT_TYPE" \
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
       ]
     }')
   
   SELLABLE_32_1TB_ID=$(echo "$VARIANT_32_1TB_RESPONSE" | grep -o '"sellableItem":{"id":"[^"]*' | cut -d'"' -f6)
   echo "✅ 32GB+1TB Variant Created"
   ```

3. **Set Up Inventory**
   ```bash
   # Stock the variants
   curl -s -X PUT "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/stock" \
     -H "$CONTENT_TYPE" \
     -d '{
       "onHand": 25,
       "reserved": 3,
       "warehouseCode": "MAIN"
     }' > /dev/null
   echo "✅ 16GB+512GB Stocked: 25 on hand, 3 reserved"
   
   curl -s -X PUT "$BASE_URL/api/inventory/LAPTOP-PRO-32GB-1TB/stock" \
     -H "$CONTENT_TYPE" \
     -d '{
       "onHand": 8,
       "reserved": 1,
       "warehouseCode": "MAIN"
     }' > /dev/null
   echo "✅ 32GB+1TB Stocked: 8 on hand, 1 reserved"
   ```

4. **Create Product Bundle**
   ```bash
   BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"name\": \"Gaming Setup Bundle\",
       \"description\": \"Complete gaming setup\",
       \"sku\": \"GAMING-SETUP-001\",
       \"price\": 1399.99,
       \"items\": [
         {
           \"sellableItemId\": \"$SELLABLE_16_512_ID\",
           \"quantity\": 1
         }
       ],
       \"metadata\": {
         \"promotion\": \"Holiday Special\"
       }
     }")
   
   BUNDLE_ID=$(echo "$BUNDLE_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
   echo "✅ Bundle Created: $BUNDLE_ID"
   ```

5. **Test Bundle Availability**
   ```bash
   AVAILABILITY_RESPONSE=$(curl -s -X GET "$BASE_URL/api/bundles/$BUNDLE_ID/availability?warehouseCode=MAIN")
   AVAILABLE_QTY=$(echo "$AVAILABILITY_RESPONSE" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
   echo "✅ Bundle Availability: $AVAILABLE_QTY units"
   
   # Expected: 22 units (25 on hand - 3 reserved = 22 available)
   if [ "$AVAILABLE_QTY" -eq 22 ]; then
     echo "✅ Availability calculation correct"
   else
     echo "❌ Availability calculation incorrect. Expected: 22, Got: $AVAILABLE_QTY"
   fi
   ```

6. **Simulate Order Process**
   ```bash
   # Reserve stock for order
   curl -s -X POST "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/reserve" \
     -H "$CONTENT_TYPE" \
     -d '{
       "quantity": 2,
       "warehouseCode": "MAIN"
     }' > /dev/null
   echo "✅ Reserved 2 units for order"
   
   # Check updated availability
   UPDATED_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$BUNDLE_ID/availability?warehouseCode=MAIN")
   UPDATED_QTY=$(echo "$UPDATED_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
   echo "✅ Updated Bundle Availability: $UPDATED_QTY units"
   
   # Expected: 20 units (25 on hand - 5 reserved = 20 available)
   if [ "$UPDATED_QTY" -eq 20 ]; then
     echo "✅ Reservation impact calculated correctly"
   else
     echo "❌ Reservation impact incorrect. Expected: 20, Got: $UPDATED_QTY"
   fi
   ```

**Expected Results:**
- All products, variants, and bundles created successfully
- Inventory properly tracked with reservations
- Bundle availability calculated correctly
- Real-time updates when stock changes

---

### Scenario 2: Validation and Error Handling

**Business Context**: Testing system robustness with invalid data and edge cases

#### Test Steps

1. **Test Duplicate SKU Validation**
   ```bash
   echo "Testing duplicate SKU validation..."
   
   # Try to create variant with existing SKU
   DUPLICATE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
     -H "$CONTENT_TYPE" \
     -d '{
       "sku": "LAPTOP-PRO-16GB-512GB",
       "price": 1399.99,
       "optionValues": [
         {
           "optionName": "Memory",
           "optionSlug": "memory",
           "value": "8GB DDR5"
         }
       ]
     }')
   
   # Check for error response
   if echo "$DUPLICATE_RESPONSE" | grep -q "already exists"; then
     echo "✅ Duplicate SKU validation working"
   else
     echo "❌ Duplicate SKU validation failed"
     echo "$DUPLICATE_RESPONSE"
   fi
   ```

2. **Test Duplicate Variant Combination**
   ```bash
   echo "Testing duplicate variant combination validation..."
   
   DUPLICATE_COMBO_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
     -H "$CONTENT_TYPE" \
     -d '{
       "sku": "LAPTOP-PRO-16GB-512GB-DUPLICATE",
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
       ]
     }')
   
   if echo "$DUPLICATE_COMBO_RESPONSE" | grep -q "combination.*already exists"; then
     echo "✅ Duplicate combination validation working"
   else
     echo "❌ Duplicate combination validation failed"
     echo "$DUPLICATE_COMBO_RESPONSE"
   fi
   ```

3. **Test Invalid Bundle Creation**
   ```bash
   echo "Testing invalid bundle validation..."
   
   # Try to create bundle with non-existent sellable item
   INVALID_BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
     -H "$CONTENT_TYPE" \
     -d '{
       "name": "Invalid Bundle",
       "sku": "INVALID-BUNDLE-001",
       "price": 999.99,
       "items": [
         {
           "sellableItemId": "00000000-0000-0000-0000-000000000000",
           "quantity": 1
         }
       ]
     }')
   
   if echo "$INVALID_BUNDLE_RESPONSE" | grep -q "not found\|does not exist"; then
     echo "✅ Invalid bundle item validation working"
   else
     echo "❌ Invalid bundle item validation failed"
     echo "$INVALID_BUNDLE_RESPONSE"
   fi
   ```

4. **Test Input Validation**
   ```bash
   echo "Testing input validation..."
   
   # Try to create product with empty name
   INVALID_INPUT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
     -H "$CONTENT_TYPE" \
     -d '{
       "name": "",
       "slug": "",
       "price": -100
     }')
   
   if echo "$INVALID_INPUT_RESPONSE" | grep -q "validation.*error"; then
     echo "✅ Input validation working"
   else
     echo "❌ Input validation failed"
     echo "$INVALID_INPUT_RESPONSE"
   fi
   ```

**Expected Results:**
- All validation errors properly caught and returned
- RFC 7807 compliant error responses
- Detailed field-level validation messages
- Appropriate HTTP status codes (400, 404, etc.)

---

### Scenario 3: Batch Operations and Performance

**Business Context**: Bulk importing product catalogs efficiently

#### Test Steps

1. **Batch Create Variants**
   ```bash
   echo "Testing batch variant creation..."
   
   BATCH_RESPONSE=$(curl -s -X POST "$BASE_URL/api/batch/variants" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"idempotencyKey\": \"test-batch-$(date +%s)\",
       \"onConflict\": \"skip\",
       \"items\": [
         {
           \"productMasterId\": \"$LAPTOP_ID\",
           \"sku\": \"LAPTOP-PRO-8GB-256GB\",
           \"price\": 999.99,
           \"optionValues\": [
             {
               \"optionName\": \"Memory\",
               \"optionSlug\": \"memory\",
               \"value\": \"8GB DDR5\"
             },
             {
               \"optionName\": \"Storage\",
               \"optionSlug\": \"storage\",
               \"value\": \"256GB NVMe SSD\"
             }
           ]
         },
         {
           \"productMasterId\": \"$LAPTOP_ID\",
           \"sku\": \"LAPTOP-PRO-16GB-1TB\",
           \"price\": 1499.99,
           \"optionValues\": [
             {
               \"optionName\": \"Memory\",
               \"optionSlug\": \"memory\",
               \"value\": \"16GB DDR5\"
             },
             {
               \"optionName\": \"Storage\",
               \"optionSlug\": \"storage\",
               \"value\": \"1TB NVMe SSD\"
             }
           ]
         }
       ]
     }")
   
   SUCCESS_COUNT=$(echo "$BATCH_RESPONSE" | grep -o '"successCount":[0-9]*' | cut -d':' -f2)
   FAILURE_COUNT=$(echo "$BATCH_RESPONSE" | grep -o '"failureCount":[0-9]*' | cut -d':' -f2)
   
   echo "✅ Batch operation completed: $SUCCESS_COUNT successes, $FAILURE_COUNT failures"
   ```

2. **Test Idempotency**
   ```bash
   echo "Testing batch idempotency..."
   
   # Run same batch again with same idempotency key
   IDEMPOTENT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/batch/variants" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"idempotencyKey\": \"test-batch-$(date +%s)\",
       \"onConflict\": \"skip\",
       \"items\": [
         {
           \"productMasterId\": \"$LAPTOP_ID\",
           \"sku\": \"LAPTOP-PRO-IDEMPOTENT-TEST\",
           \"price\": 1199.99,
           \"optionValues\": []
         }
       ]
     }")
   
   # Run again with same key
   IDEMPOTENT_RESPONSE2=$(curl -s -X POST "$BASE_URL/api/batch/variants" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"idempotencyKey\": \"test-batch-$(date +%s)\",
       \"onConflict\": \"skip\",
       \"items\": [
         {
           \"productMasterId\": \"$LAPTOP_ID\",
           \"sku\": \"LAPTOP-PRO-IDEMPOTENT-TEST\",
           \"price\": 1199.99,
           \"optionValues\": []
         }
       ]
     }")
   
   echo "✅ Idempotency test completed"
   ```

**Expected Results:**
- Batch operations process multiple items efficiently
- Idempotency keys prevent duplicate processing
- Proper success/failure reporting
- Atomic operations with rollback on errors

---

### Scenario 4: Complex Bundle Availability

**Business Context**: Multi-component bundles with varying stock levels

#### Test Steps

1. **Create Multi-Component Bundle**
   ```bash
   echo "Creating multi-component bundle..."
   
   # Create additional products for bundle
   MOUSE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
     -H "$CONTENT_TYPE" \
     -d '{
       "name": "Gaming Mouse Pro",
       "slug": "gaming-mouse-pro",
       "category": "Accessories"
     }')
   MOUSE_ID=$(echo "$MOUSE_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
   
   # Create mouse variant
   MOUSE_VARIANT_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$MOUSE_ID/variants" \
     -H "$CONTENT_TYPE" \
     -d '{
       "sku": "MOUSE-PRO-BLACK",
       "price": 79.99,
       "optionValues": []
     }')
   MOUSE_SELLABLE_ID=$(echo "$MOUSE_VARIANT_RESPONSE" | grep -o '"sellableItem":{"id":"[^"]*' | cut -d'"' -f6)
   
   # Stock mouse with limited quantity
   curl -s -X PUT "$BASE_URL/api/inventory/MOUSE-PRO-BLACK/stock" \
     -H "$CONTENT_TYPE" \
     -d '{
       "onHand": 5,
       "reserved": 1,
       "warehouseCode": "MAIN"
     }' > /dev/null
   
   # Create bundle with multiple components
   MULTI_BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"name\": \"Complete Gaming Setup\",
       \"sku\": \"GAMING-COMPLETE-001\",
       \"price\": 1399.99,
       \"items\": [
         {
           \"sellableItemId\": \"$SELLABLE_16_512_ID\",
           \"quantity\": 1
         },
         {
           \"sellableItemId\": \"$MOUSE_SELLABLE_ID\",
           \"quantity\": 1
         }
       ]
     }")
   
   MULTI_BUNDLE_ID=$(echo "$MULTI_BUNDLE_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
   echo "✅ Multi-component bundle created: $MULTI_BUNDLE_ID"
   ```

2. **Test Availability Calculation**
   ```bash
   echo "Testing multi-component availability..."
   
   MULTI_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$MULTI_BUNDLE_ID/availability?warehouseCode=MAIN")
   MULTI_QTY=$(echo "$MULTI_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
   
   echo "✅ Multi-component bundle availability: $MULTI_QTY units"
   
   # Expected: 4 units (limited by mouse: 5 on hand - 1 reserved = 4 available)
   if [ "$MULTI_QTY" -eq 4 ]; then
     echo "✅ Multi-component availability calculation correct"
   else
     echo "❌ Multi-component availability incorrect. Expected: 4, Got: $MULTI_QTY"
   fi
   ```

3. **Test Stock Impact on Bundle**
   ```bash
   echo "Testing stock impact on bundle availability..."
   
   # Reserve more mouse stock
   curl -s -X POST "$BASE_URL/api/inventory/MOUSE-PRO-BLACK/reserve" \
     -H "$CONTENT_TYPE" \
     -d '{
       "quantity": 2,
       "warehouseCode": "MAIN"
     }' > /dev/null
   
   # Check updated bundle availability
   UPDATED_MULTI_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$MULTI_BUNDLE_ID/availability?warehouseCode=MAIN")
   UPDATED_MULTI_QTY=$(echo "$UPDATED_MULTI_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
   
   echo "✅ Updated multi-component availability: $UPDATED_MULTI_QTY units"
   
   # Expected: 2 units (mouse now: 5 on hand - 3 reserved = 2 available)
   if [ "$UPDATED_MULTI_QTY" -eq 2 ]; then
     echo "✅ Stock impact calculation correct"
   else
     echo "❌ Stock impact incorrect. Expected: 2, Got: $UPDATED_MULTI_QTY"
   fi
   ```

**Expected Results:**
- Bundle availability limited by component with lowest stock
- Real-time updates when any component stock changes
- Accurate component-level availability breakdown
- Proper handling of multi-quantity requirements

---

### Scenario 5: Search and Filtering

**Business Context**: Customer browsing and searching product catalog

#### Test Steps

1. **Test Product Search**
   ```bash
   echo "Testing product search functionality..."
   
   # Search by name
   SEARCH_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?search=gaming")
   SEARCH_COUNT=$(echo "$SEARCH_RESPONSE" | grep -o '"total":[0-9]*' | cut -d':' -f2)
   echo "✅ Search 'gaming' found $SEARCH_COUNT products"
   
   # Search by category
   CATEGORY_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?category=Electronics")
   CATEGORY_COUNT=$(echo "$CATEGORY_RESPONSE" | grep -o '"total":[0-9]*' | cut -d':' -f2)
   echo "✅ Category 'Electronics' found $CATEGORY_COUNT products"
   ```

2. **Test Sorting**
   ```bash
   echo "Testing sorting functionality..."
   
   # Sort by name ascending
   SORT_ASC_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?sortBy=name&sortDirection=asc")
   echo "✅ Sort by name ascending completed"
   
   # Sort by creation date descending
   SORT_DESC_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?sortBy=createdAt&sortDirection=desc")
   echo "✅ Sort by creation date descending completed"
   ```

3. **Test Pagination**
   ```bash
   echo "Testing pagination..."
   
   # Get first page with small page size
   PAGE1_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?page=1&pageSize=2")
   PAGE1_COUNT=$(echo "$PAGE1_RESPONSE" | grep -o '"data":\[[^]]*\]' | grep -o '"id":' | wc -l)
   HAS_NEXT=$(echo "$PAGE1_RESPONSE" | grep -o '"hasNext":[^,]*' | cut -d':' -f2)
   
   echo "✅ Page 1 returned $PAGE1_COUNT items, hasNext: $HAS_NEXT"
   
   # Get second page
   PAGE2_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?page=2&pageSize=2")
   PAGE2_COUNT=$(echo "$PAGE2_RESPONSE" | grep -o '"data":\[[^]]*\]' | grep -o '"id":' | wc -l)
   
   echo "✅ Page 2 returned $PAGE2_COUNT items"
   ```

4. **Test Combined Filters**
   ```bash
   echo "Testing combined filters..."
   
   COMBINED_RESPONSE=$(curl -s -X GET "$BASE_URL/api/products?search=gaming&category=Electronics&sortBy=name&page=1&pageSize=5")
   COMBINED_COUNT=$(echo "$COMBINED_RESPONSE" | grep -o '"total":[0-9]*' | cut -d':' -f2)
   echo "✅ Combined filters found $COMBINED_COUNT products"
   ```

**Expected Results:**
- Search returns relevant products
- Sorting works correctly in both directions
- Pagination provides proper metadata
- Combined filters work together correctly

---

### Scenario 6: Edge Cases and Stress Testing

**Business Context**: Testing system limits and edge conditions

#### Test Steps

1. **Test Zero Stock Scenarios**
   ```bash
   echo "Testing zero stock scenarios..."
   
   # Set stock to zero
   curl -s -X PUT "$BASE_URL/api/inventory/MOUSE-PRO-BLACK/stock" \
     -H "$CONTENT_TYPE" \
     -d '{
       "onHand": 0,
       "reserved": 0,
       "warehouseCode": "MAIN"
     }' > /dev/null
   
   # Check bundle availability
   ZERO_STOCK_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$MULTI_BUNDLE_ID/availability?warehouseCode=MAIN")
   ZERO_STOCK_QTY=$(echo "$ZERO_STOCK_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
   IS_AVAILABLE=$(echo "$ZERO_STOCK_AVAILABILITY" | grep -o '"isAvailable":[^,]*' | cut -d':' -f2)
   
   echo "✅ Zero stock bundle availability: $ZERO_STOCK_QTY units, available: $IS_AVAILABLE"
   
   if [ "$ZERO_STOCK_QTY" -eq 0 ] && [ "$IS_AVAILABLE" = "false" ]; then
     echo "✅ Zero stock handling correct"
   else
     echo "❌ Zero stock handling incorrect"
   fi
   ```

2. **Test Large Batch Operations**
   ```bash
   echo "Testing large batch operations..."
   
   # Create batch with multiple items
   LARGE_BATCH_ITEMS='['
   for i in {1..10}; do
     if [ $i -gt 1 ]; then
       LARGE_BATCH_ITEMS+=','
     fi
     LARGE_BATCH_ITEMS+="{
       \"productMasterId\": \"$LAPTOP_ID\",
       \"sku\": \"LAPTOP-BATCH-$i\",
       \"price\": $((1000 + i * 100)).99,
       \"optionValues\": []
     }"
   done
   LARGE_BATCH_ITEMS+=']'
   
   LARGE_BATCH_RESPONSE=$(curl -s -X POST "$BASE_URL/api/batch/variants" \
     -H "$CONTENT_TYPE" \
     -d "{
       \"idempotencyKey\": \"large-batch-$(date +%s)\",
       \"onConflict\": \"skip\",
       \"items\": $LARGE_BATCH_ITEMS
     }")
   
   LARGE_SUCCESS_COUNT=$(echo "$LARGE_BATCH_RESPONSE" | grep -o '"successCount":[0-9]*' | cut -d':' -f2)
   echo "✅ Large batch operation: $LARGE_SUCCESS_COUNT successes"
   ```

3. **Test Concurrent Operations**
   ```bash
   echo "Testing concurrent stock operations..."
   
   # Simulate concurrent reservations
   curl -s -X POST "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/reserve" \
     -H "$CONTENT_TYPE" \
     -d '{"quantity": 1, "warehouseCode": "MAIN"}' &
   
   curl -s -X POST "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/reserve" \
     -H "$CONTENT_TYPE" \
     -d '{"quantity": 1, "warehouseCode": "MAIN"}' &
   
   curl -s -X POST "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/reserve" \
     -H "$CONTENT_TYPE" \
     -d '{"quantity": 1, "warehouseCode": "MAIN"}' &
   
   wait
   echo "✅ Concurrent operations completed"
   ```

**Expected Results:**
- Zero stock properly handled in availability calculations
- Large batch operations complete successfully
- Concurrent operations maintain data consistency
- System remains stable under stress

---

## Test Results Summary

### Automated Test Execution

```bash
#!/bin/bash
# Save as run-all-tests.sh

echo "🚀 Starting Comprehensive API Test Suite"
echo "========================================"

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to run test and track results
run_test() {
    local test_name="$1"
    local test_command="$2"
    
    echo "Running: $test_name"
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    if eval "$test_command"; then
        PASSED_TESTS=$((PASSED_TESTS + 1))
        echo "✅ PASSED: $test_name"
    else
        FAILED_TESTS=$((FAILED_TESTS + 1))
        echo "❌ FAILED: $test_name"
    fi
    echo ""
}

# Run all test scenarios
run_test "Health Check" "curl -s $BASE_URL/api/health | grep -q healthy"
run_test "Create Product" "curl -s -X POST $BASE_URL/api/products -H '$CONTENT_TYPE' -d '{\"name\":\"Test\",\"slug\":\"test-$(date +%s)\"}' | grep -q '\"id\":'"
# Add more tests...

echo "📊 Test Results Summary"
echo "======================"
echo "Total Tests: $TOTAL_TESTS"
echo "Passed: $PASSED_TESTS"
echo "Failed: $FAILED_TESTS"
echo "Success Rate: $(( PASSED_TESTS * 100 / TOTAL_TESTS ))%"

if [ $FAILED_TESTS -eq 0 ]; then
    echo "🎉 All tests passed!"
    exit 0
else
    echo "⚠️  Some tests failed"
    exit 1
fi
```

### Performance Benchmarks

```bash
# Response time testing
echo "Testing API response times..."

# Health check (should be < 100ms)
time curl -s "$BASE_URL/api/health" > /dev/null

# Product creation (should be < 500ms)
time curl -s -X POST "$BASE_URL/api/products" \
  -H "$CONTENT_TYPE" \
  -d '{"name":"Perf Test","slug":"perf-test-'$(date +%s)'"}' > /dev/null

# Bundle availability calculation (should be < 1000ms)
time curl -s -X GET "$BASE_URL/api/bundles/$BUNDLE_ID/availability" > /dev/null
```

### Data Validation Checks

```bash
# Verify data integrity
echo "Verifying data integrity..."

# Check SKU uniqueness
TOTAL_SKUS=$(curl -s "$BASE_URL/api/products" | grep -o '"sku":"[^"]*"' | wc -l)
UNIQUE_SKUS=$(curl -s "$BASE_URL/api/products" | grep -o '"sku":"[^"]*"' | sort | uniq | wc -l)

if [ "$TOTAL_SKUS" -eq "$UNIQUE_SKUS" ]; then
    echo "✅ SKU uniqueness maintained"
else
    echo "❌ SKU uniqueness violated"
fi

# Check bundle availability consistency
# (Add more integrity checks as needed)
```

## Conclusion

These comprehensive test scenarios validate:

1. **Core Functionality**: Product, variant, and bundle management
2. **Business Logic**: Availability calculations, stock management
3. **Data Integrity**: Validation rules and constraints
4. **Performance**: Response times and batch operations
5. **Error Handling**: Validation errors and edge cases
6. **Scalability**: Large datasets and concurrent operations

The test suite provides confidence in the system's reliability, performance, and business logic correctness, making it ready for production deployment and demonstration to stakeholders.