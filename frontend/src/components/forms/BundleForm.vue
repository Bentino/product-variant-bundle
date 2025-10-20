<template>
  <Modal
    :model-value="true"
    :title="isEditing ? 'Edit Bundle' : 'Create Bundle'"
    size="xl"
    @update:model-value="(value) => !value && $emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Left Column -->
        <div class="space-y-6">
          <!-- Name -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Bundle Name *
            </label>
            <input
              v-model="formData.name.value"
              type="text"
              class="input"
              :class="{ 'border-red-500': formData.name.error && formData.name.touched }"
              placeholder="Enter bundle name"
              @blur="validateSingleField('name')"
            />
            <p v-if="formData.name.error && formData.name.touched" class="mt-1 text-sm text-red-600">
              {{ formData.name.error }}
            </p>
          </div>

          <!-- SKU -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              SKU *
            </label>
            <input
              v-model="formData.sku.value"
              type="text"
              class="input"
              :class="{ 'border-red-500': formData.sku.error && formData.sku.touched }"
              placeholder="BUNDLE-SKU-001"
              @blur="validateSingleField('sku')"
            />
            <p v-if="formData.sku.error && formData.sku.touched" class="mt-1 text-sm text-red-600">
              {{ formData.sku.error }}
            </p>
          </div>

          <!-- Price -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Bundle Price *
            </label>
            <div class="relative">
              <span class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">$</span>
              <input
                v-model.number="formData.price.value"
                type="number"
                step="0.01"
                min="0"
                class="input pl-8"
                :class="{ 'border-red-500': formData.price.error && formData.price.touched }"
                placeholder="0.00"
                @blur="validateSingleField('price')"
              />
            </div>
            <p v-if="formData.price.error && formData.price.touched" class="mt-1 text-sm text-red-600">
              {{ formData.price.error }}
            </p>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Description
            </label>
            <textarea
              v-model="formData.description.value"
              rows="4"
              class="input"
              :class="{ 'border-red-500': formData.description.error && formData.description.touched }"
              placeholder="Enter bundle description"
              @blur="validateSingleField('description')"
            ></textarea>
            <p v-if="formData.description.error && formData.description.touched" class="mt-1 text-sm text-red-600">
              {{ formData.description.error }}
            </p>
          </div>

          <!-- Status -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Status *
            </label>
            <select
              v-model.number="formData.status.value"
              class="input"
              :class="{ 'border-red-500': formData.status.error && formData.status.touched }"
              @blur="validateSingleField('status')"
            >
              <option :value="0">Inactive</option>
              <option :value="1">Active</option>
              <option :value="2">Archived</option>
            </select>
            <p v-if="formData.status.error && formData.status.touched" class="mt-1 text-sm text-red-600">
              {{ formData.status.error }}
            </p>
          </div>
        </div>

        <!-- Right Column - Bundle Items -->
        <div class="space-y-6">
          <div>
            <div class="flex items-center justify-between mb-4">
              <label class="block text-sm font-medium text-gray-700">
                Bundle Items *
              </label>
              <button
                type="button"
                @click="showItemSelector = true"
                class="btn btn-secondary btn-sm"
              >
                <PlusIcon class="w-4 h-4 mr-2" />
                Add Item
              </button>
            </div>

            <!-- Items List -->
            <div class="space-y-3 max-h-80 overflow-y-auto">
              <div
                v-for="(item, index) in bundleItems"
                :key="index"
                class="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg"
              >
                <div class="flex-1">
                  <div class="text-sm font-medium text-gray-900">
                    {{ item.sku || item.sellableItem?.sku || 'Unknown Item' }}
                  </div>
                  <div class="text-xs text-gray-500">
                    {{ item.sellableItem?.type === 0 ? 'Variant' : 'Bundle' }}
                    <span v-if="item.itemName" class="ml-2">• {{ item.itemName }}</span>
                  </div>
                </div>
                <div class="flex items-center space-x-2">
                  <label class="text-xs text-gray-500">Qty:</label>
                  <input
                    v-model.number="item.quantity"
                    type="number"
                    min="1"
                    class="w-16 px-2 py-1 text-sm border border-gray-300 rounded"
                  />
                </div>
                <button
                  type="button"
                  @click="removeItem(index)"
                  class="text-red-600 hover:text-red-800"
                >
                  <XMarkIcon class="w-4 h-4" />
                </button>
              </div>

              <div v-if="!bundleItems.length" class="text-center py-8 text-gray-500">
                <RectangleGroupIcon class="mx-auto h-12 w-12 text-gray-300 mb-2" />
                <p>No items added yet</p>
                <p class="text-sm">Click "Add Item" to start building your bundle</p>
              </div>
            </div>

            <p v-if="formData.items.error && formData.items.touched" class="mt-2 text-sm text-red-600">
              {{ formData.items.error }}
            </p>
          </div>
        </div>
      </div>

      <!-- Metadata -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Additional Metadata
        </label>
        <div class="space-y-3">
          <div
            v-for="(meta, index) in metadata"
            :key="index"
            class="flex items-center space-x-3"
          >
            <input
              v-model="meta.key"
              type="text"
              placeholder="Metadata key"
              class="input flex-1"
            />
            <input
              v-model="meta.value"
              type="text"
              placeholder="Metadata value"
              class="input flex-1"
            />
            <button
              type="button"
              @click="removeMetadata(index)"
              class="text-red-600 hover:text-red-800"
            >
              <XMarkIcon class="w-5 h-5" />
            </button>
          </div>
          <button
            type="button"
            @click="addMetadata"
            class="btn btn-secondary btn-sm"
          >
            <PlusIcon class="w-4 h-4 mr-2" />
            Add Metadata
          </button>
        </div>
      </div>
    </form>

    <template #footer>
      <button
        type="button"
        @click="$emit('close')"
        class="btn btn-secondary"
        :disabled="loading"
      >
        Cancel
      </button>
      <button
        @click="handleSubmit"
        class="btn btn-primary"
        :disabled="loading || !isFormValid || (!isEditing && !bundleItems.length)"
      >
        <div v-if="loading" class="flex items-center">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
          {{ isEditing ? 'Updating...' : 'Creating...' }}
        </div>
        <span v-else>
          {{ isEditing ? 'Update Bundle' : 'Create Bundle' }}
        </span>
      </button>
    </template>

    <!-- Item Selector Modal -->
    <ItemSelector
      v-if="showItemSelector"
      @close="showItemSelector = false"
      @select="addItem"
    />
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useMutation, useQueryClient } from '@tanstack/vue-query'
import { XMarkIcon, PlusIcon, RectangleGroupIcon } from '@heroicons/vue/24/outline'
import Modal from '@/components/Modal.vue'
import ItemSelector from './ItemSelector.vue'
import { useFormValidation } from '@/composables/useFormValidation'
import { bundleApi } from '@/services/api'
import type { ProductBundle, CreateBundleRequest, UpdateBundleRequest, BundleItem } from '@/types/api'
import type { ValidationRule } from '@/types/forms'

interface Props {
  bundle?: ProductBundle | null
}

interface Emits {
  (e: 'close'): void
  (e: 'saved', bundle: ProductBundle): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const queryClient = useQueryClient()
const loading = ref(false)
const showItemSelector = ref(false)

const isEditing = computed(() => !!props.bundle)

// Form initial data
const initialData = {
  name: '',
  description: '',
  sku: '',
  price: 0,
  status: 1,
  items: [],
  metadata: {}
}

// Validation rules
const validationRules: Record<string, ValidationRule> = {
  name: { required: true, minLength: 2, maxLength: 255 },
  sku: { 
    required: true, 
    minLength: 2, 
    maxLength: 100,
    pattern: /^[A-Z0-9-_]+$/,
    custom: (value: string) => {
      if (value && !/^[A-Z0-9-_]+$/.test(value)) {
        return 'SKU can only contain uppercase letters, numbers, hyphens, and underscores'
      }
      return null
    }
  },
  price: { 
    required: true,
    custom: (value: number) => {
      if (value < 0) return 'Price must be greater than or equal to 0'
      return null
    }
  },
  description: { maxLength: 2000 },
  status: { 
    required: true,
    custom: (value: number) => {
      if (value !== 0 && value !== 1 && value !== 2) return 'Status must be Inactive (0), Active (1), or Archived (2)'
      return null
    }
  },
  items: {
    required: false, // Allow empty items for editing existing bundles
    custom: (value: any[]) => {
      // Only require items for new bundles
      if (!isEditing.value && (!value || value.length === 0)) {
        return 'At least one item is required'
      }
      return null
    }
  }
}

const {
  formData,
  validateForm,
  validateSingleField,
  resetForm,
  isFormValid,
  formValues
} = useFormValidation(initialData, validationRules)

// Bundle items management
const bundleItems = ref<Array<BundleItem & { sellableItem?: any }>>([])

// Metadata management
const metadata = ref<Array<{ key: string; value: string }>>([])

const addItem = (selectedItems: any[]) => {
  // Handle both single item (legacy) and multiple items
  const itemsToAdd = Array.isArray(selectedItems) ? selectedItems : [selectedItems]
  
  itemsToAdd.forEach(selectedItem => {
    // selectedItem.id is now the sellableItemId from inventory
    const sellableItemId = selectedItem.id
    const existingIndex = bundleItems.value.findIndex(
      item => item.sellableItemId === sellableItemId
    )
    
    if (existingIndex >= 0) {
      bundleItems.value[existingIndex].quantity += 1
    } else {
      bundleItems.value.push({
        sellableItemId: sellableItemId,
        quantity: 1,
        sellableItem: {
          id: sellableItemId,
          sku: selectedItem.sku,
          type: selectedItem.type
        }
      })
    }
  })
  
  // Update form data
  formData.items.value = bundleItems.value.map(item => ({
    sellableItemId: item.sellableItemId,
    quantity: item.quantity
  }))
  
  showItemSelector.value = false
}

const removeItem = (index: number) => {
  bundleItems.value.splice(index, 1)
  formData.items.value = bundleItems.value.map(item => ({
    sellableItemId: item.sellableItemId,
    quantity: item.quantity
  }))
}

const addMetadata = () => {
  metadata.value.push({ key: '', value: '' })
}

const removeMetadata = (index: number) => {
  metadata.value.splice(index, 1)
}

// Convert metadata array to object
const getMetadataObject = () => {
  const meta: Record<string, any> = {}
  metadata.value.forEach(item => {
    if (item.key && item.value) {
      meta[item.key] = item.value
    }
  })
  return meta
}

// Auto-generate SKU from name
watch(() => formData.name.value, (newName) => {
  if (!isEditing.value && newName) {
    const timestamp = Date.now().toString().slice(-6) // Last 6 digits of timestamp
    const sku = newName
      .toUpperCase()
      .replace(/[^A-Z0-9\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim()
    formData.sku.value = `${sku}-BUNDLE-${timestamp}`
  }
})

// Mutations
const createMutation = useMutation({
  mutationFn: (data: CreateBundleRequest) => bundleApi.createBundle(data),
  onSuccess: (bundle) => {
    queryClient.invalidateQueries({ queryKey: ['bundles'] })
    emit('saved', bundle)
    emit('close')
  },
  onError: (error: any) => {
    console.error('Create bundle error:', error)
    if (error.response?.data?.errors) {
      Object.entries(error.response.data.errors).forEach(([field, messages]) => {
        if (formData[field as keyof typeof formData]) {
          formData[field as keyof typeof formData].error = (messages as string[])[0]
        }
      })
    }
  }
})

const updateMutation = useMutation({
  mutationFn: ({ id, data }: { id: string; data: UpdateBundleRequest }) => 
    bundleApi.updateBundle(id, data),
  onSuccess: (bundle) => {
    queryClient.invalidateQueries({ queryKey: ['bundles'] })
    emit('saved', bundle)
    emit('close')
  },
  onError: (error: any) => {
    console.error('Update bundle error:', error)
    if (error.response?.data?.errors) {
      Object.entries(error.response.data.errors).forEach(([field, messages]) => {
        if (formData[field as keyof typeof formData]) {
          formData[field as keyof typeof formData].error = (messages as string[])[0]
        }
      })
    }
  }
})

const handleSubmit = async () => {
  // Update items in form data
  formData.items.value = bundleItems.value.map(item => ({
    sellableItemId: item.sellableItemId,
    quantity: item.quantity
  }))

  if (!validateForm()) return

  loading.value = true

  try {
    const requestData = {
      ...formValues.value,
      items: bundleItems.value.map(item => ({
        sellableItemId: item.sellableItemId,
        quantity: item.quantity
      })),
      metadata: getMetadataObject()
    }

    if (isEditing.value && props.bundle) {
      await updateMutation.mutateAsync({
        id: props.bundle.id,
        data: requestData
      })
    } else {
      await createMutation.mutateAsync(requestData)
    }
  } finally {
    loading.value = false
  }
}

// Initialize data from existing bundle
onMounted(() => {
  if (props.bundle) {
    // Populate form data
    formData.name.value = props.bundle.name || ''
    formData.description.value = props.bundle.description || ''
    formData.sku.value = props.bundle.sku || ''
    formData.price.value = props.bundle.price || 0
    formData.status.value = props.bundle.status ?? 1
    
    // Populate bundle items
    bundleItems.value = props.bundle.items?.map(item => ({
      ...item,
      sellableItem: item.sellableItem
    })) || []
    
    // Update form items
    formData.items.value = bundleItems.value.map(item => ({
      sellableItemId: item.sellableItemId,
      quantity: item.quantity
    }))

    // Populate metadata
    if (props.bundle.metadata) {
      metadata.value = Object.entries(props.bundle.metadata).map(([key, value]) => ({
        key,
        value: String(value)
      }))
    }
  }
})
</script>