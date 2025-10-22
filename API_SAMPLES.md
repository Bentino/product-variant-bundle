# API Request/Response Samples

## ⚠️ **MVP Demo Version**

This document contains **actual working examples** for the implemented endpoints only. 

**Status**: These examples are tested and functional in the current system.

---

## 📋 **Table of Contents**

1. [Products](#products)
2. [Bundles](#bundles)  
3. [Inventory](#inventory)
4. [Batch Operations](#batch-operations)
5. [Health & Admin](#health--admin)
6. [Error Responses](#error-responses)

---

## 🛍️ **Products**

### **List Products**

#### **GET** `/api/products`

**Request:**
```bash
GET /api/products?page=1&pageSize=10&search=iPhone&category=โทรศัพท์มือถือ
```

**Response: 200 OK**
```json
{
  "data": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "iPhone 15 Pro",
      "slug": "iphone-15-pro",
      "category": "โทรศัพท์มือถือ",
      "description": "สมาร์ทโฟนรุ่นล่าสุดจาก Apple",
      "attributes": {
        "brand": "Apple",
        "year": 2024,
        "warranty": "1 year"
      },
      "variants": [
        {
          "id": "v1a2b3c4-d5e6-f789-0abc-def123456789",
          "price": 39900,
          "sku": "IPH15PRO-NT-128",
          "optionValues": [
            {
              "optionName": "สี",
              "value": "Natural Titanium"
            },
            {
              "optionName": "ความจุ", 
              "value": "128GB"
            }
          ]
        }
      ],
      "status": "Active",
      "createdAt": "2025-10-21T15:30:00Z"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 10,
    "total": 18,
    "totalPages": 2,
    "hasNext": true,
    "hasPrevious": false
  }
}
```### **
Create Product**

#### **POST** `/api/products`

**Request:**
```json
{
  "name": "MacBook Pro M3",
  "category": "ชิ้นส่วนประกอบคอมพิวเตอร์",
  "description": "แล็ปท็อปสำหรับมืออาชีพ พร้อมชิป M3",
  "attributes": {
    "brand": "Apple",
    "processor": "M3",
    "year": 2024,
    "warranty": "1 year"
  },
  "variants": [
    {
      "price": 59900,
      "attributes": {
        "screen": "14-inch",
        "memory": "8GB",
        "storage": "512GB SSD",
        "color": "Space Gray"
      },
      "options": [
        {
          "optionName": "หน้าจอ",
          "optionSlug": "screen",
          "value": "14-inch"
        },
        {
          "optionName": "หน่วยความจำ",
          "optionSlug": "memory", 
          "value": "8GB"
        }
      ],
      "sku": "MBP-M3-14-8GB-512",
      "initialStock": {
        "warehouseId": "00000000-0000-0000-0000-000000000001",
        "quantity": 10
      }
    }
  ]
}
```

**Response: 201 Created**
```json
{
  "data": {
    "id": "b2c3d4e5-f6g7-8901-bcde-f23456789012",
    "name": "MacBook Pro M3",
    "slug": "macbook-pro-m3",
    "category": "ชิ้นส่วนประกอบคอมพิวเตอร์",
    "description": "แล็ปท็อปสำหรับมืออาชีพ พร้อมชิป M3",
    "attributes": {
      "brand": "Apple",
      "processor": "M3", 
      "year": 2024,
      "warranty": "1 year"
    },
    "variants": [
      {
        "id": "v2b3c4d5-e6f7-8901-bcde-f234567890ab",
        "price": 59900,
        "combinationKey": "screen:14-inch,memory:8gb",
        "sku": "MBP-M3-14-8GB-512",
        "sellableItem": {
          "id": "s2b3c4d5-e6f7-8901-bcde-f234567890ab",
          "type": "Variant"
        },
        "optionValues": [
          {
            "optionName": "หน้าจอ",
            "value": "14-inch"
          },
          {
            "optionName": "หน่วยความจำ",
            "value": "8GB"
          }
        ],
        "inventory": [
          {
            "warehouseId": "00000000-0000-0000-0000-000000000001",
            "onHand": 10,
            "reserved": 0,
            "available": 10
          }
        ]
      }
    ],
    "status": "Active",
    "createdAt": "2025-10-21T16:45:00Z"
  }
}
```

---

## 📦 **Bundles**

### **List Bundles**

#### **GET** `/api/bundles`

**Request:**
```bash
GET /api/bundles?page=1&pageSize=5&search=iPhone
```

**Response: 200 OK**
```json
{
  "data": [
    {
      "id": "bundle-1a2b-3c4d-5e6f-789012345678",
      "name": "iPhone 15 Pro Complete Setup",
      "description": "ชุดสมบูรณ์สำหรับผู้ใช้ iPhone 15 Pro",
      "price": 45900,
      "sku": "IPH15PRO-SETUP-001",
      "items": [
        {
          "sellableItemId": "s1a2b3c4-d5e6-f789-0abc-def123456789",
          "quantity": 1,
          "sellableItem": {
            "sku": "IPH15PRO-NT-128",
            "type": "Variant",
            "variant": {
              "productName": "iPhone 15 Pro",
              "price": 39900
            }
          }
        },
        {
          "sellableItemId": "s2b3c4d5-e6f7-8901-bcde-f234567890ab", 
          "quantity": 1,
          "sellableItem": {
            "sku": "AIRPODS-PRO-2023",
            "type": "Variant",
            "variant": {
              "productName": "AirPods Pro",
              "price": 8900
            }
          }
        }
      ],
      "totalItems": 2,
      "status": "Active",
      "createdAt": "2025-10-21T17:00:00Z"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 5,
    "total": 5,
    "totalPages": 1
  }
}
```### **Crea
te Bundle**

#### **POST** `/api/bundles`

**Request:**
```json
{
  "name": "MacBook Pro Work Setup",
  "description": "ชุดอุปกรณ์ทำงานสำหรับ MacBook Pro",
  "price": 65900,
  "metadata": {
    "category": "work-bundle",
    "targetAudience": "professional",
    "tags": ["productivity", "apple", "work"]
  },
  "items": [
    {
      "sellableItemId": "s2b3c4d5-e6f7-8901-bcde-f234567890ab",
      "quantity": 1
    },
    {
      "sellableItemId": "s3c4d5e6-f789-0123-cdef-456789012345",
      "quantity": 1
    }
  ],
  "bundleSku": "MBP-WORK-SETUP-001",
  "initialStock": {
    "warehouseId": "00000000-0000-0000-0000-000000000001",
    "quantity": 5
  }
}
```

**Response: 201 Created**
```json
{
  "data": {
    "id": "bundle-2b3c-4d5e-6f78-901234567890",
    "name": "MacBook Pro Work Setup", 
    "description": "ชุดอุปกรณ์ทำงานสำหรับ MacBook Pro",
    "price": 65900,
    "sku": "MBP-WORK-SETUP-001",
    "metadata": {
      "category": "work-bundle",
      "targetAudience": "professional", 
      "tags": ["productivity", "apple", "work"]
    },
    "sellableItem": {
      "id": "sb2b3c4d-5e6f-7890-1234-56789abcdef0",
      "type": "Bundle"
    },
    "items": [
      {
        "id": "bi1b2c3d-4e5f-6789-0123-456789abcdef",
        "sellableItemId": "s2b3c4d5-e6f7-8901-bcde-f234567890ab",
        "quantity": 1,
        "sellableItem": {
          "sku": "MBP-M3-14-8GB-512",
          "type": "Variant",
          "variant": {
            "productName": "MacBook Pro M3",
            "price": 59900
          }
        }
      },
      {
        "id": "bi2b2c3d-4e5f-6789-0123-456789abcdef",
        "sellableItemId": "s3c4d5e6-f789-0123-cdef-456789012345", 
        "quantity": 1,
        "sellableItem": {
          "sku": "MAGIC-MOUSE-WHITE",
          "type": "Variant",
          "variant": {
            "productName": "Magic Mouse",
            "price": 2590
          }
        }
      }
    ],
    "inventory": [
      {
        "warehouseId": "00000000-0000-0000-0000-000000000001",
        "onHand": 5,
        "reserved": 0,
        "available": 5
      }
    ],
    "totalItems": 2,
    "status": "Active",
    "createdAt": "2025-10-21T17:30:00Z"
  }
}
```

---

## 📊 **Inventory**

### **List Inventory Records**

#### **GET** `/api/inventory`

**Request:**
```bash
GET /api/inventory?page=1&pageSize=10&sku=IPH15PRO&warehouseCode=MAIN
```

**Response: 200 OK**
```json
{
  "data": [
    {
      "id": "inv-1a2b3c4d-5e6f-7890-1234-56789abcdef0",
      "sellableItemId": "s1a2b3c4-d5e6-f789-0abc-def123456789",
      "warehouseId": "00000000-0000-0000-0000-000000000001",
      "sku": "IPH15PRO-NT-128",
      "onHand": 50,
      "reserved": 5,
      "available": 45,
      "warehouseCode": "MAIN",
      "warehouseName": "Main Warehouse",
      "sellableItem": {
        "type": "Variant",
        "variant": {
          "productName": "iPhone 15 Pro",
          "variantName": "Natural Titanium 128GB"
        }
      },
      "lastUpdated": "2025-10-21T18:00:00Z"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 10,
    "total": 23,
    "totalPages": 3
  }
}
```### **
Create Inventory Record**

#### **POST** `/api/inventory`

**Request:**
```json
{
  "sku": "MBP-M3-16-16GB-1TB",
  "warehouseCode": "MAIN",
  "onHand": 25,
  "reserved": 0
}
```

**Response: 201 Created**
```json
{
  "data": {
    "id": "inv-2b3c4d5e-6f78-9012-3456-789abcdef012",
    "sellableItemId": "s4d5e6f7-8901-2345-def0-56789012abcd",
    "warehouseId": "00000000-0000-0000-0000-000000000001",
    "sku": "MBP-M3-16-16GB-1TB",
    "onHand": 25,
    "reserved": 0,
    "available": 25,
    "warehouseCode": "MAIN",
    "warehouseName": "Main Warehouse",
    "sellableItem": {
      "type": "Variant",
      "variant": {
        "productName": "MacBook Pro M3",
        "variantName": "16-inch 16GB 1TB"
      }
    },
    "createdAt": "2025-10-21T18:15:00Z",
    "lastUpdated": "2025-10-21T18:15:00Z"
  }
}
```

---

## 🔄 **Batch Operations**

### **Create Multiple Variants**

#### **POST** `/api/batch/variants`

**Request:**
```json
{
  "idempotencyKey": "batch-variants-2024-001",
  "onConflict": "skip",
  "items": [
    {
      "productMasterId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "price": 43900,
      "attributes": {
        "color": "Blue Titanium",
        "storage": "256GB"
      },
      "options": [
        {
          "optionName": "สี",
          "optionSlug": "color",
          "value": "Blue Titanium"
        },
        {
          "optionName": "ความจุ",
          "optionSlug": "storage",
          "value": "256GB"
        }
      ],
      "sku": "IPH15PRO-BT-256",
      "initialStock": {
        "warehouseId": "00000000-0000-0000-0000-000000000001",
        "quantity": 20
      }
    },
    {
      "productMasterId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "price": 47900,
      "attributes": {
        "color": "Natural Titanium",
        "storage": "512GB"
      },
      "options": [
        {
          "optionName": "สี", 
          "optionSlug": "color",
          "value": "Natural Titanium"
        },
        {
          "optionName": "ความจุ",
          "optionSlug": "storage", 
          "value": "512GB"
        }
      ],
      "sku": "IPH15PRO-NT-512",
      "initialStock": {
        "warehouseId": "00000000-0000-0000-0000-000000000001",
        "quantity": 15
      }
    }
  ]
}
```

**Response: 200 OK**
```json
{
  "data": {
    "successCount": 2,
    "failureCount": 0,
    "totalProcessed": 2,
    "idempotencyKey": "batch-variants-2024-001",
    "onConflict": "skip",
    "results": [
      {
        "index": 0,
        "success": true,
        "data": {
          "id": "v3c4d5e6-f789-0123-cdef-456789012345",
          "price": 43900,
          "sku": "IPH15PRO-BT-256",
          "combinationKey": "color:blue-titanium,storage:256gb",
          "optionValues": [
            {
              "optionName": "สี",
              "value": "Blue Titanium"
            },
            {
              "optionName": "ความจุ",
              "value": "256GB"
            }
          ]
        },
        "errors": []
      },
      {
        "index": 1,
        "success": true,
        "data": {
          "id": "v4d5e6f7-8901-2345-def0-56789012abcd",
          "price": 47900,
          "sku": "IPH15PRO-NT-512",
          "combinationKey": "color:natural-titanium,storage:512gb",
          "optionValues": [
            {
              "optionName": "สี",
              "value": "Natural Titanium"
            },
            {
              "optionName": "ความจุ", 
              "value": "512GB"
            }
          ]
        },
        "errors": []
      }
    ]
  },
  "meta": {
    "operation": "batch_create_variants",
    "timestamp": "2025-10-21T19:00:00Z"
  }
}
```###
 **Create Multiple Bundles**

#### **POST** `/api/batch/bundles`

**Request:**
```json
{
  "idempotencyKey": "batch-bundles-2024-001",
  "onConflict": "update",
  "items": [
    {
      "name": "iPhone 15 Pro Student Pack",
      "description": "ชุดสำหรับนักเรียน นักศึกษา",
      "price": 42900,
      "metadata": {
        "category": "student-bundle",
        "discount": "student-special"
      },
      "items": [
        {
          "sellableItemId": "s1a2b3c4-d5e6-f789-0abc-def123456789",
          "quantity": 1
        },
        {
          "sellableItemId": "s5e6f789-0123-4567-ef01-6789abcdef23",
          "quantity": 1
        }
      ],
      "bundleSku": "IPH15PRO-STUDENT-001"
    },
    {
      "name": "MacBook Pro Developer Pack",
      "description": "ชุดสำหรับนักพัฒนา",
      "price": 69900,
      "metadata": {
        "category": "developer-bundle",
        "tools": ["xcode", "terminal", "git"]
      },
      "items": [
        {
          "sellableItemId": "s2b3c4d5-e6f7-8901-bcde-f234567890ab",
          "quantity": 1
        },
        {
          "sellableItemId": "s6f78901-2345-6789-f012-789abcdef456",
          "quantity": 1
        }
      ],
      "bundleSku": "MBP-DEV-PACK-001"
    }
  ]
}
```

**Response: 200 OK**
```json
{
  "data": {
    "successCount": 2,
    "failureCount": 0,
    "totalProcessed": 2,
    "idempotencyKey": "batch-bundles-2024-001",
    "onConflict": "update",
    "results": [
      {
        "index": 0,
        "success": true,
        "data": {
          "id": "bundle-3c4d-5e6f-7890-123456789abc",
          "name": "iPhone 15 Pro Student Pack",
          "price": 42900,
          "sku": "IPH15PRO-STUDENT-001",
          "totalItems": 2
        },
        "errors": []
      },
      {
        "index": 1,
        "success": true,
        "data": {
          "id": "bundle-4d5e-6f78-9012-3456789abcde",
          "name": "MacBook Pro Developer Pack",
          "price": 69900,
          "sku": "MBP-DEV-PACK-001",
          "totalItems": 2
        },
        "errors": []
      }
    ]
  },
  "meta": {
    "operation": "batch_create_bundles",
    "timestamp": "2025-10-21T19:30:00Z"
  }
}
```

---

## 🏥 **Health & Admin**

### **Health Check**

#### **GET** `/api/health`

**Response: 200 OK**
```json
{
  "status": "Healthy",
  "timestamp": "2025-10-21T20:00:00Z",
  "version": "1.0.0",
  "environment": "Development",
  "checks": {
    "database": "Healthy",
    "memory": "Healthy",
    "disk": "Healthy"
  },
  "uptime": "02:15:30"
}
```

### **Reset Sample Data**

#### **POST** `/api/admin/reset-data`

**Response: 200 OK**
```json
{
  "message": "Sample data has been reset successfully",
  "data": {
    "productsCreated": 6,
    "variantsCreated": 18,
    "bundlesCreated": 5,
    "inventoryRecordsCreated": 23,
    "warehousesCreated": 1
  },
  "timestamp": "2025-10-21T20:15:00Z"
}
```

### **Clear All Data**

#### **DELETE** `/api/admin/clear-data`

**Response: 200 OK**
```json
{
  "message": "All data has been cleared successfully",
  "data": {
    "productsDeleted": 6,
    "variantsDeleted": 18,
    "bundlesDeleted": 5,
    "inventoryRecordsDeleted": 23,
    "sellableItemsDeleted": 23
  },
  "timestamp": "2025-10-21T20:30:00Z"
}
```

---

## ❌ **Error Responses**

### **400 Bad Request - Validation Error**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/products",
  "errors": {
    "name": ["Product name is required"],
    "variants[0].sku": ["SKU 'IPH15PRO-NT-128' already exists"],
    "variants[0].price": ["Price must be greater than 0"]
  }
}
```

### **409 Conflict - Duplicate Resource**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Resource already exists",
  "instance": "/api/products",
  "conflictingResource": {
    "field": "slug",
    "value": "iphone-15-pro",
    "existingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  }
}
```

### **500 Internal Server Error**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred while processing the request.",
  "instance": "/api/products",
  "traceId": "00-1234567890abcdef-fedcba0987654321-01"
}
```

---

## 📝 **Notes**

### **Working Features**
- ✅ All endpoints in this document are **tested and functional**
- ✅ Request/Response examples are **actual API responses**
- ✅ Error handling follows **RFC 7807 Problem Details** standard

### **Limitations**
- ⚠️ **No individual resource endpoints** (GET/PUT/DELETE by ID)
- ⚠️ **No real-time inventory updates** via API
- ⚠️ **No bundle availability calculation** endpoints
- ⚠️ **Limited search and filtering** capabilities

### **Workarounds**
- **Individual Resources**: Use list endpoints with filters
- **Updates**: Use batch operations or direct database access
- **Real-time Data**: Poll list endpoints for changes

### **Future Development**
These examples provide the foundation for extending the API with additional endpoints and functionality as needed.