<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Inventory</h1>
        <p class="mt-1 text-sm text-gray-500">
          Monitor stock levels and manage inventory
        </p>
      </div>
      <div class="flex items-center space-x-3">
        <button class="btn btn-primary" @click="showUpdateStockModal = true">
          <PlusIcon class="w-4 h-4 mr-2" />
          Update Stock
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="card p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <CubeIcon class="h-8 w-8 text-primary-600" />
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">
                Total Items
              </dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ stats.totalItems }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="card p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <CheckCircleIcon class="h-8 w-8 text-green-600" />
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">
                In Stock
              </dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ stats.inStock }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="card p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <ExclamationTriangleIcon class="h-8 w-8 text-yellow-600" />
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">
                Low Stock
              </dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ stats.lowStock }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="card p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <XCircleIcon class="h-8 w-8 text-red-600" />
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">
                Out of Stock
              </dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ stats.outOfStock }}
              </dd>
            </dl>
          </div>
        </div>
      </div>
    </div>

    <!-- Inventory Table -->
    <DataTable
      title="Inventory Records"
      description="Current stock levels across all warehouses"
      :columns="columns"
      :data="inventoryRecords"
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
          v-model="filter.warehouse"
          class="input w-40"
          @change="onFilterChange"
        >
          <option value="">All Warehouses</option>
          <option value="MAIN">Main Warehouse</option>
          <option value="WEST">West Coast</option>
          <option value="EAST">East Coast</option>
        </select>
      </template>

      <template #cell-sku="{ value, item }">
        <div>
          <div class="text-sm font-medium text-gray-900">{{ value }}</div>
          <div class="text-sm text-gray-500">
            {{ item.sellableItem?.type === 0 ? 'Product Variant' : 'Bundle' }}
          </div>
        </div>
      </template>

      <template #cell-onHand="{ value }">
        <span class="text-sm font-medium text-gray-900">{{ value }}</span>
      </template>

      <template #cell-reserved="{ value }">
        <span class="text-sm text-gray-600">{{ value }}</span>
      </template>

      <template #cell-available="{ value }">
        <span 
          class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
          :class="getStockLevelClass(value)"
        >
          {{ value }}
        </span>
      </template>

      <template #cell-warehouseCode="{ value }">
        <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
          {{ value }}
        </span>
      </template>

      <template #cell-actions="{ item }">
        <div class="flex items-center space-x-2">
          <button 
            class="text-primary-600 hover:text-primary-900 text-sm font-medium"
            @click="openUpdateModal(item)"
          >
            Update
          </button>
          <button 
            class="text-gray-600 hover:text-gray-900 text-sm font-medium"
            @click="openReserveModal(item)"
          >
            Manage
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Update Stock Modal -->
    <Modal 
      v-model="showUpdateStockModal" 
      title="Update Stock"
      size="md"
    >
      <form @submit.prevent="handleUpdateStock" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Select Item
          </label>
          <select 
            v-model="updateStockForm.sku" 
            class="input w-full"
            required
          >
            <option value="">Select an item...</option>
            <option 
              v-for="item in inventoryRecords" 
              :key="item.id" 
              :value="item.sku"
            >
              {{ item.sku }} (Current: {{ item.onHand }})
            </option>
          </select>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              On Hand
            </label>
            <input 
              v-model.number="updateStockForm.onHand" 
              type="number" 
              min="0"
              class="input w-full"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Reserved
            </label>
            <input 
              v-model.number="updateStockForm.reserved" 
              type="number" 
              min="0"
              class="input w-full"
              required
            />
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Warehouse
          </label>
          <select v-model="updateStockForm.warehouseCode" class="input w-full">
            <option value="MAIN">Main Warehouse</option>
          </select>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button 
            type="button" 
            class="btn btn-secondary"
            @click="showUpdateStockModal = false"
          >
            Cancel
          </button>
          <button 
            type="submit" 
            class="btn btn-primary"
            :disabled="updateStockMutation.isPending.value"
          >
            {{ updateStockMutation.isPending.value ? 'Updating...' : 'Update Stock' }}
          </button>
        </div>
      </form>
    </Modal>

    <!-- Item Update Modal -->
    <Modal 
      v-model="showItemUpdateModal" 
      title="Update Item Stock"
      size="md"
    >
      <form @submit.prevent="handleItemUpdate" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            SKU
          </label>
          <input 
            :value="selectedItem?.sku" 
            class="input w-full bg-gray-50"
            readonly
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              On Hand
            </label>
            <input 
              v-model.number="itemUpdateForm.onHand" 
              type="number" 
              min="0"
              class="input w-full"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Reserved
            </label>
            <input 
              v-model.number="itemUpdateForm.reserved" 
              type="number" 
              min="0"
              class="input w-full"
              required
            />
          </div>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button 
            type="button" 
            class="btn btn-secondary"
            @click="showItemUpdateModal = false"
          >
            Cancel
          </button>
          <button 
            type="submit" 
            class="btn btn-primary"
            :disabled="itemUpdateMutation.isPending.value"
          >
            {{ itemUpdateMutation.isPending.value ? 'Updating...' : 'Update' }}
          </button>
        </div>
      </form>
    </Modal>

    <!-- Reserve/Release Stock Modal -->
    <Modal 
      v-model="showReserveModal" 
      title="Manage Stock Reservation"
      size="md"
    >
      <form @submit.prevent="handleReservationAction" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            SKU
          </label>
          <input 
            :value="selectedItem?.sku" 
            class="input w-full bg-gray-50"
            readonly
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Available Stock
            </label>
            <input 
              :value="selectedItem?.available" 
              class="input w-full bg-gray-50"
              readonly
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Currently Reserved
            </label>
            <input 
              :value="selectedItem?.reserved" 
              class="input w-full bg-gray-50"
              readonly
            />
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Action
          </label>
          <select v-model="reserveForm.action" class="input w-full" @change="onActionChange">
            <option value="reserve">Reserve Stock</option>
            <option value="release">Release Reserved Stock</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            {{ reserveForm.action === 'reserve' ? 'Quantity to Reserve' : 'Quantity to Release' }}
          </label>
          <input 
            v-model.number="reserveForm.quantity" 
            type="number" 
            min="1"
            :max="reserveForm.action === 'reserve' ? (selectedItem?.available || 0) : (selectedItem?.reserved || 0)"
            class="input w-full"
            required
          />
          <p class="text-xs text-gray-500 mt-1">
            {{ reserveForm.action === 'reserve' 
              ? `Max: ${selectedItem?.available || 0} (available)` 
              : `Max: ${selectedItem?.reserved || 0} (currently reserved)` 
            }}
          </p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Warehouse
          </label>
          <select v-model="reserveForm.warehouseCode" class="input w-full">
            <option value="MAIN">Main Warehouse</option>
          </select>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button 
            type="button" 
            class="btn btn-secondary"
            @click="showReserveModal = false"
          >
            Cancel
          </button>
          <button 
            type="submit" 
            class="btn"
            :class="reserveForm.action === 'reserve' ? 'btn-primary' : 'btn-warning'"
            :disabled="reserveStockMutation.isPending.value || releaseStockMutation.isPending.value"
          >
            {{ getReservationButtonText() }}
          </button>
        </div>
      </form>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import {
  PlusIcon,
  CubeIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  XCircleIcon
} from '@heroicons/vue/24/outline'
import DataTable from '@/components/DataTable.vue'
import Modal from '@/components/Modal.vue'
import { inventoryApi } from '@/services/api'
import type { InventoryRecord } from '@/types/api'

// Filter state
const filter = reactive({
  page: 1,
  pageSize: 10,
  search: '',
  warehouse: '',
  sortBy: 'sku',
  sortDirection: 'asc' as 'asc' | 'desc'
})

const columns = [
  { key: 'sku', label: 'SKU', sortable: true },
  { key: 'onHand', label: 'On Hand', sortable: true, align: 'right' as const },
  { key: 'reserved', label: 'Reserved', sortable: true, align: 'right' as const },
  { key: 'available', label: 'Available', sortable: true, align: 'right' as const },
  { key: 'warehouseCode', label: 'Warehouse' },
  { key: 'lastUpdated', label: 'Last Updated', sortable: true, format: 'datetime' as const },
  { key: 'actions', label: 'Actions', align: 'right' as const }
]

// Fetch inventory data
const { data: inventoryData, isLoading } = useQuery({
  queryKey: ['inventory', filter],
  queryFn: () => inventoryApi.getInventoryRecords(filter),
  keepPreviousData: true
})

const inventoryRecords = computed(() => inventoryData.value?.data || [])
const pagination = computed(() => inventoryData.value?.meta)

const queryClient = useQueryClient()

// Modal states
const showUpdateStockModal = ref(false)
const showItemUpdateModal = ref(false)
const showReserveModal = ref(false)
const selectedItem = ref<InventoryRecord | null>(null)

// Form states
const updateStockForm = reactive({
  sku: '',
  onHand: 0,
  reserved: 0,
  warehouseCode: 'MAIN'
})

const itemUpdateForm = reactive({
  onHand: 0,
  reserved: 0
})

const reserveForm = reactive({
  quantity: 1,
  warehouseCode: 'MAIN',
  action: 'reserve' as 'reserve' | 'release'
})



// Calculate stats
const stats = computed(() => {
  const records = inventoryRecords.value
  const totalItems = pagination.value?.total || 0
  const inStock = records.filter(r => r.available > 0).length
  const lowStock = records.filter(r => r.available > 0 && r.available < 10).length
  const outOfStock = records.filter(r => r.available === 0).length
  
  return {
    totalItems,
    inStock,
    lowStock,
    outOfStock
  }
})

// Methods
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

// Mutations
const updateStockMutation = useMutation({
  mutationFn: (data: { sku: string; onHand: number; reserved: number; warehouseCode: string }) =>
    inventoryApi.updateStock(data.sku, {
      sku: data.sku,
      onHand: data.onHand,
      reserved: data.reserved,
      warehouseCode: data.warehouseCode
    }),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['inventory'] })
    showUpdateStockModal.value = false
    resetUpdateStockForm()
  }
})

const itemUpdateMutation = useMutation({
  mutationFn: (data: { sku: string; onHand: number; reserved: number; warehouseCode: string }) =>
    inventoryApi.updateStock(data.sku, {
      sku: data.sku,
      onHand: data.onHand,
      reserved: data.reserved,
      warehouseCode: data.warehouseCode
    }),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['inventory'] })
    showItemUpdateModal.value = false
    selectedItem.value = null
  }
})

const reserveStockMutation = useMutation({
  mutationFn: (data: { sku: string; quantity: number; warehouseCode: string }) =>
    inventoryApi.reserveStock(data.sku, {
      sku: data.sku,
      quantity: data.quantity,
      warehouseCode: data.warehouseCode
    }),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['inventory'] })
    showReserveModal.value = false
    selectedItem.value = null
    resetReserveForm()
  }
})

const releaseStockMutation = useMutation({
  mutationFn: (data: { sku: string; quantity: number; warehouseCode: string }) =>
    inventoryApi.releaseStock(data.sku, {
      sku: data.sku,
      quantity: data.quantity,
      warehouseCode: data.warehouseCode
    }),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['inventory'] })
    showReserveModal.value = false
    selectedItem.value = null
    resetReserveForm()
  }
})

// Helper functions
const resetUpdateStockForm = () => {
  updateStockForm.sku = ''
  updateStockForm.onHand = 0
  updateStockForm.reserved = 0
  updateStockForm.warehouseCode = 'MAIN'
}

const resetReserveForm = () => {
  reserveForm.quantity = 1
  reserveForm.warehouseCode = 'MAIN'
  reserveForm.action = 'reserve'
}

const openUpdateModal = (item: InventoryRecord) => {
  selectedItem.value = item
  itemUpdateForm.onHand = item.onHand
  itemUpdateForm.reserved = item.reserved
  showItemUpdateModal.value = true
}

const openReserveModal = (item: InventoryRecord) => {
  selectedItem.value = item
  reserveForm.quantity = 1
  reserveForm.warehouseCode = item.warehouseCode
  reserveForm.action = 'reserve'
  showReserveModal.value = true
}

// Form handlers
const handleUpdateStock = () => {
  updateStockMutation.mutate({
    sku: updateStockForm.sku,
    onHand: updateStockForm.onHand,
    reserved: updateStockForm.reserved,
    warehouseCode: updateStockForm.warehouseCode
  })
}

const handleItemUpdate = () => {
  if (!selectedItem.value) return
  
  itemUpdateMutation.mutate({
    sku: selectedItem.value.sku,
    onHand: itemUpdateForm.onHand,
    reserved: itemUpdateForm.reserved,
    warehouseCode: selectedItem.value.warehouseCode
  })
}

const handleReservationAction = () => {
  if (!selectedItem.value) return
  
  const mutationData = {
    sku: selectedItem.value.sku,
    quantity: reserveForm.quantity,
    warehouseCode: reserveForm.warehouseCode
  }
  
  if (reserveForm.action === 'reserve') {
    reserveStockMutation.mutate(mutationData)
  } else {
    releaseStockMutation.mutate(mutationData)
  }
}

const onActionChange = () => {
  // Reset quantity when action changes
  reserveForm.quantity = 1
}

const getReservationButtonText = () => {
  const isLoading = reserveStockMutation.isPending.value || releaseStockMutation.isPending.value
  
  if (isLoading) {
    return reserveForm.action === 'reserve' ? 'Reserving...' : 'Releasing...'
  }
  
  return reserveForm.action === 'reserve' ? 'Reserve Stock' : 'Release Stock'
}

const getStockLevelClass = (available: number) => {
  if (available === 0) return 'bg-red-100 text-red-800'
  if (available < 10) return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}
</script>