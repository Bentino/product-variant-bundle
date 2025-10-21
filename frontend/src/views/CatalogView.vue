<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Product Catalog</h1>
        <p class="mt-1 text-sm text-gray-500">
          All products and bundles in your catalog
        </p>
      </div>
      <div class="flex items-center space-x-3">
        <button @click="openCreateProduct" class="btn btn-secondary">
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Product
        </button>
        <button @click="openCreateBundle" class="btn btn-primary">
          <RectangleGroupIcon class="w-4 h-4 mr-2" />
          Create Bundle
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="card p-4">
      <div class="flex flex-col sm:flex-row sm:items-center space-y-3 sm:space-y-0 sm:space-x-4">
        <!-- Search -->
        <div class="flex-1">
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search products and bundles..."
              class="input pl-10 w-full"
              @input="onSearch"
            />
          </div>
        </div>
        
        <!-- Type Filter -->
        <select
          v-model="filter.type"
          class="input w-40"
          @change="onFilterChange"
        >
          <option value="">All Items</option>
          <option value="product">Products Only</option>
          <option value="bundle">Bundles Only</option>
        </select>
        
        <!-- Category Filter -->
        <select
          v-model="filter.category"
          class="input w-40"
          @change="onFilterChange"
        >
          <option value="">- All Categories -</option>
          <option value="Computer Hardware">Computer Hardware</option>
          <option value="Mobile Phone">Mobile Phone</option>
          <option value="Accessory">Accessory</option>
        </select>
        
        <!-- Status Filter -->
        <select
          v-model="filter.status"
          class="input w-32"
          @change="onFilterChange"
        >
          <option value="">All Status</option>
          <option value="0">Active</option>
          <option value="1">Inactive</option>
        </select>
      </div>
    </div>

    <!-- Items Grid/List -->
    <div class="card">
      <!-- View Toggle -->
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <h3 class="text-lg font-medium text-gray-900">
              {{ filteredItems.length }} items
            </h3>
            <div class="text-sm text-gray-500">
              {{ productCount }} products, {{ bundleCount }} bundles
            </div>
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="viewMode = 'grid'"
              class="p-2 rounded-md"
              :class="viewMode === 'grid' 
                ? 'bg-primary-100 text-primary-600' 
                : 'text-gray-400 hover:text-gray-600'"
            >
              <Squares2X2Icon class="w-5 h-5" />
            </button>
            <button
              @click="viewMode = 'list'"
              class="p-2 rounded-md"
              :class="viewMode === 'list' 
                ? 'bg-primary-100 text-primary-600' 
                : 'text-gray-400 hover:text-gray-600'"
            >
              <ListBulletIcon class="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="p-12 text-center">
        <div class="flex items-center justify-center">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-500"></div>
          <span class="ml-3 text-gray-500">Loading catalog...</span>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!filteredItems.length" class="p-12 text-center">
        <CubeIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">No items found</h3>
        <p class="mt-1 text-sm text-gray-500">
          Try adjusting your search or filter criteria
        </p>
      </div>

      <!-- Grid View -->
      <div v-else-if="viewMode === 'grid'" class="p-6">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <div
            v-for="item in paginatedItems"
            :key="item.id"
            class="group relative bg-white border border-gray-200 rounded-lg hover:shadow-md transition-shadow"
          >
            <!-- Item Image/Icon -->
            <div class="aspect-w-1 aspect-h-1 w-full overflow-hidden rounded-t-lg bg-gray-100">
              <div class="flex items-center justify-center h-48">
                <component 
                  :is="item.type === 'bundle' ? RectangleGroupIcon : CubeIcon"
                  class="h-12 w-12"
                  :class="item.type === 'bundle' ? 'text-primary-500' : 'text-gray-400'"
                />
              </div>
            </div>
            
            <!-- Item Info -->
            <div class="p-4">
              <div class="flex items-start justify-between">
                <div class="flex-1 min-w-0">
                  <h3 class="text-sm font-medium text-gray-900 truncate">
                    {{ item.name }}
                  </h3>
                  <p class="text-xs text-gray-500 mt-1">
                    {{ item.type === 'bundle' ? item.sku : item.slug }}
                  </p>
                </div>
                <span 
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ml-2"
                  :class="item.type === 'bundle' 
                    ? 'bg-primary-100 text-primary-800' 
                    : 'bg-gray-100 text-gray-800'"
                >
                  {{ item.type === 'bundle' ? 'Bundle' : 'Product' }}
                </span>
              </div>
              
              <!-- Category/Items -->
              <div class="mt-2">
                <p class="text-xs text-gray-500">
                  <span v-if="item.type === 'product'">{{ item.category }}</span>
                  <span v-else>{{ item.items?.length || 0 }} items</span>
                </p>
              </div>
              
              <!-- Price -->
              <div class="mt-2 flex items-center justify-between">
                <span class="text-sm font-medium text-gray-900">
                  ${{ item.price?.toFixed(2) || '0.00' }}
                </span>
                <span 
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium"
                  :class="item.status === 1 
                    ? 'bg-green-100 text-green-800' 
                    : item.status === 0 
                    ? 'bg-yellow-100 text-yellow-800'
                    : 'bg-red-100 text-red-800'"
                >
                  {{ item.status === 1 ? 'Active' : item.status === 0 ? 'Inactive' : 'Archived' }}
                </span>
              </div>
              
              <!-- Actions -->
              <div class="mt-3 flex items-center space-x-2">
                <button 
                  @click="item.type === 'product' ? openEditProduct(item) : openEditBundle(item)"
                  class="flex-1 btn btn-secondary btn-sm"
                >
                  Edit
                </button>
                <button 
                  @click="confirmDelete(item)"
                  class="btn btn-secondary btn-sm text-red-600 hover:text-red-800"
                >
                  <XMarkIcon class="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else class="overflow-x-auto">
        <table class="table">
          <thead>
            <tr>
              <th class="cursor-pointer hover:bg-gray-100" @click="onSort('name')">
                <div class="flex items-center space-x-1">
                  <span>Name</span>
                  <div class="flex flex-col">
                    <ChevronUpIcon 
                      class="w-3 h-3 -mb-1"
                      :class="sortBy === 'name' && sortDirection === 'asc' 
                        ? 'text-primary-500' 
                        : 'text-gray-300'"
                    />
                    <ChevronDownIcon 
                      class="w-3 h-3"
                      :class="sortBy === 'name' && sortDirection === 'desc' 
                        ? 'text-primary-500' 
                        : 'text-gray-300'"
                    />
                  </div>
                </div>
              </th>
              <th>Type</th>
              <th>Category/Items</th>
              <th class="text-right cursor-pointer hover:bg-gray-100" @click="onSort('price')">
                <div class="flex items-center justify-end space-x-1">
                  <span>Price</span>
                  <div class="flex flex-col">
                    <ChevronUpIcon 
                      class="w-3 h-3 -mb-1"
                      :class="sortBy === 'price' && sortDirection === 'asc' 
                        ? 'text-primary-500' 
                        : 'text-gray-300'"
                    />
                    <ChevronDownIcon 
                      class="w-3 h-3"
                      :class="sortBy === 'price' && sortDirection === 'desc' 
                        ? 'text-primary-500' 
                        : 'text-gray-300'"
                    />
                  </div>
                </div>
              </th>
              <th>Status</th>
              <th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in paginatedItems" :key="item.id">
              <td>
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10">
                    <div class="h-10 w-10 rounded-lg flex items-center justify-center"
                         :class="item.type === 'bundle' ? 'bg-primary-100' : 'bg-gray-100'">
                      <component 
                        :is="item.type === 'bundle' ? RectangleGroupIcon : CubeIcon"
                        class="h-5 w-5"
                        :class="item.type === 'bundle' ? 'text-primary-600' : 'text-gray-500'"
                      />
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                    <div class="text-sm text-gray-500">
                      {{ item.type === 'bundle' ? item.sku : item.slug }}
                    </div>
                  </div>
                </div>
              </td>
              <td>
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="item.type === 'bundle' 
                    ? 'bg-primary-100 text-primary-800' 
                    : 'bg-gray-100 text-gray-800'"
                >
                  {{ item.type === 'bundle' ? 'Bundle' : 'Product' }}
                </span>
              </td>
              <td class="text-sm text-gray-900">
                <span v-if="item.type === 'product'">{{ item.category }}</span>
                <span v-else>{{ item.items?.length || 0 }} items</span>
              </td>
              <td class="text-right text-sm font-medium text-gray-900">
                ${{ item.price?.toFixed(2) || '0.00' }}
              </td>
              <td>
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="item.status === 1 
                    ? 'bg-green-100 text-green-800' 
                    : item.status === 0 
                    ? 'bg-yellow-100 text-yellow-800'
                    : 'bg-red-100 text-red-800'"
                >
                  {{ item.status === 1 ? 'Active' : item.status === 0 ? 'Inactive' : 'Archived' }}
                </span>
              </td>
              <td class="text-right">
                <div class="flex items-center justify-end space-x-2">
                  <button 
                    @click="item.type === 'product' ? openEditProduct(item) : openEditBundle(item)"
                    class="text-primary-600 hover:text-primary-900 text-sm font-medium"
                  >
                    Edit
                  </button>
                  <button 
                    @click="confirmDelete(item)"
                    class="text-red-600 hover:text-red-900 text-sm font-medium"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="px-6 py-4 border-t border-gray-200">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-3 sm:space-y-0">
          <div class="text-sm text-gray-700">
            Showing {{ ((currentPage - 1) * pageSize) + 1 }} to 
            {{ Math.min(currentPage * pageSize, filteredItems.length) }} of 
            {{ filteredItems.length }} results
          </div>
          <div class="flex items-center space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--"
              class="btn btn-secondary btn-sm"
              :class="{ 'opacity-50 cursor-not-allowed': currentPage === 1 }"
            >
              <ChevronLeftIcon class="w-4 h-4" />
              Previous
            </button>
            
            <div class="flex items-center space-x-1">
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="currentPage = page"
                class="btn btn-sm"
                :class="page === currentPage ? 'btn-primary' : 'btn-secondary'"
              >
                {{ page }}
              </button>
            </div>
            
            <button
              :disabled="currentPage === totalPages"
              @click="currentPage++"
              class="btn btn-secondary btn-sm"
              :class="{ 'opacity-50 cursor-not-allowed': currentPage === totalPages }"
            >
              Next
              <ChevronRightIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <ProductForm
      v-if="showProductForm"
      :product="editingItem as ProductMaster"
      @close="showProductForm = false"
      @saved="onProductSaved"
    />

    <BundleForm
      v-if="showBundleForm"
      :bundle="editingItem as ProductBundle"
      @close="showBundleForm = false"
      @saved="onBundleSaved"
    />

    <ConfirmDialog
      v-if="showDeleteDialog"
      :title="`Delete ${deletingItem?.type === 'product' ? 'Product' : 'Bundle'}`"
      :message="`Are you sure you want to delete this ${deletingItem?.type === 'product' ? 'product' : 'bundle'}?`"
      :item-name="deletingItem?.name"
      warning="This action cannot be undone."
      confirm-text="Delete"
      loading-text="Deleting..."
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import {
  PlusIcon,
  RectangleGroupIcon,
  MagnifyingGlassIcon,
  CubeIcon,
  Squares2X2Icon,
  ListBulletIcon,
  EyeIcon,
  ChevronUpIcon,
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { productApi, bundleApi } from '@/services/api'
import type { ProductMaster, ProductBundle } from '@/types/api'
import ProductForm from '@/components/forms/ProductForm.vue'
import BundleForm from '@/components/forms/BundleForm.vue'
import ConfirmDialog from '@/components/forms/ConfirmDialog.vue'

// State
const viewMode = ref<'grid' | 'list'>('grid')
const searchQuery = ref('')
const currentPage = ref(1)
const pageSize = ref(12)
const sortBy = ref('name')
const sortDirection = ref<'asc' | 'desc'>('asc')

// Modal states
const showProductForm = ref(false)
const showBundleForm = ref(false)
const showDeleteDialog = ref(false)
const editingItem = ref<ProductMaster | ProductBundle | null>(null)
const deletingItem = ref<any>(null)

const queryClient = useQueryClient()

const filter = reactive({
  type: '', // 'product', 'bundle', or ''
  category: '',
  status: ''
})

// Fetch data
const { data: productsData, isLoading: productsLoading } = useQuery({
  queryKey: ['products'],
  queryFn: () => productApi.getProducts({ pageSize: 1000 })
})

const { data: bundlesData, isLoading: bundlesLoading } = useQuery({
  queryKey: ['bundles'],
  queryFn: () => bundleApi.getBundles({ pageSize: 1000 })
})

const isLoading = computed(() => productsLoading.value || bundlesLoading.value)

// Combined items
const allItems = computed(() => {
  const products = (productsData.value?.data || []).map(product => ({
    ...product,
    type: 'product' as const,
    price: product.variants?.[0]?.price || 0
  }))
  
  const bundles = (bundlesData.value?.data || []).map(bundle => ({
    ...bundle,
    type: 'bundle' as const
  }))
  
  return [...products, ...bundles]
})

// Filtered items
const filteredItems = computed(() => {
  let items = allItems.value

  // Filter by type
  if (filter.type) {
    items = items.filter(item => item.type === filter.type)
  }

  // Filter by category (products only)
  if (filter.category) {
    items = items.filter(item => 
      item.type === 'product' ? item.category === filter.category : true
    )
  }

  // Filter by status
  if (filter.status !== '') {
    items = items.filter(item => item.status === parseInt(filter.status))
  }

  // Filter by search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    items = items.filter(item =>
      item.name.toLowerCase().includes(query) ||
      (item.type === 'product' && item.slug.toLowerCase().includes(query)) ||
      (item.type === 'bundle' && item.sku?.toLowerCase().includes(query))
    )
  }

  // Sort items
  items.sort((a, b) => {
    let aValue = a[sortBy.value as keyof typeof a]
    let bValue = b[sortBy.value as keyof typeof b]
    
    if (typeof aValue === 'string') aValue = aValue.toLowerCase()
    if (typeof bValue === 'string') bValue = bValue.toLowerCase()
    
    if (aValue < bValue) return sortDirection.value === 'asc' ? -1 : 1
    if (aValue > bValue) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })

  return items
})

// Pagination
const totalPages = computed(() => Math.ceil(filteredItems.value.length / pageSize.value))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredItems.value.slice(start, end)
})

const visiblePages = computed(() => {
  const current = currentPage.value
  const total = totalPages.value
  const delta = 2
  
  const range = []
  const rangeWithDots = []
  
  for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
    range.push(i)
  }
  
  if (current - delta > 2) {
    rangeWithDots.push(1, '...')
  } else {
    rangeWithDots.push(1)
  }
  
  rangeWithDots.push(...range)
  
  if (current + delta < total - 1) {
    rangeWithDots.push('...', total)
  } else {
    rangeWithDots.push(total)
  }
  
  return rangeWithDots.filter((item, index, arr) => arr.indexOf(item) === index)
})

// Counts
const productCount = computed(() => 
  filteredItems.value.filter(item => item.type === 'product').length
)

const bundleCount = computed(() => 
  filteredItems.value.filter(item => item.type === 'bundle').length
)

// Methods
const onSearch = () => {
  currentPage.value = 1
}

const onSort = (column: string) => {
  if (sortBy.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortBy.value = column
    sortDirection.value = 'asc'
  }
  currentPage.value = 1
}

const onFilterChange = () => {
  currentPage.value = 1
}

// CRUD Operations
const openCreateProduct = () => {
  editingItem.value = null
  showProductForm.value = true
}

const openEditProduct = (product: ProductMaster) => {
  editingItem.value = product
  showProductForm.value = true
}

const openCreateBundle = () => {
  editingItem.value = null
  showBundleForm.value = true
}

const openEditBundle = (bundle: ProductBundle) => {
  editingItem.value = bundle
  showBundleForm.value = true
}

const confirmDelete = (item: any) => {
  deletingItem.value = item
  showDeleteDialog.value = true
}

// Delete mutations
const deleteProductMutation = useMutation({
  mutationFn: (id: string) => productApi.deleteProduct(id),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['products'] })
    showDeleteDialog.value = false
    deletingItem.value = null
  }
})

const deleteBundleMutation = useMutation({
  mutationFn: (id: string) => bundleApi.deleteBundle(id),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['bundles'] })
    showDeleteDialog.value = false
    deletingItem.value = null
  }
})

const handleDelete = async () => {
  if (!deletingItem.value) return

  try {
    if (deletingItem.value.type === 'product') {
      await deleteProductMutation.mutateAsync(deletingItem.value.id)
    } else {
      await deleteBundleMutation.mutateAsync(deletingItem.value.id)
    }
  } catch (error) {
    console.error('Delete error:', error)
  }
}

const onProductSaved = () => {
  showProductForm.value = false
  editingItem.value = null
}

const onBundleSaved = () => {
  showBundleForm.value = false
  editingItem.value = null
}

// Watch for search changes
let searchTimeout: NodeJS.Timeout
watch(searchQuery, () => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    onSearch()
  }, 300)
})

// Reset page when filters change
watch([filter], () => {
  currentPage.value = 1
})
</script>