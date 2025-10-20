<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Bundles</h1>
        <p class="mt-1 text-sm text-gray-500">
          Manage your product bundles and packages
        </p>
      </div>
      <button 
        @click="showBundleForm = true"
        class="btn btn-primary"
      >
        <PlusIcon class="w-4 h-4 mr-2" />
        Create Bundle
      </button>
    </div>

    <!-- Bundles Table -->
    <DataTable
      title="Product Bundles"
      description="All bundles in your catalog"
      :columns="columns"
      :data="bundles"
      :loading="isLoading"
      :pagination="pagination"
      :sort-by="filter.sortBy"
      :sort-direction="filter.sortDirection"
      @search="onSearch"
      @sort="onSort"
      @page-change="onPageChange"
    >
      <template #cell-name="{ item }">
        <div class="flex items-center">
          <div class="flex-shrink-0 h-10 w-10">
            <div class="h-10 w-10 rounded-lg bg-primary-100 flex items-center justify-center">
              <RectangleGroupIcon class="h-5 w-5 text-primary-600" />
            </div>
          </div>
          <div class="ml-4">
            <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
            <div class="text-sm text-gray-500">{{ item.sellableItem?.sku }}</div>
          </div>
        </div>
      </template>

      <template #cell-items="{ item }">
        <div class="text-sm text-gray-900">
          {{ item.items?.length || 0 }} items
        </div>
        <div class="text-xs text-gray-500">
          {{ getTotalQuantity(item.items) }} total qty
        </div>
      </template>

      <template #cell-price="{ value }">
        <span class="text-sm font-medium text-gray-900">
          ${{ value?.toFixed(2) }}
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

      <template #cell-availability="{ item }">
        <div class="flex items-center">
          <div class="flex-shrink-0 w-2 h-2 rounded-full mr-2"
               :class="getAvailabilityColor(item)">
          </div>
          <span class="text-sm text-gray-900">
            {{ getAvailabilityText(item) }}
          </span>
        </div>
      </template>

      <template #cell-actions="{ item }">
        <div class="flex items-center space-x-2">
          <button 
            @click="editBundle(item)"
            class="text-primary-600 hover:text-primary-900 text-sm font-medium"
          >
            Edit
          </button>
          <button class="text-gray-600 hover:text-gray-900 text-sm font-medium">
            Availability
          </button>
          <button 
            @click="deleteBundle(item)"
            class="text-red-600 hover:text-red-900 text-sm font-medium"
          >
            Delete
          </button>
        </div>
      </template>
    </DataTable>



    <!-- Bundle Form Modal -->
    <BundleForm
      v-if="showBundleForm"
      :bundle="editingBundle"
      @close="onBundleFormClose"
      @saved="onBundleSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { PlusIcon, RectangleGroupIcon } from '@heroicons/vue/24/outline'
import DataTable from '@/components/DataTable.vue'
import BundleForm from '@/components/forms/BundleForm.vue'
import { bundleApi } from '@/services/api'
import type { ProductBundle, BundleFilter, BundleItem } from '@/types/api'

const queryClient = useQueryClient()
const showBundleForm = ref(false)
const editingBundle = ref<ProductBundle | null>(null)



const filter = reactive<BundleFilter>({
  page: 1,
  pageSize: 10,
  search: '',
  sortBy: 'name',
  sortDirection: 'asc'
})

const columns = [
  { key: 'name', label: 'Bundle', sortable: true },
  { key: 'items', label: 'Items', align: 'center' as const },
  { key: 'price', label: 'Price', sortable: true, align: 'right' as const },
  { key: 'availability', label: 'Availability' },
  { key: 'status', label: 'Status', sortable: true },
  { key: 'createdAt', label: 'Created', sortable: true, format: 'date' as const },
  { key: 'actions', label: 'Actions', align: 'right' as const }
]

const { data, isLoading } = useQuery({
  queryKey: ['bundles', filter],
  queryFn: () => bundleApi.getBundles(filter),
  keepPreviousData: true
})

const bundles = computed(() => data.value?.data || [])
const pagination = computed(() => data.value?.meta)

const getTotalQuantity = (items: BundleItem[] = []) => {
  return items.reduce((total, item) => total + item.quantity, 0)
}

const getAvailabilityColor = (bundle: ProductBundle) => {
  // Mock availability logic - in real app, this would come from API
  const random = Math.random()
  if (random > 0.7) return 'bg-green-500'
  if (random > 0.3) return 'bg-yellow-500'
  return 'bg-red-500'
}

const getAvailabilityText = (bundle: ProductBundle) => {
  // Mock availability logic - in real app, this would come from API
  const random = Math.random()
  if (random > 0.7) return 'In Stock'
  if (random > 0.3) return 'Limited'
  return 'Out of Stock'
}

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

// Bundle form management
const editBundle = (bundle: ProductBundle) => {
  editingBundle.value = bundle
  showBundleForm.value = true
}

const deleteBundle = async (bundle: ProductBundle) => {
  if (!confirm(`Are you sure you want to delete "${bundle.name}"?`)) return
  
  try {
    await bundleApi.deleteBundle(bundle.id)
    queryClient.invalidateQueries({ queryKey: ['bundles'] })
  } catch (error) {
    console.error('Delete bundle error:', error)
    alert('Failed to delete bundle. Please try again.')
  }
}

const onBundleFormClose = () => {
  showBundleForm.value = false
  editingBundle.value = null
}

const onBundleSaved = (bundle: ProductBundle) => {
  showBundleForm.value = false
  editingBundle.value = null
  // Data will be refreshed automatically by React Query
}
</script>