<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-6">
          <h1 class="text-3xl font-bold text-gray-900">API Reference</h1>
          <p class="mt-2 text-lg text-gray-600">
            Product Variant Bundle API Documentation
          </p>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="flex gap-8">
        <!-- Sidebar Navigation -->
        <div class="w-64 flex-shrink-0">
          <div class="sticky top-8">
            <nav class="space-y-1">
              <div class="pb-4">
                <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wide">
                  Quick Start
                </h3>
                <ul class="mt-2 space-y-1">
                  <li v-if="isDev"><a href="#api-test" class="nav-link">API Test</a></li>
                  <li><a href="#overview" class="nav-link">Overview</a></li>
                  <li><a href="#authentication" class="nav-link">Authentication</a></li>
                  <li><a href="#response-format" class="nav-link">Response Format</a></li>
                </ul>
              </div>

              <div class="pb-4">
                <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wide">
                  Endpoints
                </h3>
                <ul class="mt-2 space-y-1">
                  <li><a href="#products" class="nav-link">Products</a></li>
                  <li><a href="#bundles" class="nav-link">Bundles</a></li>
                  <li><a href="#inventory" class="nav-link">Inventory</a></li>
                  <li><a href="#batch" class="nav-link">Batch Operations</a></li>
                  <li><a href="#health" class="nav-link">Health Checks</a></li>
                </ul>
              </div>

              <div class="pb-4">
                <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wide">
                  Data Models
                </h3>
                <ul class="mt-2 space-y-1">
                  <li><a href="#product-model" class="nav-link">Product Master</a></li>
                  <li><a href="#variant-model" class="nav-link">Product Variant</a></li>
                  <li><a href="#bundle-model" class="nav-link">Product Bundle</a></li>
                  <li><a href="#inventory-model" class="nav-link">Inventory Record</a></li>
                </ul>
              </div>
            </nav>
          </div>
        </div>

        <!-- Main Content -->
        <div class="flex-1 max-w-none">
          <!-- API Test Section (Development) -->
          <section v-if="isDev" id="api-test" class="mb-12">
            <div class="bg-blue-50 border border-blue-200 rounded-lg p-6">
              <h2 class="text-xl font-bold text-blue-900 mb-4">API Connection Test</h2>
              <ApiTest />
            </div>
          </section>

          <!-- Overview Section -->
          <section id="overview" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Overview</h2>
            <div class="prose prose-lg max-w-none">
              <p class="text-gray-600 mb-4">
                The Product Variant Bundle API is a comprehensive RESTful API for managing product catalogs 
                with variants and bundles. Built with .NET 8 and PostgreSQL, it provides flexible schema 
                design, inventory management, and batch operations.
              </p>
              
              <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
                <h3 class="text-lg font-semibold text-blue-900 mb-2">Base URL</h3>
                <code class="text-blue-800 bg-blue-100 px-2 py-1 rounded">
                  http://localhost:8080/api
                </code>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <div class="bg-white border border-gray-200 rounded-lg p-4">
                  <h3 class="font-semibold text-gray-900 mb-2">Key Features</h3>
                  <ul class="text-sm text-gray-600 space-y-1">
                    <li>• Flexible JSONB attributes</li>
                    <li>• Global SKU management</li>
                    <li>• Virtual bundle system</li>
                    <li>• Multi-warehouse support</li>
                    <li>• Batch operations</li>
                    <li>• Real-time inventory</li>
                  </ul>
                </div>
                
                <div class="bg-white border border-gray-200 rounded-lg p-4">
                  <h3 class="font-semibold text-gray-900 mb-2">Quick Links</h3>
                  <ul class="text-sm space-y-1">
                    <li>
                      <a href="http://localhost:8080" target="_blank" 
                         class="text-blue-600 hover:text-blue-800">
                        Interactive API Documentation
                      </a>
                    </li>
                    <li>
                      <a href="http://localhost:8080/swagger/v1/swagger.json" target="_blank"
                         class="text-blue-600 hover:text-blue-800">
                        OpenAPI Specification
                      </a>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </section>

          <!-- Authentication Section -->
          <section id="authentication" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Authentication</h2>
            <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <p class="text-yellow-800">
                <strong>Note:</strong> This API currently does not require authentication. 
                It's designed for internal use or development environments.
              </p>
            </div>
          </section>

          <!-- Response Format Section -->
          <section id="response-format" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Response Format</h2>
            
            <div class="space-y-6">
              <div>
                <h3 class="text-lg font-semibold text-gray-900 mb-3">Success Response</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    // Actual response data
  },
  "meta": {
    "page": 1,
    "pageSize": 20,
    "total": 100,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  },
  "errors": []
}</code></pre>
                </div>
              </div>

              <div>
                <h3 class="text-lg font-semibold text-gray-900 mb-3">Error Response</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-red-400 text-sm"><code>{
  "data": null,
  "meta": null,
  "errors": [
    "Validation error message",
    "Another error message"
  ]
}</code></pre>
                </div>
              </div>
            </div>
          </section>

          <!-- Products Section -->
          <section id="products" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Products</h2>
            
            <div class="space-y-8">
              <!-- List Products -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/products</code>
                </div>
                <p class="text-gray-600 mb-4">List products with filtering and pagination</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Query Parameters</h4>
                <div class="overflow-x-auto">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Parameter</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Type</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">page</td>
                        <td class="px-4 py-2 text-sm">integer</td>
                        <td class="px-4 py-2 text-sm">Page number (default: 1)</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">pageSize</td>
                        <td class="px-4 py-2 text-sm">integer</td>
                        <td class="px-4 py-2 text-sm">Items per page (default: 20)</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">search</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Search in name, description</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">category</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Filter by category</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Create Product -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/products</code>
                </div>
                <p class="text-gray-600 mb-4">Create a new product master</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "name": "Gaming Laptop",
  "slug": "gaming-laptop",
  "description": "High-performance gaming laptop for professionals",
  "category": "Electronics",
  "attributes": {
    "brand": "TechBrand",
    "warranty": "2 years",
    "weight": "2.5kg"
  }
}</code></pre>
                </div>

                <h4 class="font-semibold text-gray-900 mb-2 mt-4">Response Example</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    "id": "b825c53e-028f-4233-951b-f37ee698ae42",
    "name": "Gaming Laptop",
    "slug": "gaming-laptop",
    "description": "High-performance gaming laptop for professionals",
    "category": "Electronics",
    "attributes": {
      "brand": "TechBrand",
      "warranty": "2 years",
      "weight": "2.5kg"
    },
    "variants": [],
    "status": 1,
    "createdAt": "2024-10-20T08:26:22.083046Z",
    "updatedAt": "2024-10-20T08:26:22.083046Z"
  },
  "meta": null,
  "errors": []
}</code></pre>
                </div>
              </div>

              <!-- Product Variants -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/products/{id}/variants</code>
                </div>
                <p class="text-gray-600 mb-4">Create a product variant</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "sku": "LAPTOP-16GB-512GB",
  "price": 1299.99,
  "optionValues": [
    {
      "optionName": "Memory",
      "value": "16GB"
    },
    {
      "optionName": "Storage", 
      "value": "512GB"
    }
  ],
  "attributes": {
    "weight": "2.5kg",
    "color": "Space Gray"
  }
}</code></pre>
                </div>

                <h4 class="font-semibold text-gray-900 mb-2 mt-4">Response Example</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    "id": "050cf8cf-398a-46d4-9640-6ae2f108796d",
    "productMasterId": "b825c53e-028f-4233-951b-f37ee698ae42",
    "sku": "LAPTOP-16GB-512GB",
    "price": 1299.99,
    "combinationKey": "memory:16GB|storage:512GB",
    "optionValues": [
      {
        "optionName": "Memory",
        "value": "16GB"
      },
      {
        "optionName": "Storage",
        "value": "512GB"
      }
    ],
    "attributes": {
      "weight": "2.5kg",
      "color": "Space Gray"
    },
    "status": 1,
    "createdAt": "2024-10-20T12:31:13.23561Z",
    "updatedAt": "2024-10-20T12:31:13.23561Z"
  },
  "meta": null,
  "errors": []
}</code></pre>
                </div>
              </div>
            </div>
          </section>

          <!-- Bundles Section -->
          <section id="bundles" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Bundles</h2>
            
            <div class="space-y-8">
              <!-- List Bundles -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/bundles</code>
                </div>
                <p class="text-gray-600 mb-4">List bundles with filtering and pagination</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Query Parameters</h4>
                <div class="overflow-x-auto">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Parameter</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Type</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">page</td>
                        <td class="px-4 py-2 text-sm">integer</td>
                        <td class="px-4 py-2 text-sm">Page number (default: 1)</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">pageSize</td>
                        <td class="px-4 py-2 text-sm">integer</td>
                        <td class="px-4 py-2 text-sm">Items per page (default: 20)</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">search</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Search in name, description</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Create Bundle -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/bundles</code>
                </div>
                <p class="text-gray-600 mb-4">Create a new bundle</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "name": "Gaming Setup Bundle",
  "description": "Complete gaming setup with laptop and accessories",
  "sku": "GAMING-SETUP-001",
  "price": 1599.99,
  "items": [
    {
      "sellableItemId": "fad7e86f-9c19-4a5d-9dc0-a231a242332f",
      "quantity": 1
    },
    {
      "sellableItemId": "22222222-2222-2222-2222-222222222222",
      "quantity": 1
    }
  ],
  "metadata": {
    "promotion": "Holiday Special",
    "discount": "Save $200"
  }
}</code></pre>
                </div>

                <h4 class="font-semibold text-gray-900 mb-2 mt-4">Response Example</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    "id": "c1615a98-3ae7-45e5-8c41-64f4f25378d6",
    "name": "Gaming Setup Bundle",
    "description": "Complete gaming setup with laptop and accessories",
    "sku": "GAMING-SETUP-001",
    "price": 1599.99,
    "items": [
      {
        "sellableItemId": "fad7e86f-9c19-4a5d-9dc0-a231a242332f",
        "quantity": 1,
        "sku": "LAPTOP-16GB-512GB",
        "itemName": "Gaming Laptop"
      }
    ],
    "metadata": {
      "promotion": "Holiday Special",
      "discount": "Save $200"
    },
    "status": 1,
    "createdAt": "2024-10-20T13:20:21.838686Z",
    "updatedAt": "2024-10-20T13:20:21.838686Z"
  },
  "meta": null,
  "errors": []
}</code></pre>
                </div>
              </div>

              <!-- Bundle Availability -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/bundles/{id}/availability</code>
                </div>
                <p class="text-gray-600 mb-4">Calculate bundle availability based on component inventory</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Query Parameters</h4>
                <div class="overflow-x-auto">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Parameter</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Type</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">warehouseCode</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Warehouse code (default: MAIN)</td>
                      </tr>
                    </tbody>
                  </table>
                </div>

                <h4 class="font-semibold text-gray-900 mb-2 mt-4">Response Example</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    "bundleId": "c1615a98-3ae7-45e5-8c41-64f4f25378d6",
    "warehouseCode": "MAIN",
    "availableQuantity": 25,
    "isAvailable": true,
    "componentAvailability": [
      {
        "sellableItemId": "fad7e86f-9c19-4a5d-9dc0-a231a242332f",
        "sku": "LAPTOP-16GB-512GB",
        "requiredQuantity": 1,
        "availableQuantity": 45,
        "maxBundles": 45
      },
      {
        "sellableItemId": "22222222-2222-2222-2222-222222222222",
        "sku": "MOUSE-WIRELESS-001",
        "requiredQuantity": 1,
        "availableQuantity": 25,
        "maxBundles": 25
      }
    ],
    "calculatedAt": "2024-10-20T14:30:00Z"
  },
  "meta": null,
  "errors": []
}</code></pre>
                </div>
                
                <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mt-4">
                  <p class="text-blue-800 text-sm">
                    <strong>Note:</strong> Bundle availability is calculated dynamically based on component 
                    inventory levels using the formula: min(floor((on_hand - reserved) / required_quantity))
                  </p>
                </div>
              </div>
            </div>
          </section>

          <!-- Inventory Section -->
          <section id="inventory" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Inventory</h2>
            
            <div class="space-y-8">
              <!-- List Inventory -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/inventory</code>
                </div>
                <p class="text-gray-600 mb-4">List inventory records with filtering</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Query Parameters</h4>
                <div class="overflow-x-auto">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Parameter</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Type</th>
                        <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">warehouse</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Filter by warehouse code</td>
                      </tr>
                      <tr>
                        <td class="px-4 py-2 text-sm font-mono">search</td>
                        <td class="px-4 py-2 text-sm">string</td>
                        <td class="px-4 py-2 text-sm">Search by SKU</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Update Stock -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-yellow-100 text-yellow-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    PUT
                  </span>
                  <code class="text-lg font-mono">/inventory/{sku}/stock</code>
                </div>
                <p class="text-gray-600 mb-4">Update stock levels (both on-hand and reserved)</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "SKU": "ITEM-SKU",
  "OnHand": 100,
  "Reserved": 10,
  "WarehouseCode": "MAIN"
}</code></pre>
                </div>
              </div>

              <!-- Reserve Stock -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/inventory/{sku}/reserve</code>
                </div>
                <p class="text-gray-600 mb-4">Reserve stock for pending orders</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "SKU": "ITEM-SKU",
  "Quantity": 5,
  "WarehouseCode": "MAIN"
}</code></pre>
                </div>
              </div>
            </div>
          </section>

          <!-- Batch Operations Section -->
          <section id="batch" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Batch Operations</h2>
            
            <div class="space-y-8">
              <!-- Batch Create Variants -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/batch/variants</code>
                </div>
                <p class="text-gray-600 mb-4">Create multiple variants in a single operation</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "idempotencyKey": "batch-2024-001",
  "onConflict": "skip",
  "items": [
    {
      "productMasterId": "uuid",
      "sku": "SKU-001",
      "price": 99.99,
      "optionValues": [
        {
          "optionName": "Color",
          "value": "Red"
        }
      ]
    }
  ]
}</code></pre>
                </div>
              </div>

              <!-- Batch Create Bundles -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    POST
                  </span>
                  <code class="text-lg font-mono">/batch/bundles</code>
                </div>
                <p class="text-gray-600 mb-4">Create multiple bundles in a single operation</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Request Body</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "idempotencyKey": "batch-bundles-001",
  "onConflict": "update",
  "items": [
    {
      "name": "Bundle Name",
      "sku": "BUNDLE-001",
      "price": 199.99,
      "items": [
        {
          "sellableItemId": "uuid",
          "quantity": 2
        }
      ]
    }
  ]
}</code></pre>
                </div>
              </div>
            </div>
          </section>

          <!-- Health Checks Section -->
          <section id="health" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Health Checks</h2>
            
            <div class="space-y-8">
              <!-- API Health -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/health</code>
                </div>
                <p class="text-gray-600 mb-4">API health status</p>
                
                <h4 class="font-semibold text-gray-900 mb-2">Response Example</h4>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "database": {
      "status": "Healthy",
      "duration": "00:00:00.0098765"
    }
  }
}</code></pre>
                </div>
              </div>

              <!-- Database Health -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded mr-3">
                    GET
                  </span>
                  <code class="text-lg font-mono">/health/database</code>
                </div>
                <p class="text-gray-600 mb-4">Database connectivity and health status</p>
              </div>
            </div>
          </section>

          <!-- Data Models Section -->
          <section id="product-model" class="mb-12">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Data Models</h2>
            
            <div class="space-y-8">
              <!-- Product Master Model -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4">Product Master</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "id": "b825c53e-028f-4233-951b-f37ee698ae42",
  "name": "Gaming Laptop",
  "slug": "gaming-laptop",
  "description": "High-performance gaming laptop",
  "category": "Electronics",
  "attributes": {
    "brand": "TechBrand",
    "warranty": "2 years",
    "weight": "2.5kg"
  },
  "variants": [
    {
      "id": "050cf8cf-398a-46d4-9640-6ae2f108796d",
      "sku": "LAPTOP-16GB-512GB",
      "price": 1299.99
    }
  ],
  "status": 1,
  "createdAt": "2024-10-20T08:26:22.083046Z",
  "updatedAt": "2024-10-20T12:31:28.425341Z"
}</code></pre>
                </div>
              </div>

              <!-- Product Variant Model -->
              <div id="variant-model" class="bg-white border border-gray-200 rounded-lg p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4">Product Variant</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "id": "050cf8cf-398a-46d4-9640-6ae2f108796d",
  "productMasterId": "b825c53e-028f-4233-951b-f37ee698ae42",
  "sku": "LAPTOP-16GB-512GB",
  "price": 1299.99,
  "combinationKey": "memory:16GB|storage:512GB",
  "optionValues": [
    {
      "optionName": "Memory",
      "optionSlug": "memory",
      "value": "16GB"
    },
    {
      "optionName": "Storage",
      "optionSlug": "storage", 
      "value": "512GB"
    }
  ],
  "attributes": {
    "weight": "2.5kg",
    "color": "Space Gray"
  },
  "status": 1,
  "createdAt": "2024-10-20T12:31:13.23561Z",
  "updatedAt": "2024-10-20T12:31:13.23561Z"
}</code></pre>
                </div>
              </div>

              <!-- Product Bundle Model -->
              <div id="bundle-model" class="bg-white border border-gray-200 rounded-lg p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4">Product Bundle</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "id": "c1615a98-3ae7-45e5-8c41-64f4f25378d6",
  "name": "Gaming Setup Bundle",
  "description": "Complete gaming setup with laptop and accessories",
  "sku": "GAMING-SETUP-001",
  "price": 1599.99,
  "items": [
    {
      "sellableItemId": "fad7e86f-9c19-4a5d-9dc0-a231a242332f",
      "quantity": 1,
      "bundleId": "c1615a98-3ae7-45e5-8c41-64f4f25378d6",
      "sku": "LAPTOP-16GB-512GB",
      "itemName": "Gaming Laptop",
      "id": "806a195d-66e9-4067-960d-b1b97f5fbf0d"
    },
    {
      "sellableItemId": "22222222-2222-2222-2222-222222222222",
      "quantity": 1,
      "bundleId": "c1615a98-3ae7-45e5-8c41-64f4f25378d6",
      "sku": "MOUSE-WIRELESS-001",
      "itemName": "Wireless Gaming Mouse",
      "id": "907b296e-77f0-5178-a71e-ca969869629f"
    }
  ],
  "metadata": {
    "promotion": "Holiday Special",
    "discount": "Save $200"
  },
  "status": 1,
  "createdAt": "2024-10-20T13:20:21.838686Z",
  "updatedAt": "2024-10-20T13:20:21.838686Z"
}</code></pre>
                </div>
              </div>

              <!-- Inventory Record Model -->
              <div id="inventory-model" class="bg-white border border-gray-200 rounded-lg p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4">Inventory Record</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "id": "50828a0c-3c51-46a5-96ec-afae6375ccb7",
  "sellableItemId": "fad7e86f-9c19-4a5d-9dc0-a231a242332f",
  "warehouseId": "00000000-0000-0000-0000-000000000001",
  "sku": "GAMING-SETUP-001",
  "onHand": 50,
  "reserved": 5,
  "available": 45,
  "warehouseCode": "MAIN",
  "status": 1,
  "createdAt": "2024-10-20T14:34:41.448652Z",
  "updatedAt": "2024-10-20T14:34:53.0998438Z"
}</code></pre>
                </div>
              </div>

              <!-- Batch Response Model -->
              <div class="bg-white border border-gray-200 rounded-lg p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4">Batch Operation Response</h3>
                <div class="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre class="text-green-400 text-sm"><code>{
  "data": {
    "successCount": 8,
    "failureCount": 2,
    "totalProcessed": 10,
    "idempotencyKey": "batch-2024-001",
    "onConflict": "skip",
    "results": [
      {
        "index": 0,
        "success": true,
        "data": {
          "id": "uuid",
          "sku": "CREATED-SKU-001",
          "price": 99.99
        },
        "errors": []
      },
      {
        "index": 1,
        "success": false,
        "data": null,
        "errors": ["SKU already exists"]
      }
    ]
  },
  "meta": {
    "operation": "batch_create_variants",
    "timestamp": "2024-10-20T10:00:00Z"
  },
  "errors": []
}</code></pre>
                </div>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import ApiTest from '@/components/ApiTest.vue'
import { onMounted } from 'vue'

const isDev = import.meta.env.DEV

onMounted(() => {
  // Smooth scrolling for navigation links
  const links = document.querySelectorAll('a[href^="#"]')
  links.forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault()
      const target = document.querySelector(link.getAttribute('href')!)
      if (target) {
        target.scrollIntoView({ behavior: 'smooth' })
      }
    })
  })
})
</script>

<style scoped>
.nav-link {
  @apply block px-3 py-2 text-sm text-gray-600 hover:text-gray-900 hover:bg-gray-50 rounded-md transition-colors;
}
</style>