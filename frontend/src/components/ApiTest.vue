<template>
  <div class="card p-6">
    <h3 class="text-lg font-medium text-gray-900 mb-4">API Connection Test</h3>
    
    <div class="space-y-4">
      <!-- Health Check -->
      <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
        <div class="flex items-center">
          <div 
            class="w-3 h-3 rounded-full mr-3"
            :class="healthStatus === 'success' ? 'bg-green-500' : 
                   healthStatus === 'error' ? 'bg-red-500' : 'bg-yellow-500'"
          ></div>
          <span class="text-sm font-medium">API Health Check</span>
        </div>
        <button 
          @click="testHealth"
          :disabled="loading"
          class="btn btn-sm btn-secondary"
        >
          {{ loading ? 'Testing...' : 'Test' }}
        </button>
      </div>

      <!-- Products API -->
      <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
        <div class="flex items-center">
          <div 
            class="w-3 h-3 rounded-full mr-3"
            :class="productsStatus === 'success' ? 'bg-green-500' : 
                   productsStatus === 'error' ? 'bg-red-500' : 'bg-yellow-500'"
          ></div>
          <span class="text-sm font-medium">Products API</span>
        </div>
        <button 
          @click="testProducts"
          :disabled="loading"
          class="btn btn-sm btn-secondary"
        >
          {{ loading ? 'Testing...' : 'Test' }}
        </button>
      </div>

      <!-- Bundles API -->
      <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
        <div class="flex items-center">
          <div 
            class="w-3 h-3 rounded-full mr-3"
            :class="bundlesStatus === 'success' ? 'bg-green-500' : 
                   bundlesStatus === 'error' ? 'bg-red-500' : 'bg-yellow-500'"
          ></div>
          <span class="text-sm font-medium">Bundles API</span>
        </div>
        <button 
          @click="testBundles"
          :disabled="loading"
          class="btn btn-sm btn-secondary"
        >
          {{ loading ? 'Testing...' : 'Test' }}
        </button>
      </div>

      <!-- Results -->
      <div v-if="lastResult" class="mt-4 p-3 rounded-lg text-sm"
           :class="lastResult.success ? 'bg-green-50 text-green-800' : 'bg-red-50 text-red-800'">
        <div class="font-medium">{{ lastResult.success ? 'Success' : 'Error' }}</div>
        <div class="mt-1">{{ lastResult.message }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { productApi, bundleApi } from '@/services/api'
import api from '@/services/api'

const loading = ref(false)
const healthStatus = ref<'idle' | 'success' | 'error'>('idle')
const productsStatus = ref<'idle' | 'success' | 'error'>('idle')
const bundlesStatus = ref<'idle' | 'success' | 'error'>('idle')
const lastResult = ref<{ success: boolean; message: string } | null>(null)

const testHealth = async () => {
  loading.value = true
  try {
    await api.get('/health')
    healthStatus.value = 'success'
    lastResult.value = { success: true, message: 'API health check passed' }
  } catch (error: any) {
    healthStatus.value = 'error'
    lastResult.value = { 
      success: false, 
      message: `Health check failed: ${error.message}` 
    }
  } finally {
    loading.value = false
  }
}

const testProducts = async () => {
  loading.value = true
  try {
    const result = await productApi.getProducts({ page: 1, pageSize: 1 })
    productsStatus.value = 'success'
    lastResult.value = { 
      success: true, 
      message: `Products API working. Found ${result.meta.totalItems} products` 
    }
  } catch (error: any) {
    productsStatus.value = 'error'
    lastResult.value = { 
      success: false, 
      message: `Products API failed: ${error.message}` 
    }
  } finally {
    loading.value = false
  }
}

const testBundles = async () => {
  loading.value = true
  try {
    const result = await bundleApi.getBundles({ page: 1, pageSize: 1 })
    bundlesStatus.value = 'success'
    lastResult.value = { 
      success: true, 
      message: `Bundles API working. Found ${result.meta.totalItems} bundles` 
    }
  } catch (error: any) {
    bundlesStatus.value = 'error'
    lastResult.value = { 
      success: false, 
      message: `Bundles API failed: ${error.message}` 
    }
  } finally {
    loading.value = false
  }
}
</script>