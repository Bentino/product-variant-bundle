<template>
  <Modal
    :model-value="true"
    :title="isEditing ? 'Edit Variant' : 'Create Variant'"
    size="xl"
    @update:model-value="(value) => !value && $emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Option Values Section -->
      <div>
        <div class="flex items-center justify-between mb-3">
          <label class="block text-sm font-medium text-gray-700">
            Option Values *
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
        
        <!-- No Options Available -->
        <div v-if="availableOptions.length === 0" class="mb-4 p-4 bg-blue-50 rounded-lg border border-blue-200">
          <p class="text-sm font-medium text-blue-800 mb-2">💡 Getting started</p>
          <p class="text-xs text-blue-700">
            Add option types (like Size, Color) using the dropdown below to create variants.
          </p>
        </div>

        <!-- Instructions -->
        <div v-else class="mb-4 p-4 bg-green-50 rounded-lg border border-green-200">
          <p class="text-sm font-medium text-green-800 mb-2">📋 How to create a variant:</p>
          <div class="space-y-1 text-sm text-green-700">
            <div>1. Select an <strong>Option Type</strong> from the dropdown (e.g., Size, Color)</div>
            <div>2. Choose or enter a <strong>Value</strong> for that option (e.g., Large, Red)</div>
            <div>3. Add more options if needed for this specific variant</div>
          </div>
          <div class="mt-3 p-2 bg-blue-50 rounded border border-blue-200">
            <p class="text-xs text-blue-700">
              <strong>Available Options:</strong> {{ availableOptions.map(opt => opt.name).join(', ') }}
            </p>
          </div>
        </div>

        <!-- Option Values List -->
        <div class="space-y-3 max-h-64 overflow-y-auto">
          <div
            v-for="(option, index) in optionValues"
            :key="index"
            class="p-3 border border-gray-200 rounded-lg"
            :class="{ 'border-red-300 bg-red-50': optionValuesError && (!option.optionName || !option.value) }"
          >
            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-xs font-medium text-gray-600 mb-1">
                  Option Type
                </label>
                <select
                  v-model="option.variantOptionId"
                  class="input text-sm"
                  :class="{ 'border-red-500': optionValuesError && !option.variantOptionId }"
                  @change="onOptionTypeChange(option, index)"
                  required
                >
                  <option value="">Select option type...</option>
                  <option 
                    v-for="availableOption in availableOptions" 
                    :key="availableOption.id"
                    :value="availableOption.id"
                    :disabled="optionValues.some((opt, idx) => idx !== index && opt.variantOptionId === availableOption.id)"
                  >
                    {{ availableOption.name }}
                  </option>
                  <option value="__ADD_NEW__" class="font-medium text-primary-600">
                    + Add New Option Type
                  </option>
                </select>
                <p v-if="isDuplicateOptionType(option.variantOptionId, index)" class="text-xs text-yellow-600 mt-1">
                  ⚠️ This option type already exists
                </p>
              </div>
              <div class="flex items-end gap-2">
                <div class="flex-1">
                  <label class="block text-xs font-medium text-gray-600 mb-1">
                    Value
                  </label>
                  <input
                    v-model="option.value"
                    type="text"
                    placeholder="Large, Red, Cotton..."
                    class="input text-sm"
                    :class="{ 'border-red-500': optionValuesError && !option.value }"
                    @input="onOptionChange"
                    required
                  />
                  <div v-if="option.variantOptionId && getUsedValuesForOption(option.variantOptionId).length > 0" class="mt-1">
                    <p class="text-xs text-red-600">
                      ⚠️ Already used: {{ getUsedValuesForOption(option.variantOptionId).join(', ') }}
                    </p>
                  </div>
                </div>
                <button
                  type="button"
                  @click="removeOptionValue(index)"
                  class="btn btn-secondary btn-sm text-red-600 hover:text-red-800 mb-0"
                  title="Remove option"
                >
                  <XMarkIcon class="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>

          <div v-if="!optionValues.length" class="text-center py-8 text-gray-500">
            <CubeIcon class="w-12 h-12 mx-auto mb-3 text-gray-300" />
            <p class="text-sm">No options added yet</p>
            <p class="text-xs">Click "Add Option" to start</p>
          </div>
        </div>
      </div>

      <!-- Product Details Section -->
      <div class="space-y-6">
        <!-- SKU -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <label class="block text-sm font-medium text-gray-700">
              SKU *
            </label>
            <button
              type="button"
              @click="generateSKU"
              class="text-sm text-primary-600 hover:text-primary-800 font-medium"
            >
              Auto Generate
            </button>
          </div>
          <input
            v-model="formData.sku.value"
            type="text"
            class="input"
            :class="{ 'border-red-500': formData.sku.error && formData.sku.touched }"
            :placeholder="isEditing ? 'Enter SKU manually' : 'Will auto-generate based on product + options'"
            @blur="validateSingleField('sku')"
          />
          <p v-if="formData.sku.error && formData.sku.touched" class="mt-1 text-sm text-red-600">
            {{ formData.sku.error }}
          </p>
          <p class="mt-1 text-xs text-gray-500">
            {{ isEditing 
              ? 'Click "Auto Generate" to create new SKU based on product and options' 
              : 'SKU will auto-generate when you add options, or click "Auto Generate"' 
            }}
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
        :disabled="!canSubmit"
      >
        <div v-if="loading" class="flex items-center">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
          {{ isEditing ? 'Updating...' : 'Creating...' }}
        </div>
        <span v-else>
          {{ isEditing ? 'Update Variant' : 'Create Variant' }}
        </span>
      </button>
      
      <!-- Debug info (development only) -->
      <div v-if="isDev" class="text-xs text-gray-500 mt-2 space-y-1">
        <div>Debug: Loading={{ loading }}, FormValid={{ isFormValid }}, CanSubmit={{ canSubmit }}</div>
        <div>Product ID: {{ product.id }}</div>
        <div>Product Name: {{ product.name }}</div>
        <div>Available Options: {{ availableOptions.length }} items</div>
        <div v-if="availableOptions.length > 0">
          Options: {{ availableOptions.map(opt => opt.name).join(', ') }}
        </div>
      </div>
    </template>
  </Modal>

  <!-- Create Option Modal -->
  <Modal
    v-model="showCreateOptionModal"
    title="Create Option Type"
    size="md"
  >
    <div class="space-y-4">
      <p class="text-sm text-gray-600">
        Create a new option type (like Size, Color, Material) for this product.
      </p>
      
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Option Name *
        </label>
        <input
          v-model="newOptionName"
          type="text"
          placeholder="e.g., Size, Color, Material"
          class="input w-full"
          @keyup.enter="createVariantOption"
        />
        <p class="text-xs text-gray-500 mt-1">
          This will be used to categorize variant values (e.g., Size: Small/Medium/Large)
        </p>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        @click="showCreateOptionModal = false"
        class="btn btn-secondary"
      >
        Cancel
      </button>
      <button
        @click="createVariantOption"
        class="btn btn-primary"
        :disabled="!newOptionName.trim()"
      >
        Create Option
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useMutation } from '@tanstack/vue-query'
import { XMarkIcon, PlusIcon, CubeIcon } from '@heroicons/vue/24/outline'
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
const availableOptions = ref<any[]>([])
const showCreateOptionModal = ref(false)
const newOptionName = ref('')

// Development mode check
const isDev = import.meta.env.DEV

// Check if form can be submitted
const canSubmit = computed(() => {
  return !loading.value && isFormValid.value
})

// Fetch available variant options for this product
const fetchAvailableOptions = async () => {
  try {
    console.log('Fetching options for product:', props.product.id)
    console.log('Full product object:', props.product)
    const response = await fetch(`/api/products/${props.product.id}/options`)
    console.log('Response status:', response.status)
    if (response.ok) {
      const data = await response.json()
      console.log('Available options response:', data)
      availableOptions.value = data.data || []
    } else {
      const errorText = await response.text()
      console.error('Failed to fetch options, status:', response.status, 'Error:', errorText)
    }
  } catch (error) {
    console.error('Failed to fetch variant options:', error)
  }
}

// Get available values for a specific option (excluding already used ones)
const getAvailableValuesForOption = (optionId: string) => {
  const option = availableOptions.value.find(opt => opt.id === optionId)
  if (!option) return []
  
  // Get all used values for this option from existing variants
  const usedValues = option.values?.map((v: any) => v.value) || []
  
  // For now, return some common values excluding used ones
  const commonValues = {
    'c0c9ecc1-bea2-4564-9f5f-793d54296078': ['XS', 'Small', 'Medium', 'Large', 'XL', 'XXL'], // Size
    'ef75a211-dcdf-4d7d-9234-10d2cf8655cf': ['Red', 'Blue', 'Green', 'Black', 'White', 'Yellow', 'Purple', 'Orange'] // Color
  }
  
  const allValues = commonValues[optionId as keyof typeof commonValues] || []
  return allValues.filter(value => !usedValues.includes(value))
}

// Get used values for display
const getUsedValuesForOption = (optionId: string) => {
  const option = availableOptions.value.find(opt => opt.id === optionId)
  return option?.values?.map((v: any) => v.value) || []
}

// Create new variant option
const createVariantOption = async () => {
  if (!newOptionName.value.trim()) return
  
  try {
    const response = await fetch(`/api/products/${props.product.id}/options`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        name: newOptionName.value.trim(),
        slug: newOptionName.value.toLowerCase().replace(/[^a-z0-9]/g, '-').replace(/-+/g, '-')
      })
    })
    
    if (response.ok) {
      const newOption = await response.json()
      console.log('Created new option:', newOption)
      
      // Refresh available options
      await fetchAvailableOptions()
      
      // Close modal and reset
      showCreateOptionModal.value = false
      newOptionName.value = ''
      
      // Auto-select the newly created option in the first empty option slot
      const emptyOption = optionValues.value.find(opt => !opt.variantOptionId)
      if (emptyOption && newOption.data) {
        emptyOption.variantOptionId = newOption.data.id
        emptyOption.optionName = newOption.data.name
        emptyOption.optionSlug = newOption.data.slug
      }
    } else {
      const errorText = await response.text()
      console.error('Failed to create option:', response.status, errorText)
    }
  } catch (error) {
    console.error('Error creating option:', error)
  }
}



// Handle option type selection
const onOptionTypeChange = (option: any, index: number) => {
  // Check if user selected "Add New"
  if (option.variantOptionId === '__ADD_NEW__') {
    // Reset selection and show modal
    option.variantOptionId = ''
    showCreateOptionModal.value = true
    return
  }
  
  // Clear the value when option type changes
  option.value = ''
  
  // Update option name for display
  const selectedOption = availableOptions.value.find(opt => opt.id === option.variantOptionId)
  if (selectedOption) {
    option.optionName = selectedOption.name
    option.optionSlug = selectedOption.slug
  }
  
  onOptionChange()
}

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
const optionValuesError = ref<string>('')

const addOptionValue = () => {
  optionValues.value.push({
    optionName: '',
    optionSlug: '',
    value: ''
  })
  // Clear error when adding new option
  optionValuesError.value = ''
}

const removeOptionValue = (index: number) => {
  optionValues.value.splice(index, 1)
  // Auto-update SKU when options change
  if (formData.sku.value && !isEditing.value) {
    generateSKU()
  }
}

// Validate option values
const validateOptionValues = () => {
  const validOptions = optionValues.value.filter(opt => opt.optionName && opt.value)
  
  if (validOptions.length === 0) {
    optionValuesError.value = 'At least one option is required'
    return false
  }
  
  // Check for incomplete options
  const incompleteOptions = optionValues.value.filter(opt => 
    (opt.optionName && !opt.value) || (!opt.optionName && opt.value)
  )
  
  if (incompleteOptions.length > 0) {
    optionValuesError.value = 'Please complete all option fields or remove incomplete ones'
    return false
  }
  
  optionValuesError.value = ''
  return true
}

// Check for duplicate option types
const isDuplicateOptionType = (optionName: string, currentIndex: number) => {
  if (!optionName) return false
  
  return optionValues.value.some((option, index) => 
    index !== currentIndex && 
    option.optionName && 
    option.optionName.toLowerCase().trim() === optionName.toLowerCase().trim()
  )
}

// Check and warn about duplicate options
const checkDuplicateOptions = () => {
  // This function can be used to show warnings or prevent form submission
  const duplicates = optionValues.value.filter((option, index) => 
    isDuplicateOptionType(option.optionName, index)
  )
  
  if (duplicates.length > 0) {
    console.warn('Duplicate option types detected:', duplicates)
  }
}

// Handle option changes for auto SKU generation
const onOptionChange = () => {
  // Clear error when user starts typing
  optionValuesError.value = ''
  
  // Auto-generate SKU when options change (only for new variants)
  if (!isEditing.value && optionValues.value.some(opt => opt.optionName && opt.value)) {
    setTimeout(() => {
      generateSKU()
    }, 300) // Debounce to avoid too frequent updates
  }
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

// Auto-generate SKU based on product slug and option values
const generateSKU = () => {
  console.log('Generating SKU...')
  console.log('Product slug:', props.product.slug)
  console.log('Option values:', optionValues.value)
  
  const productSlug = props.product.slug.toUpperCase().replace(/-/g, '_')
  
  // Get option values for SKU generation
  const optionParts = optionValues.value
    .filter(option => option.variantOptionId && option.value)
    .map(option => {
      // Use optionName if available, otherwise use a generic key
      const optionKey = (option.optionName || 'OPT').toUpperCase().replace(/[^A-Z0-9]/g, '').substring(0, 3)
      const optionValue = option.value.toUpperCase().replace(/[^A-Z0-9]/g, '').substring(0, 3)
      return `${optionKey}_${optionValue}`
    })
    .join('_')
  
  // Generate timestamp suffix for uniqueness
  const timestamp = Date.now().toString().slice(-4)
  
  let generatedSKU = productSlug
  
  if (optionParts) {
    generatedSKU += `_${optionParts}`
  }
  
  generatedSKU += `_${timestamp}`
  
  // Ensure SKU doesn't exceed reasonable length
  if (generatedSKU.length > 50) {
    generatedSKU = generatedSKU.substring(0, 50)
  }
  
  console.log('Generated SKU:', generatedSKU)
  formData.sku.value = generatedSKU
  validateSingleField('sku')
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
  // Validate form and option values
  const isFormValidResult = validateForm()
  const isOptionsValidResult = validateOptionValues()
  
  if (!isFormValidResult || !isOptionsValidResult) {
    return
  }

  loading.value = true
  updateOptionSlugs()

  try {
    // Convert optionValues to the format expected by API
    const apiOptionValues = optionValues.value
      .filter(ov => ov.variantOptionId && ov.value)
      .map(ov => ({
        variantOptionId: ov.variantOptionId,
        value: ov.value
      }))

    const requestData = {
      productMasterId: props.product.id,
      sku: formValues.value.sku,
      price: formValues.value.price,
      status: formValues.value.status,
      optionValues: apiOptionValues,
      attributes: getAttributesObject()
    }
    
    console.log('Submitting variant data:', requestData)
    console.log('SKU value:', formValues.value.sku)

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
onMounted(async () => {
  // Fetch available options first
  await fetchAvailableOptions()
  
  if (props.variant) {
    optionValues.value = props.variant.optionValues?.map(ov => ({ ...ov })) || []
    
    if (props.variant.attributes) {
      attributes.value = Object.entries(props.variant.attributes).map(([key, value]) => ({
        key,
        value: String(value)
      }))
    }
  } else {
    // Add initial option for new variants
    addOptionValue()
    // Auto-generate initial SKU for new variants
    generateSKU()
  }
})
</script>