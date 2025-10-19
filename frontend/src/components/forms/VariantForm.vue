<template>
  <Modal
    :model-value="true"
    :title="isEditing ? 'Edit Variant' : 'Create Variant'"
    size="lg"
    @update:model-value="(value) => !value && $emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Left Column -->
        <div class="space-y-4">
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
              placeholder="PRODUCT-VARIANT-001"
              @blur="validateSingleField('sku')"
            />
            <p v-if="formData.sku.error && formData.sku.touched" class="mt-1 text-sm text-red-600">
              {{ formData.sku.error }}
            </p>
          </div>

          <!-- Price -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Price *
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
              <option :value="0">Active</option>
              <option :value="1">Inactive</option>
            </select>
            <p v-if="formData.status.error && formData.status.touched" class="mt-1 text-sm text-red-600">
              {{ formData.status.error }}
            </p>
          </div>
        </div>

        <!-- Right Column - Option Values -->
        <div class="space-y-4">
          <div>
            <div class="flex items-center justify-between mb-3">
              <label class="block text-sm font-medium text-gray-700">
                Option Values
              </label>
              <button
                type="button"
                @click="addOptionValue"
                class="btn btn-secondary btn-sm"
              >
                <PlusIcon class="w-4 h-4 mr-2" />
                Add Option
              </button>
            </div>

            <!-- Option Values List -->
            <div class="space-y-3 max-h-48 overflow-y-auto">
              <div
                v-for="(option, index) in optionValues"
                :key="index"
                class="grid grid-cols-3 gap-2 p-3 border border-gray-200 rounded-lg"
              >
                <input
                  v-model="option.optionName"
                  type="text"
                  placeholder="Option name"
                  class="input text-sm"
                />
                <input
                  v-model="option.value"
                  type="text"
                  placeholder="Value"
                  class="input text-sm"
                />
                <button
                  type="button"
                  @click="removeOptionValue(index)"
                  class="btn btn-secondary btn-sm text-red-600 hover:text-red-800"
                >
                  <XMarkIcon class="w-4 h-4" />
                </button>
              </div>

              <div v-if="!optionValues.length" class="text-center py-4 text-gray-500 text-sm">
                No options added yet
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Attributes -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Additional Attributes
        </label>
        <div class="space-y-3">
          <div
            v-for="(attr, index) in attributes"
            :key="index"
            class="flex items-center space-x-3"
          >
            <input
              v-model="attr.key"
              type="text"
              placeholder="Attribute name"
              class="input flex-1"
            />
            <input
              v-model="attr.value"
              type="text"
              placeholder="Attribute value"
              class="input flex-1"
            />
            <button
              type="button"
              @click="removeAttribute(index)"
              class="text-red-600 hover:text-red-800"
            >
              <XMarkIcon class="w-5 h-5" />
            </button>
          </div>
          <button
            type="button"
            @click="addAttribute"
            class="btn btn-secondary btn-sm"
          >
            <PlusIcon class="w-4 h-4 mr-2" />
            Add Attribute
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
        :disabled="loading || !isFormValid"
      >
        <div v-if="loading" class="flex items-center">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
          {{ isEditing ? 'Updating...' : 'Creating...' }}
        </div>
        <span v-else>
          {{ isEditing ? 'Update Variant' : 'Create Variant' }}
        </span>
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useMutation } from '@tanstack/vue-query'
import { XMarkIcon, PlusIcon } from '@heroicons/vue/24/outline'
import Modal from '@/components/Modal.vue'
import { useFormValidation } from '@/composables/useFormValidation'
import { productApi } from '@/services/api'
import type { ProductMaster, ProductVariant, VariantOptionValue } from '@/types/api'
import type { ValidationRule } from '@/types/forms'

interface Props {
  product: ProductMaster
  variant?: ProductVariant | null
}

interface Emits {
  (e: 'close'): void
  (e: 'saved', variant: ProductVariant): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const loading = ref(false)
const isEditing = computed(() => !!props.variant)

// Form initial data
const initialData = {
  sku: props.variant?.sku || '',
  price: props.variant?.price || 0,
  status: props.variant?.status ?? 0
}

// Validation rules
const validationRules: Record<string, ValidationRule> = {
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
  status: { required: true }
}

const {
  formData,
  validateForm,
  validateSingleField,
  isFormValid,
  formValues
} = useFormValidation(initialData, validationRules)

// Option values management
const optionValues = ref<VariantOptionValue[]>([])

const addOptionValue = () => {
  optionValues.value.push({
    optionName: '',
    optionSlug: '',
    value: ''
  })
}

const removeOptionValue = (index: number) => {
  optionValues.value.splice(index, 1)
}

// Attributes management
const attributes = ref<Array<{ key: string; value: string }>>([])

const addAttribute = () => {
  attributes.value.push({ key: '', value: '' })
}

const removeAttribute = (index: number) => {
  attributes.value.splice(index, 1)
}

// Convert attributes array to object
const getAttributesObject = () => {
  const attrs: Record<string, any> = {}
  attributes.value.forEach(attr => {
    if (attr.key && attr.value) {
      attrs[attr.key] = attr.value
    }
  })
  return attrs
}

// Auto-generate slug for option values
const updateOptionSlugs = () => {
  optionValues.value.forEach(option => {
    if (option.optionName && !option.optionSlug) {
      option.optionSlug = option.optionName
        .toLowerCase()
        .replace(/[^a-z0-9\s-]/g, '')
        .replace(/\s+/g, '-')
        .replace(/-+/g, '-')
        .trim()
    }
  })
}

// Create/Update mutations
const createVariantMutation = useMutation({
  mutationFn: (data: any) => productApi.createVariant(props.product.id, data),
  onSuccess: (variant) => {
    emit('saved', variant)
  },
  onError: (error) => {
    console.error('Create variant error:', error)
  }
})

const updateVariantMutation = useMutation({
  mutationFn: (data: any) => productApi.updateVariant(props.product.id, props.variant!.id, data),
  onSuccess: (variant) => {
    emit('saved', variant)
  },
  onError: (error) => {
    console.error('Update variant error:', error)
  }
})

const handleSubmit = async () => {
  if (!validateForm()) return

  loading.value = true
  updateOptionSlugs()

  try {
    const requestData = {
      productMasterId: props.product.id,
      ...formValues.value,
      optionValues: optionValues.value.filter(ov => ov.optionName && ov.value),
      attributes: getAttributesObject()
    }

    if (isEditing.value) {
      await updateVariantMutation.mutateAsync(requestData)
    } else {
      await createVariantMutation.mutateAsync(requestData)
    }
  } finally {
    loading.value = false
  }
}

// Initialize data from existing variant
onMounted(() => {
  if (props.variant) {
    optionValues.value = props.variant.optionValues?.map(ov => ({ ...ov })) || []
    
    if (props.variant.attributes) {
      attributes.value = Object.entries(props.variant.attributes).map(([key, value]) => ({
        key,
        value: String(value)
      }))
    }
  }
})
</script>