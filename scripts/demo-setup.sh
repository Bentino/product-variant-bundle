#!/bin/bash

# Product Variant Bundle API - Demo Setup Script
# This script creates comprehensive sample data for demonstration

set -e  # Exit on any error

BASE_URL="http://localhost:8080"
CONTENT_TYPE="Content-Type: application/json"

echo "🚀 Starting Product Variant Bundle API Demo Setup"
echo "=================================================="

# Check if API is running
echo "📡 Checking API health..."
if ! curl -s "$BASE_URL/api/health" > /dev/null; then
    echo "❌ API is not running. Please start with: docker compose up -d"
    exit 1
fi
echo "✅ API is healthy"

# Function to extract ID from JSON response
extract_id() {
    echo "$1" | grep -o '"id":"[^"]*' | cut -d'"' -f4
}

# Function to extract sellable item ID from variant response
extract_sellable_id() {
    echo "$1" | grep -o '"sellableItem":{"id":"[^"]*' | cut -d'"' -f6
}

echo ""
echo "🏗️  Creating Product Masters..."
echo "================================"

# 1. Create Gaming Laptop Product Master
echo "Creating Gaming Laptop..."
LAPTOP_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
  -H "$CONTENT_TYPE" \
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
  }')

LAPTOP_ID=$(extract_id "$LAPTOP_RESPONSE")
echo "✅ Gaming Laptop created with ID: $LAPTOP_ID"

# 2. Create Gaming Mouse Product Master
echo "Creating Gaming Mouse..."
MOUSE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
  -H "$CONTENT_TYPE" \
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
  }')

MOUSE_ID=$(extract_id "$MOUSE_RESPONSE")
echo "✅ Gaming Mouse created with ID: $MOUSE_ID"

# 3. Create Gaming Headset Product Master
echo "Creating Gaming Headset..."
HEADSET_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products" \
  -H "$CONTENT_TYPE" \
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
  }')

HEADSET_ID=$(extract_id "$HEADSET_RESPONSE")
echo "✅ Gaming Headset created with ID: $HEADSET_ID"

echo ""
echo "🔧 Creating Product Variants..."
echo "==============================="

# Create Laptop Variants
echo "Creating laptop variants..."

# 16GB + 512GB Laptop
LAPTOP_16_512_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
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
    ],
    "attributes": {
      "color": "Midnight Black",
      "model": "Pro-16-512"
    }
  }')

LAPTOP_16_512_SELLABLE_ID=$(extract_sellable_id "$LAPTOP_16_512_RESPONSE")
echo "✅ Laptop 16GB+512GB variant created (Sellable ID: $LAPTOP_16_512_SELLABLE_ID)"

# 32GB + 1TB Laptop
LAPTOP_32_1TB_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
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
    ],
    "attributes": {
      "color": "Midnight Black",
      "model": "Pro-32-1TB"
    }
  }')

LAPTOP_32_1TB_SELLABLE_ID=$(extract_sellable_id "$LAPTOP_32_1TB_RESPONSE")
echo "✅ Laptop 32GB+1TB variant created (Sellable ID: $LAPTOP_32_1TB_SELLABLE_ID)"

# 16GB + 1TB Laptop
LAPTOP_16_1TB_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$LAPTOP_ID/variants" \
  -H "$CONTENT_TYPE" \
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
  }')

LAPTOP_16_1TB_SELLABLE_ID=$(extract_sellable_id "$LAPTOP_16_1TB_RESPONSE")
echo "✅ Laptop 16GB+1TB variant created (Sellable ID: $LAPTOP_16_1TB_SELLABLE_ID)"

# Create Mouse Variants
echo "Creating mouse variants..."

# Black Mouse
MOUSE_BLACK_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$MOUSE_ID/variants" \
  -H "$CONTENT_TYPE" \
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
  }')

MOUSE_BLACK_SELLABLE_ID=$(extract_sellable_id "$MOUSE_BLACK_RESPONSE")
echo "✅ Black Mouse variant created (Sellable ID: $MOUSE_BLACK_SELLABLE_ID)"

# White Mouse
MOUSE_WHITE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$MOUSE_ID/variants" \
  -H "$CONTENT_TYPE" \
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
  }')

MOUSE_WHITE_SELLABLE_ID=$(extract_sellable_id "$MOUSE_WHITE_RESPONSE")
echo "✅ White Mouse variant created (Sellable ID: $MOUSE_WHITE_SELLABLE_ID)"

# Create Headset Variants
echo "Creating headset variants..."

# Black Headset
HEADSET_BLACK_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$HEADSET_ID/variants" \
  -H "$CONTENT_TYPE" \
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
  }')

HEADSET_BLACK_SELLABLE_ID=$(extract_sellable_id "$HEADSET_BLACK_RESPONSE")
echo "✅ Black Headset variant created (Sellable ID: $HEADSET_BLACK_SELLABLE_ID)"

# White Headset  
HEADSET_WHITE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/products/$HEADSET_ID/variants" \
  -H "$CONTENT_TYPE" \
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
  }')

HEADSET_WHITE_SELLABLE_ID=$(extract_sellable_id "$HEADSET_WHITE_RESPONSE")
echo "✅ White Headset variant created (Sellable ID: $HEADSET_WHITE_SELLABLE_ID)"

echo ""
echo "📦 Setting Up Inventory..."
echo "=========================="

# Stock Laptop Variants
echo "Stocking laptop variants..."
curl -s -X PUT "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 25,
    "reserved": 3,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Laptop 16GB+512GB: 25 on hand, 3 reserved"

curl -s -X PUT "$BASE_URL/api/inventory/LAPTOP-PRO-32GB-1TB/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 8,
    "reserved": 1,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Laptop 32GB+1TB: 8 on hand, 1 reserved"

curl -s -X PUT "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-1TB/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 15,
    "reserved": 2,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Laptop 16GB+1TB: 15 on hand, 2 reserved"

# Stock Accessories
echo "Stocking accessories..."
curl -s -X PUT "$BASE_URL/api/inventory/MOUSE-PRO-BLACK/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 50,
    "reserved": 5,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Black Mouse: 50 on hand, 5 reserved"

curl -s -X PUT "$BASE_URL/api/inventory/MOUSE-PRO-WHITE/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 30,
    "reserved": 2,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ White Mouse: 30 on hand, 2 reserved"

curl -s -X PUT "$BASE_URL/api/inventory/HEADSET-PRO-BLACK/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 40,
    "reserved": 4,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Black Headset: 40 on hand, 4 reserved"

curl -s -X PUT "$BASE_URL/api/inventory/HEADSET-PRO-WHITE/stock" \
  -H "$CONTENT_TYPE" \
  -d '{
    "onHand": 25,
    "reserved": 1,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ White Headset: 25 on hand, 1 reserved"

echo ""
echo "🎁 Creating Product Bundles..."
echo "=============================="

# Complete Gaming Setup Bundle
echo "Creating Complete Gaming Setup Bundle..."
COMPLETE_BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
  -H "$CONTENT_TYPE" \
  -d "{
    \"name\": \"Complete Gaming Setup Pro\",
    \"description\": \"Everything you need for professional gaming: laptop, mouse, and headset\",
    \"sku\": \"GAMING-SETUP-COMPLETE\",
    \"price\": 1399.99,
    \"items\": [
      {
        \"sellableItemId\": \"$LAPTOP_16_512_SELLABLE_ID\",
        \"quantity\": 1
      },
      {
        \"sellableItemId\": \"$MOUSE_BLACK_SELLABLE_ID\",
        \"quantity\": 1
      },
      {
        \"sellableItemId\": \"$HEADSET_BLACK_SELLABLE_ID\",
        \"quantity\": 1
      }
    ],
    \"metadata\": {
      \"promotion\": \"Holiday Special 2024\",
      \"discount\": \"Save \$110\",
      \"validUntil\": \"2024-12-31\",
      \"category\": \"Gaming Bundles\"
    }
  }")

COMPLETE_BUNDLE_ID=$(extract_id "$COMPLETE_BUNDLE_RESPONSE")
echo "✅ Complete Gaming Setup Bundle created (ID: $COMPLETE_BUNDLE_ID)"

# Premium Gaming Bundle
echo "Creating Premium Gaming Setup Bundle..."
PREMIUM_BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
  -H "$CONTENT_TYPE" \
  -d "{
    \"name\": \"Premium Gaming Setup Pro\",
    \"description\": \"Top-tier gaming setup with 32GB laptop and premium accessories\",
    \"sku\": \"GAMING-SETUP-PREMIUM\",
    \"price\": 1799.99,
    \"items\": [
      {
        \"sellableItemId\": \"$LAPTOP_32_1TB_SELLABLE_ID\",
        \"quantity\": 1
      },
      {
        \"sellableItemId\": \"$MOUSE_WHITE_SELLABLE_ID\",
        \"quantity\": 1
      },
      {
        \"sellableItemId\": \"$HEADSET_WHITE_SELLABLE_ID\",
        \"quantity\": 1
      }
    ],
    \"metadata\": {
      \"promotion\": \"Premium Collection\",
      \"discount\": \"Save \$210\",
      \"validUntil\": \"2024-12-31\",
      \"category\": \"Premium Bundles\"
    }
  }")

PREMIUM_BUNDLE_ID=$(extract_id "$PREMIUM_BUNDLE_RESPONSE")
echo "✅ Premium Gaming Setup Bundle created (ID: $PREMIUM_BUNDLE_ID)"

# Accessories Bundle
echo "Creating Gaming Accessories Bundle..."
ACCESSORIES_BUNDLE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/bundles" \
  -H "$CONTENT_TYPE" \
  -d "{
    \"name\": \"Gaming Accessories Bundle\",
    \"description\": \"Perfect gaming accessories combo - mouse and headset\",
    \"sku\": \"GAMING-ACCESSORIES\",
    \"price\": 179.99,
    \"items\": [
      {
        \"sellableItemId\": \"$MOUSE_BLACK_SELLABLE_ID\",
        \"quantity\": 1
      },
      {
        \"sellableItemId\": \"$HEADSET_BLACK_SELLABLE_ID\",
        \"quantity\": 1
      }
    ],
    \"metadata\": {
      \"promotion\": \"Accessories Deal\",
      \"discount\": \"Save \$30\",
      \"validUntil\": \"2024-12-31\",
      \"category\": \"Accessory Bundles\"
    }
  }")

ACCESSORIES_BUNDLE_ID=$(extract_id "$ACCESSORIES_BUNDLE_RESPONSE")
echo "✅ Gaming Accessories Bundle created (ID: $ACCESSORIES_BUNDLE_ID)"

echo ""
echo "🧪 Testing Key Features..."
echo "=========================="

# Test Bundle Availability
echo "Testing bundle availability calculations..."
COMPLETE_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$COMPLETE_BUNDLE_ID/availability?warehouseCode=MAIN")
COMPLETE_QTY=$(echo "$COMPLETE_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
echo "✅ Complete Gaming Setup availability: $COMPLETE_QTY units"

PREMIUM_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$PREMIUM_BUNDLE_ID/availability?warehouseCode=MAIN")
PREMIUM_QTY=$(echo "$PREMIUM_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
echo "✅ Premium Gaming Setup availability: $PREMIUM_QTY units"

# Test Stock Reservation
echo "Testing stock reservation..."
curl -s -X POST "$BASE_URL/api/inventory/LAPTOP-PRO-16GB-512GB/reserve" \
  -H "$CONTENT_TYPE" \
  -d '{
    "quantity": 2,
    "warehouseCode": "MAIN"
  }' > /dev/null
echo "✅ Reserved 2 units of Laptop 16GB+512GB"

# Check updated availability
UPDATED_AVAILABILITY=$(curl -s -X GET "$BASE_URL/api/bundles/$COMPLETE_BUNDLE_ID/availability?warehouseCode=MAIN")
UPDATED_QTY=$(echo "$UPDATED_AVAILABILITY" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
echo "✅ Complete Gaming Setup availability after reservation: $UPDATED_QTY units"

echo ""
echo "📊 Demo Data Summary"
echo "===================="
echo "Products Created:"
echo "  • Gaming Laptop Pro (3 variants)"
echo "  • Gaming Mouse Pro (2 variants)"  
echo "  • Gaming Headset Pro (2 variants)"
echo ""
echo "Bundles Created:"
echo "  • Complete Gaming Setup Pro"
echo "  • Premium Gaming Setup Pro"
echo "  • Gaming Accessories Bundle"
echo ""
echo "Inventory Stocked:"
echo "  • All variants have realistic stock levels"
echo "  • Some items have reserved quantities"
echo "  • Multi-warehouse support (MAIN warehouse)"
echo ""
echo "🎯 Demo Ready!"
echo "==============="
echo "API Base URL: $BASE_URL"
echo "Swagger UI: $BASE_URL"
echo "Health Check: $BASE_URL/api/health"
echo ""
echo "Key Demo URLs:"
echo "  • Products: $BASE_URL/api/products"
echo "  • Bundles: $BASE_URL/api/bundles"
echo "  • Complete Bundle Availability: $BASE_URL/api/bundles/$COMPLETE_BUNDLE_ID/availability"
echo "  • Premium Bundle Availability: $BASE_URL/api/bundles/$PREMIUM_BUNDLE_ID/availability"
echo ""
echo "📋 Demo Data Summary:"
echo "  • 3 Product Masters (Laptop, Mouse, Headset)"
echo "  • 7 Product Variants (3 laptop configs, 2 mouse colors, 2 headset colors)"
echo "  • 3 Product Bundles (Complete, Premium, Accessories)"
echo "  • Realistic inventory levels with reservations"
echo ""
echo "🧪 Quick Validation Tests:"

# Test bundle availability
COMPLETE_AVAIL=$(curl -s -X GET "$BASE_URL/api/bundles/$COMPLETE_BUNDLE_ID/availability?warehouseCode=MAIN" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)
PREMIUM_AVAIL=$(curl -s -X GET "$BASE_URL/api/bundles/$PREMIUM_BUNDLE_ID/availability?warehouseCode=MAIN" | grep -o '"availableQuantity":[0-9]*' | cut -d':' -f2)

echo "  • Complete Gaming Setup availability: $COMPLETE_AVAIL units"
echo "  • Premium Gaming Setup availability: $PREMIUM_AVAIL units"

# Test search functionality
SEARCH_COUNT=$(curl -s -X GET "$BASE_URL/api/products?search=gaming" | grep -o '"total":[0-9]*' | cut -d':' -f2)
echo "  • Search 'gaming' returns: $SEARCH_COUNT products"

echo ""
echo "🎬 Demo Scenarios Ready:"
echo "  1. Product catalog browsing and search"
echo "  2. Variant selection and pricing"
echo "  3. Bundle availability calculation"
echo "  4. Inventory management and reservations"
echo "  5. Batch operations and validation"
echo "  6. Error handling and edge cases"
echo ""
echo "📚 Documentation Available:"
echo "  • API_DOCUMENTATION.md - Complete API reference with examples"
echo "  • DEMO_GUIDE.md - Comprehensive demo walkthrough"
echo "  • TEST_SCENARIOS.md - Complete test scenarios"
echo "  • Product_Variant_Bundle_API.postman_collection.json - Postman collection"
echo ""
echo "✨ Demo setup completed successfully!"
echo "🚀 Ready for presentation and testing!"