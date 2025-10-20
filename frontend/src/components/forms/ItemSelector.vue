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
          placeholder="Search products and variants..."
          class="input pl-10 w-full"
        />
      </div>

      <!-- Type Filter and Selection Controls -->
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-4">
          <label class="flex items-center">
            <input
              v-model="showVariants"
              type="checkbox"
              class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
            />
            <span class="ml-2 text-sm text-gray-700">Show Product Variants</span>
          </label>
          <label class="flex items-center">
            <input
              v-model="showBundles"
              type="checkbox"
              class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
            />
            <span class="ml-2 text-sm text-gray-700">Show Bundles</span>
          </label>
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
                </div>
                <div class="text-xs text-gray-500">
                  {{ item.type === 1 ? 'Bundle' : 'Product Variant' }}
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
import { inventoryApi } from '@/services/api'
import type { SellableItem } from '@/types/api'

interface Emits {
  (e: 'close'): void
  (e: 'select', items: any[]): void
}

const emit = defineEmits<Emits>()

const searchQuery = ref('')
const showVariants = ref(true)
const showBundles = ref(true)
const selectedItems = ref<Set<string>>(new Set())

// Fetch sellable items from inventory (has sellable items data)
const { data: inventoryData, isLoading } = useQuery({
  queryKey: ['inventory-for-bundle'],
  queryFn: () => inventoryApi.getInventoryRecords({ pageSize: 1000 })
})

// Convert inventory records to sellable items
const allSellableItems = computed(() => {
  const items: any[] = []
  
  // Add sellable items from inventory
  if (inventoryData.value?.data) {
    inventoryData.value.data.forEach(record => {
      if (record.sellableItem) {
        items.push({
          id: record.sellableItem.id,
          sku: record.sellableItem.sku,
          type: record.sellableItem.type,
          available: record.available,
          onHand: record.onHand,
          reserved: record.reserved
        })
      }
    })
  }
  
  return items
})

// Filtered items
const filteredItems = computed(() => {
  let items = allSellableItems.value

  // Filter by type
  items = items.filter(item => {
    if (item.type === 0 && !showVariants.value) return false // Variant
    if (item.type === 1 && !showBundles.value) return false // Bundle
    return true
  })

  // Filter by search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    items = items.filter(item =>
      item.sku.toLowerCase().includes(query)
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