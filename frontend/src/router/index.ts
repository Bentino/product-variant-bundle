import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Catalog',
    component: () => import('@/views/CatalogView.vue'),
    meta: { title: 'Catalog' }
  },
  {
    path: '/products',
    name: 'Products',
    component: () => import('@/views/ProductsView.vue'),
    meta: { title: 'Products' }
  },
  {
    path: '/bundles',
    name: 'Bundles',
    component: () => import('@/views/BundlesView.vue'),
    meta: { title: 'Bundles' }
  },
  {
    path: '/inventory',
    name: 'Inventory',
    component: () => import('@/views/InventoryView.vue'),
    meta: { title: 'Inventory' }
  },
  {
    path: '/api-reference',
    name: 'ApiReference',
    component: () => import('@/views/ApiReferenceView.vue'),
    meta: { title: 'API Reference' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard for page titles
router.beforeEach((to) => {
  document.title = to.meta?.title 
    ? `${to.meta.title} - Product Variant Bundle Admin`
    : 'Product Variant Bundle Admin'
})

export default router