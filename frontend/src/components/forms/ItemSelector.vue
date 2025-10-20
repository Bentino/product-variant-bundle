<template>
  <Modal
    :model-value="true"
    title="Select Items for Bundle"
    size="lg"
    @update:model-value="(value) => !value && $emit('close')"
  >
    <div class="space-y-4">
      <!-- Search -->
      <div class="relative">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search items by SKU or name..."
          class="input pl-10 w-full"
        />
      </div>

      <!-- Info and Selection Controls -->
      <div class="flex items-center justify-between">
        <div class="text-sm text-gray-500">
          Showing all available product variants and bundles
        </div>
        
        <div class="flex items-center space-x-2">
          <button
            type="button"
            @click="selectAll"
            class="text-sm text-primary-600 hover:text-primary-800"
          >
            Select All
          </button>
          <span class="text-gray-300">|</span>
          <button
            type="button"
            @click="clearAll"
            class="text-sm text-gray-600 hover:text-gray-800"
          >
            Clear All
          </button>
        </div>
      </div>

      <!-- Items List -->
      <div class="max-h-96 overflow-y-auto border border-gray-200 rounded-lg">
        <div v-if="isLoading" class="p-8 text-center">
          <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-primary-500 mx-auto"></div>
          <p class="mt-2 text-sm text-gray-500">Loading items...</p>
        </div>

        <div v-else-if="!filteredItems.length" class="p-8 text-center text-gray-500">
          <CubeIcon class="mx-auto h-12 w-12 text-gray-300 mb-2" />
          <p>No items found</p>
          <p class="text-sm">Try adjusting your search or filters</p>
        </div>

        <div v-else class="divide-y divide-gray-200">
          <div
            v-for="item in filteredItems"
            :key="item.id"
            class="p-4 hover:bg-gray-50 transition-colors"
            :class="{ 'bg-primary-50': selectedItems.has(item.id) }"
          >
            <div class="flex items-center space-x-3">
              <!-- Checkbox -->
              <input
                :id="`item-${item.id}`"
                type="checkbox"
                :checked="selectedItems.has(item.id)"
                @change="toggleItem(item)"
                class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
              />
              
              <!-- Item Icon -->
              <div class="flex-shrink-0">
                <div class="w-10 h-10 rounded-lg flex items-center justify-center"
                     :class="item.type === 1 ? 'bg-primary-100' : 'bg-gray-100'">
                  <component 
                    :is="item.type === 1 ? RectangleGroupIcon : CubeIcon"
                    class="w-5 h-5"
                    :class="item.type === 1 ? 'text-primary-600' : 'text-gray-500'"
                  />
                </div>
              </div>
              
              <!-- Item Info -->
              <label :for="`item-${item.id}`" class="flex-1 cursor-pointer">
                <div class="text-sm font-medium text-gray-900">
                  {{ item.sku }}
                  <span v-if="item.price" class="ml-2 text-primary-600 font-semibold">
                    ${{ item.price.toFixed(2) }}
                  </span>
                </div>
                <div class="text-xs text-gray-500">
                  {{ item.type === 1 ? 'Bundle' : 'Product Variant' }}
                  <span v-if="item.productName || item.bundleName" class="ml-2">
                    • {{ item.productName || item.bundleName }}
                  </span>
                  <span v-if="item.available !== undefined" class="ml-2">
                    • {{ item.available }} available
                  </span>
                </div>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex items-center justify-between">
        <div class="text-sm text-gray-500">
          {{ selectedItems.size }} item{{ selectedItems.size !== 1 ? 's' : '' }} selected
        </div>
        <div class="flex space-x-3">
          <button
            type="button"
            @click="$emit('close')"
            class="btn btn-secondary"
          >
            Cancel
          </button>
          <button
            type="button"
            @click="addSelectedItems"
            :disabled="selectedItems.size === 0"
            class="btn btn-primary"
          >
            Add {{ selectedItems.size }} Item{{ selectedItems.size !== 1 ? 's' : '' }}
          </button>
        </div>
      </div>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useQuery } from '@tanstack/vue-query'
import {
  MagnifyingGlassIcon,
  CubeIcon,
  RectangleGroupIcon
} from '@heroicons/vue/24/outline'
import Modal from '@/components/Modal.vue'
import { inventoryApi, productApi, bundleApi } from '@/services/api'
import type { SellableItem } from '@/types/api'

interface Emits {
  (e: 'close'): void
  (e: 'select', items: any[]): void
}

const emit = defineEmits<Emits>()

const searchQuery = ref('')
const selectedItems = ref<Set<string>>(new Set())

// Fetch products and their variants
const { data: productsData, isLoading: isLoadingProducts } = useQuery({
  queryKey: ['products-for-bundle'],
  queryFn: () => productApi.getProducts({ pageSize: 1000 })
})

// Fetch bundles
const { data: bundlesData, isLoading: isLoadingBundles } = useQuery({
  queryKey: ['bundles-for-bundle'],
  queryFn: () => bundleApi.getBundles({ pageSize: 1000 })
})

// Fetch inventory data for availability info
const { data: inventoryData } = useQuery({
  queryKey: ['inventory-for-bundle'],
  queryFn: () => inventoryApi.getInventoryRecords({ pageSize: 1000 }),
  retry: false // Don't retry if inventory API fails
})

const isLoading = computed(() => isLoadingProducts.value || isLoadingBundles.value)

// Convert products and bundles to sellable items
const allSellableItems = computed(() => {
  const items: any[] = []
  const addedSkus = new Set<string>()
  
  // Add product variants
  if (productsData.value?.data) {
    productsData.value.data.forEach(product => {
      if (product.variants && product.variants.length > 0) {
        product.variants.forEach(variant => {
          if (variant.sku && !addedSkus.has(variant.sku)) {
            // Find inventory info for this variant
            const inventoryRecord = inventoryData.value?.data?.find(
              record => record.sku === variant.sku
            )
            
            items.push({
              id: variant.id,
              sku: variant.sku,
              type: 0, // Variant type
              available: inventoryRecord?.available || 0,
              onHand: inventoryRecord?.onHand || 0,
              reserved: inventoryRecord?.reserved || 0,
              productName: product.name,
              price: variant.price
            })
            addedSkus.add(variant.sku)
          }
        })
      }
    })
  }
  
  // Add bundles
  if (bundlesData.value?.data) {
    bundlesData.value.data.forEach(bundle => {
      if (bundle.sku && !addedSkus.has(bundle.sku)) {
        // Find inventory info for this bundle
        const inventoryRecord = inventoryData.value?.data?.find(
          record => record.sku === bundle.sku
        )
        
        items.push({
          id: bundle.id,
          sku: bundle.sku,
          type: 1, // Bundle type
          available: inventoryRecord?.available || 0,
          onHand: inventoryRecord?.onHand || 0,
          reserved: inventoryRecord?.reserved || 0,
          bundleName: bundle.name,
          price: bundle.price
        })
        addedSkus.add(bundle.sku)
      }
    })
  }
  
  // Add remaining inventory items that don't match products/bundles
  if (inventoryData.value?.data) {
    inventoryData.value.data.forEach(record => {
      if (record.sku && !addedSkus.has(record.sku)) {
        items.push({
          id: record.sellableItemId,
          sku: record.sku,
          type: 0, // Default to variant type for unknown items
          available: record.available,
          onHand: record.onHand,
          reserved: record.reserved,
          productName: 'Unknown Item',
          price: 0
        })
        addedSkus.add(record.sku)
      }
    }
  )}
  
  return items
})

// Filtered items
const filteredItems = computed(() => {
  let items = allSellableItems.value

  // Filter by search only
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    items = items.filter(item =>
      item.sku.toLowerCase().includes(query) ||
      (item.productName && item.productName.toLowerCase().includes(query)) ||
      (item.bundleName && item.bundleName.toLowerCase().includes(query))
    )
  }

  return items
})

const toggleItem = (item: any) => {
  if (selectedItems.value.has(item.id)) {
    selectedItems.value.delete(item.id)
  } else {
    selectedItems.value.add(item.id)
  }
}

const addSelectedItems = () => {
  const itemsToAdd = allSellableItems.value.filter(item => 
    selectedItems.value.has(item.id)
  )
  emit('select', itemsToAdd)
}

const selectAll = () => {
  filteredItems.value.forEach(item => {
    selectedItems.value.add(item.id)
  })
}

const clearAll = () => {
  selectedItems.value.clear()
}
</script>