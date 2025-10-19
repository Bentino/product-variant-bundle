<template>
  <Teleport to="body">
    <div
      v-if="show"
      class="fixed inset-0 z-50 overflow-y-auto"
      @keydown.esc="onClose"
    >
      <!-- Backdrop -->
      <div
        class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
        @click="onClose"
      ></div>
      
      <!-- Modal -->
      <div class="flex min-h-full items-center justify-center p-4">
        <div
          class="relative bg-white rounded-lg shadow-xl max-w-lg w-full mx-auto transform transition-all"
          :class="sizeClasses"
          @click.stop
        >
          <!-- Header -->
          <div class="flex items-center justify-between p-6 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">
              {{ title }}
            </h3>
            <button
              @click="onClose"
              class="text-gray-400 hover:text-gray-600 transition-colors"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <!-- Body -->
          <div class="p-6">
            <slot />
          </div>
          
          <!-- Footer -->
          <div v-if="$slots.footer" class="flex items-center justify-end space-x-3 p-6 border-t border-gray-200">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

interface Props {
  modelValue: boolean
  title: string
  size?: 'sm' | 'md' | 'lg' | 'xl'
  closable?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  closable: true
})

const emit = defineEmits<Emits>()

const show = computed(() => props.modelValue)

const sizeClasses = computed(() => {
  const sizes = {
    sm: 'max-w-md',
    md: 'max-w-lg',
    lg: 'max-w-2xl',
    xl: 'max-w-4xl'
  }
  return sizes[props.size]
})

const onClose = () => {
  if (props.closable) {
    emit('update:modelValue', false)
  }
}

// Prevent body scroll when modal is open
onMounted(() => {
  if (show.value) {
    document.body.style.overflow = 'hidden'
  }
})

onUnmounted(() => {
  document.body.style.overflow = ''
})
</script>