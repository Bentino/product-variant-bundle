<template>
  <Modal
    :model-value="true"
    :title="isEditing ? 'Edit Product' : 'Create Product'"
    size="lg"
    @update:model-value="(value) => !value && $emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Name -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Product Name *
        </label>
        <input
          v-model="formData.name.value"
          type="text"
          class="input"
          :class="{ 'border-red-500': formData.name.error && formData.name.touched }"
          placeholder="Enter product name"
          @blur="validateSingleField('name')"
        />
        <p v-if="formData.name.error && formData.name.touched" class="mt-1 text-sm text-red-600">
          {{ formData.name.error }}
        </p>
      </div>

      <!-- Slug -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          URL Slug *
        </label>
        <input
          v-model="formData.slug.value"
          type="text"
          class="input"
          :class="{ 'border-red-500': formData.slug.error && formData.slug.touched }"
          placeholder="product-url-slug"
          @blur="validateSingleField('slug')"
        />
        <p v-if="formData.slug.error && formData.slug.touched" class="mt-1 text-sm text-red-600">
          {{ formData.slug.error }}
        </p>
        <p class="mt-1 text-sm text-gray-500">
          URL-friendly version of the name (lowercase, hyphens only)
        </p>
      </div>

      <!-- Category -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Category *
        </label>
        <select
          v-model="formData.category.value"
          class="input"
          :class="{ 'border-red-500': formData.category.error && formData.category.touched }"
          @blur="validateSingleField('category')"
        >
          <option value="">Select a category</option>
          <option value="Electronics">Electronics</option>
          <option value="Clothing">Clothing</option>
          <option value="Books">Books</option>
          <option value="Home & Garden">Home & Garden</option>
          <option value="Sports">Sports</option>
        </select>
        <p v-if="formData.category.error && formData.category.touched" class="mt-1 text-sm text-red-600">
          {{ formData.category.error }}
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
          placeholder="Enter product description"
          @blur="validateSingleField('description')"
        ></textarea>
        <p v-if="formData.description.error && formData.description.touched" class="mt-1 text-sm text-red-600">
          {{ formData.description.error }}
        </p>
      </div>

      <!-- Variants Section -->
      <div v-if="isEditing" class="border-t border-gray-200 pt-6">
        <div class="flex items-center justify-between mb-4">
          <div>
            <label class="block text-sm font-medium text-gray-700">
              Product Variants
            </label>
            <div v-if="deletedVariantIds.length > 0" class="text-xs text-red-600 mt-1">
              {{ deletedVariantIds.length }} variant(s) marked for deletion
            </div>
          </div>
          <button
            type="button"
            @click="showVariantForm = true"
            class="btn btn-secondary btn-sm"
          >
            <PlusIcon class="w-4 h-4 mr-2" />
            Add Variant
          </button>
        </div>

        <!-- Variants List -->
        <div class="space-y-3 max-h-60 overflow-y-auto">
          <div
            v-for="(variant, index) in productVariants"
            :key="variant.id || index"
            class="flex items-center justify-between p-3 border rounded-lg"
            :class="isVariantMarkedForDeletion(variant) 
              ? 'border-red-300 bg-red-50 opacity-60' 
              : 'border-gray-200'"
          >
            <div class="flex-1">
              <div class="text-sm font-medium" 
                   :class="isVariantMarkedForDeletion(variant) ? 'text-red-700 line-through' : 'text-gray-900'">
                <span class="font-semibold">SKU:</span> {{ variant.sku || 'No SKU' }}
                <span v-if="isVariantMarkedForDeletion(variant)" class="ml-2 text-red-600 font-normal">
                  (Will be deleted)
                </span>
              </div>
              <div class="text-xs mt-1"
                   :class="isVariantMarkedForDeletion(variant) ? 'text-red-600' : 'text-gray-500'">
                <span class="font-medium">Price:</span> ${{ variant.price?.toFixed(2) || '0.00' }}
                <span v-if="variant.optionValues?.length" class="ml-2">
                  • {{ variant.optionValues.map(ov => `${ov.optionName || 'Unknown'}: ${ov.value}`).join(', ') }}
                </span>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <button
                v-if="!isVariantMarkedForDeletion(variant)"
                type="button"
                @click="editVariant(variant)"
                class="text-primary-600 hover:text-primary-800 text-sm"
              >
                Edit
              </button>
              <button
                v-if="!isVariantMarkedForDeletion(variant)"
                type="button"
                @click="deleteVariant(variant)"
                class="text-red-600 hover:text-red-800 text-sm"
              >
                Delete
              </button>
              <button
                v-if="isVariantMarkedForDeletion(variant)"
                type="button"
                @click="undoDeleteVariant(variant)"
                class="text-green-600 hover:text-green-800 text-sm font-medium"
              >
                Undo
              </button>
            </div>
          </div>

          <div v-if="!productVariants.length" class="text-center py-8 text-gray-500">
            <CubeIcon class="mx-auto h-12 w-12 text-gray-300 mb-2" />
            <p>No variants created yet</p>
            <p class="text-sm">Click "Add Variant" to create product variations</p>
          </div>
          
          <!-- Debug info (development only) -->
          <div v-if="isDev && productVariants.length > 0" class="mt-3 p-2 bg-gray-50 rounded text-xs text-gray-600">
            <div><strong>Debug - Variants Data:</strong></div>
            <div v-for="(variant, idx) in productVariants" :key="idx" class="mt-1">
              Variant {{ idx + 1 }}: SKU="{{ variant.sku }}", Price={{ variant.price }}, Options={{ variant.optionValues?.length || 0 }}
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
          {{ isEditing ? 'Update Product' : 'Create Product' }}
        </span>
      </button>
    </template>

    <!-- Variant Form Modal -->
    <VariantForm
      v-if="showVariantForm"
      :product="product"
      :variant="editingVariant"
      @close="onVariantFormClose"
      @saved="onVariantSaved"
    />
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useMutation, useQueryClient } from '@tanstack/vue-query'
import { XMarkIcon, PlusIcon, CubeIcon } from '@heroicons/vue/24/outline'
import Modal from '@/components/Modal.vue'
import VariantForm from './VariantForm.vue'
import { useFormValidation } from '@/composables/useFormValidation'
import { productApi } from '@/services/api'
import type { ProductMaster, ProductVariant, CreateProductRequest, UpdateProductRequest } from '@/types/api'
import type { ValidationRule } from '@/types/forms'

interface Props {
  product?: ProductMaster | null
}

interface Emits {
  (e: 'close'): void
  (e: 'saved', product: ProductMaster): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const queryClient = useQueryClient()
const loading = ref(false)
const showVariantForm = ref(false)
const isDev = import.meta.env.DEV
const editingVariant = ref<ProductVariant | null>(null)

const isEditing = computed(() => !!props.product)

// Product variants
const productVariants = ref<ProductVariant[]>([])
const deletedVariantIds = ref<string[]>([]) // Track variants marked for deletion

// Form initial data
const initialData = {
  name: props.product?.name || '',
  slug: props.product?.slug || '',
  description: props.product?.description || '',
  category: props.product?.category || '',
  status: props.product?.status ?? 1,
  attributes: props.product?.attributes || {}
}

// Validation rules
const validationRules: Record<string, ValidationRule> = {
  name: { required: true, minLength: 2, maxLength: 255 },
  slug: { 
    required: true, 
    minLength: 2, 
    maxLength: 255,
    pattern: /^[a-z0-9-]+$/,
    custom: (value: string) => {
      if (value && !/^[a-z0-9-]+$/.test(value)) {
        return 'Slug can only contain lowercase letters, numbers, and hyphens'
      }
      return null
    }
  },
  category: { required: true },
  status: { 
    required: true,
    custom: (value: number) => {
      if (value !== 0 && value !== 1 && value !== 2) return 'Status must be Inactive (0), Active (1), or Archived (2)'
      return null
    }
  },
  description: { maxLength: 2000 }
}

const {
  formData,
  validateForm,
  validateSingleField,
  resetForm,
  isFormValid,
  formValues
} = useFormValidation(initialData, validationRules)

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

// Auto-generate slug from name
watch(() => formData.name.value, (newName) => {
  if (!isEditing.value && newName) {
    const slug = newName
      .toLowerCase()
      .replace(/[^a-z0-9\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim()
    formData.slug.value = slug
  }
})

// Mutations
const createMutation = useMutation({
  mutationFn: (data: CreateProductRequest) => productApi.createProduct(data),
  onSuccess: (product) => {
    queryClient.invalidateQueries({ queryKey: ['products'] })
    emit('saved', product)
    emit('close')
  },
  onError: (error: any) => {
    console.error('Create product error:', error)
    // Handle API errors
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
  mutationFn: ({ id, data }: { id: string; data: UpdateProductRequest }) => 
    productApi.updateProduct(id, data),
  onSuccess: (product) => {
    queryClient.invalidateQueries({ queryKey: ['products'] })
    emit('saved', product)
    emit('close')
  },
  onError: (error: any) => {
    console.error('Update product error:', error)
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
  if (!validateForm()) return

  loading.value = true

  try {
    const requestData = {
      ...formValues.value,
      attributes: getAttributesObject()
    }

    console.log('Submitting product data:', requestData)

    if (isEditing.value && props.product) {
      // Update product master
      await updateMutation.mutateAsync({
        id: props.product.id,
        data: requestData
      })
      
      // Delete variants that were marked for deletion
      if (deletedVariantIds.value.length > 0) {
        console.log('Deleting marked variants:', deletedVariantIds.value)
        
        for (const variantId of deletedVariantIds.value) {
          try {
            await productApi.deleteVariant(props.product.id, variantId)
            console.log('Deleted variant:', variantId)
          } catch (error) {
            console.error('Failed to delete variant:', variantId, error)
          }
        }
        
        // Clear the deletion list
        deletedVariantIds.value = []
        
        // Refresh product data to sync with server
        queryClient.invalidateQueries({ queryKey: ['products'] })
      }
    } else {
      await createMutation.mutateAsync(requestData)
    }
  } finally {
    loading.value = false
  }
}

// Variant management
const editVariant = (variant: ProductVariant) => {
  editingVariant.value = variant
  showVariantForm.value = true
}

const deleteVariant = (variant: ProductVariant) => {
  if (!confirm('Mark this variant for deletion? It will be removed when you update the product.')) return
  
  // Mark for deletion (don't delete immediately)
  if (variant.id && !deletedVariantIds.value.includes(variant.id)) {
    deletedVariantIds.value.push(variant.id)
    console.log('Marked variant for deletion:', variant.id)
    console.log('Deleted variants:', deletedVariantIds.value)
  }
}

const undoDeleteVariant = (variant: ProductVariant) => {
  if (variant.id) {
    deletedVariantIds.value = deletedVariantIds.value.filter(id => id !== variant.id)
    console.log('Undid deletion for variant:', variant.id)
  }
}

// Check if variant is marked for deletion
const isVariantMarkedForDeletion = (variant: ProductVariant) => {
  return variant.id ? deletedVariantIds.value.includes(variant.id) : false
}

const onVariantSaved = (variant: ProductVariant) => {
  if (editingVariant.value) {
    // Update existing
    const index = productVariants.value.findIndex(v => v.id === variant.id)
    if (index >= 0) {
      productVariants.value[index] = variant
    }
  } else {
    // Add new
    productVariants.value.push(variant)
  }
  
  showVariantForm.value = false
  editingVariant.value = null
  
  // Refresh product data
  queryClient.invalidateQueries({ queryKey: ['products'] })
}

const onVariantFormClose = () => {
  showVariantForm.value = false
  editingVariant.value = null
}

// Initialize attributes and variants from existing product
onMounted(() => {
  if (props.product?.attributes) {
    attributes.value = Object.entries(props.product.attributes).map(([key, value]) => ({
      key,
      value: String(value)
    }))
  }
  
  if (props.product?.variants) {
    productVariants.value = [...props.product.variants]
    console.log('ProductForm - Loaded variants:', productVariants.value)
  }
})
</script>