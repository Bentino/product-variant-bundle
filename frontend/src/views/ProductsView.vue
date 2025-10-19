<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Products</h1>
        <p class="mt-1 text-sm text-gray-500">
          Manage your product catalog and variants
        </p>
      </div>
      <button @click="openCreateProduct" class="btn btn-primary">
        <PlusIcon class="w-4 h-4 mr-2" />
        Add Product
      </button>
    </div>

    <!-- Products Table -->
    <DataTable
      title="Product Masters"
      description="All products in your catalog"
      :columns="columns"
      :data="products"
      :loading="isLoading"
      :pagination="pagination"
      :sort-by="filter.sortBy"
      :sort-direction="filter.sortDirection"
      @search="onSearch"
      @sort="onSort"
      @page-change="onPageChange"
    >
      <template #actions>
        <select
          v-model="filter.category"
          class="input w-40"
          @change="onFilterChange"
        >
          <option value="">All Categories</option>
          <option value="Electronics">Electronics</option>
          <option value="Clothing">Clothing</option>
          <option value="Books">Books</option>
        </select>
      </template>

      <template #cell-name="{ item }">
        <div class="flex items-center">
          <div class="flex-shrink-0 h-10 w-10">
            <div class="h-10 w-10 rounded-lg bg-gray-200 flex items-center justify-center">
              <CubeIcon class="h-5 w-5 text-gray-500" />
            </div>
          </div>
          <div class="ml-4">
            <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
            <div class="text-sm text-gray-500">{{ item.slug }}</div>
          </div>
        </div>
      </template>

      <template #cell-category="{ value }">
        <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-primary-100 text-primary-800">
          {{ value }}
        </span>
      </template>

      <template #cell-variants="{ item }">
        <span class="text-sm text-gray-900">
          {{ item.variants?.length || 0 }} variants
        </span>
      </template>

      <template #cell-status="{ value }">
        <span 
          class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
          :class="value === 1 
            ? 'bg-green-100 text-green-800' 
            : value === 0 
            ? 'bg-yellow-100 text-yellow-800'
            : 'bg-red-100 text-red-800'"
        >
          {{ value === 1 ? 'Active' : value === 0 ? 'Inactive' : 'Archived' }}
        </span>
      </template>

      <template #cell-actions="{ item }">
        <div class="flex items-center space-x-2">
          <button 
            @click="openEditProduct(item)"
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
      </template>
    </DataTable>

    <!-- Modals -->
    <ProductForm
      v-if="showProductForm"
      :product="editingProduct"
      @close="showProductForm = false"
      @saved="onProductSaved"
    />

    <ConfirmDialog
      v-if="showDeleteDialog"
      title="Delete Product"
      message="Are you sure you want to delete this product?"
      :item-name="deletingProduct?.name"
      warning="This action cannot be undone. All variants will also be deleted."
      confirm-text="Delete"
      loading-text="Deleting..."
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { PlusIcon, CubeIcon } from '@heroicons/vue/24/outline'
import DataTable from '@/components/DataTable.vue'
import ProductForm from '@/components/forms/ProductForm.vue'
import ConfirmDialog from '@/components/forms/ConfirmDialog.vue'
import { productApi } from '@/services/api'
import type { ProductMaster, ProductFilter, PaginationMeta } from '@/types/api'

const filter = reactive<ProductFilter>({
  page: 1,
  pageSize: 10,
  search: '',
  category: '',
  sortBy: 'name',
  sortDirection: 'asc'
})

// Modal states
const showProductForm = ref(false)
const showDeleteDialog = ref(false)
const editingProduct = ref<ProductMaster | null>(null)
const deletingProduct = ref<ProductMaster | null>(null)

const queryClient = useQueryClient()

const columns = [
  { key: 'name', label: 'Product', sortable: true },
  { key: 'category', label: 'Category', sortable: true },
  { key: 'variants', label: 'Variants', align: 'center' as const },
  { key: 'status', label: 'Status', sortable: true },
  { key: 'createdAt', label: 'Created', sortable: true, format: 'date' as const },
  { key: 'actions', label: 'Actions', align: 'right' as const }
]

const { data, isLoading, refetch } = useQuery({
  queryKey: ['products', filter],
  queryFn: () => productApi.getProducts(filter),
  keepPreviousData: true
})

const products = computed(() => data.value?.data || [])
const pagination = computed(() => data.value?.meta)

const onSearch = (query: string) => {
  filter.search = query
  filter.page = 1
}

const onSort = (column: string, direction: 'asc' | 'desc') => {
  filter.sortBy = column
  filter.sortDirection = direction
  filter.page = 1
}

const onPageChange = (page: number) => {
  filter.page = page
}

const onFilterChange = () => {
  filter.page = 1
}

// CRUD Operations
const openCreateProduct = () => {
  editingProduct.value = null
  showProductForm.value = true
}

const openEditProduct = (product: ProductMaster) => {
  editingProduct.value = product
  showProductForm.value = true
}

const confirmDelete = (product: ProductMaster) => {
  deletingProduct.value = product
  showDeleteDialog.value = true
}

// Delete mutation
const deleteProductMutation = useMutation({
  mutationFn: (id: string) => productApi.deleteProduct(id),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['products'] })
    showDeleteDialog.value = false
    deletingProduct.value = null
  }
})

const handleDelete = async () => {
  if (!deletingProduct.value) return
  await deleteProductMutation.mutateAsync(deletingProduct.value.id)
}

const onProductSaved = () => {
  showProductForm.value = false
  editingProduct.value = null
}
</script>