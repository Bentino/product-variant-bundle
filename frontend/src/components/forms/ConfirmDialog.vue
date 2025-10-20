<template>
  <Modal
    :model-value="true"
    :title="title"
    size="sm"
    @update:model-value="(value) => !value && $emit('cancel')"
  >
    <div class="space-y-4">
      <!-- Icon -->
      <div class="flex items-center justify-center w-12 h-12 mx-auto bg-red-100 rounded-full">
        <ExclamationTriangleIcon class="w-6 h-6 text-red-600" />
      </div>
      
      <!-- Message -->
      <div class="text-center">
        <p class="text-sm text-gray-600">
          {{ message }}
        </p>
        <p v-if="itemName" class="mt-2 text-sm font-medium text-gray-900">
          "{{ itemName }}"
        </p>
      </div>
      
      <!-- Warning -->
      <div v-if="warning" class="p-3 bg-yellow-50 border border-yellow-200 rounded-md">
        <p class="text-sm text-yellow-800">
          {{ warning }}
        </p>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        @click="$emit('cancel')"
        class="btn btn-secondary"
        :disabled="loading"
      >
        Cancel
      </button>
      <button
        @click="handleConfirm"
        class="btn"
        :class="destructive ? 'bg-red-600 hover:bg-red-700 text-white' : 'btn-primary'"
        :disabled="loading"
      >
        <div v-if="loading" class="flex items-center">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
          {{ loadingText || 'Processing...' }}
        </div>
        <span v-else>
          {{ confirmText || 'Confirm' }}
        </span>
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import Modal from '@/components/Modal.vue'

interface Props {
  title: string
  message: string
  itemName?: string
  warning?: string
  confirmText?: string
  loadingText?: string
  destructive?: boolean
}

interface Emits {
  (e: 'confirm'): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  confirmText: 'Confirm',
  loadingText: 'Processing...',
  destructive: true
})

const emit = defineEmits<Emits>()

const loading = ref(false)

const handleConfirm = async () => {
  loading.value = true
  try {
    emit('confirm')
  } finally {
    loading.value = false
  }
}
</script>