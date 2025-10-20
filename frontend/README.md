# Product Variant Bundle - Admin UI

A modern, responsive admin interface for managing products, variants, and bundles.

## 🎨 Design System

- **Colors**: White (#FFFFFF) + Blue (#2898CB)
- **Framework**: Vue 3 + TypeScript + Vite
- **Styling**: Tailwind CSS
- **Icons**: Heroicons
- **Components**: Headless UI

## 🚀 Features

- **Responsive Design** - Mobile and tablet friendly
- **Product Management** - CRUD operations for products and variants
- **Bundle Management** - Create and manage product bundles
- **Inventory Tracking** - Real-time stock monitoring
- **Advanced Tables** - Sorting, filtering, and pagination
- **Search & Filters** - Quick product discovery
- **Modern UI** - Clean, professional admin interface

## 🛠️ Development

### Prerequisites
- Node.js 18+
- Docker (for API backend)

### Quick Start

1. **Install dependencies:**
   ```bash
   cd frontend
   npm install
   ```

2. **Start development server:**
   ```bash
   npm run dev
   ```

3. **Access the application:**
   - Frontend: http://localhost:3000
   - API: http://localhost:8080 (must be running)

### With Docker

```bash
# Start everything (API + Frontend)
docker compose up -d

# Access points
Frontend: http://localhost:3000
API: http://localhost:8080
```

## 📱 Responsive Breakpoints

- **Mobile**: < 640px
- **Tablet**: 640px - 1024px  
- **Desktop**: > 1024px

## 🎯 Pages

### Products (`/products`)
- Product master list with variants
- Search and category filtering
- Sortable columns
- Pagination
- CRUD operations

### Bundles (`/bundles`)
- Bundle list with item composition
- Availability status
- Price management
- Bundle builder interface

### Inventory (`/inventory`)
- Stock level monitoring
- Multi-warehouse support
- Reservation tracking
- Low stock alerts

## 🔧 Tech Stack

```json
{
  "framework": "Vue 3",
  "language": "TypeScript",
  "build": "Vite",
  "styling": "Tailwind CSS",
  "components": "Headless UI",
  "icons": "Heroicons",
  "http": "Axios",
  "state": "Pinia",
  "queries": "TanStack Query"
}
```

## 📦 Project Structure

```
frontend/
├── src/
│   ├── components/          # Reusable components
│   │   └── DataTable.vue   # Advanced table component
│   ├── views/              # Page components
│   │   ├── ProductsView.vue
│   │   ├── BundlesView.vue
│   │   └── InventoryView.vue
│   ├── services/           # API services
│   │   └── api.ts
│   ├── types/              # TypeScript types
│   │   └── api.ts
│   ├── router/             # Vue Router config
│   └── style.css           # Global styles
├── public/                 # Static assets
├── Dockerfile             # Container config
└── package.json           # Dependencies
```

## 🎨 Component Library

### DataTable
Advanced table component with:
- Sorting and filtering
- Pagination
- Search
- Custom cell rendering
- Responsive design
- Loading states

### Usage Example
```vue
<DataTable
  title="Products"
  :columns="columns"
  :data="products"
  :loading="isLoading"
  :pagination="pagination"
  @search="onSearch"
  @sort="onSort"
  @page-change="onPageChange"
>
  <template #cell-name="{ item }">
    <div class="font-medium">{{ item.name }}</div>
  </template>
</DataTable>
```

## 🚀 Build & Deploy

### Development Build
```bash
npm run dev
```

### Production Build
```bash
npm run build
npm run preview
```

### Docker Production
```bash
docker build -t frontend --target production .
docker run -p 80:80 frontend
```

## 🎯 API Integration

The frontend connects to the Product Variant Bundle API:

- **Base URL**: `/api` (proxied to backend)
- **Authentication**: JWT tokens (when implemented)
- **Error Handling**: Automatic retry and user feedback
- **Caching**: TanStack Query for optimal performance

## 📱 Mobile Experience

- **Touch-friendly** - Large tap targets
- **Responsive tables** - Horizontal scroll on mobile
- **Collapsible navigation** - Mobile menu
- **Optimized forms** - Mobile-first form design

---

**Built with ❤️ for the Product Variant Bundle API assignment**