<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Navigation -->
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <!-- Logo -->
            <div class="flex-shrink-0 flex items-center">
              <div class="flex items-center space-x-2">
                <div class="w-8 h-8 bg-primary-500 rounded-lg flex items-center justify-center">
                  <span class="text-white font-bold text-sm">PV</span>
                </div>
                <span class="font-semibold text-gray-900 hidden sm:block">
                  Product Variant Bundle
                </span>
              </div>
            </div>
            
            <!-- Navigation Links -->
            <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
              <router-link
                v-for="item in navigation"
                :key="item.name"
                :to="item.href"
                class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors"
                :class="$route.path === item.href
                  ? 'border-primary-500 text-primary-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              >
                <component :is="item.icon" class="w-4 h-4 mr-2" />
                {{ item.name }}
              </router-link>
            </div>
          </div>
          
          <!-- Mobile menu button -->
          <div class="sm:hidden flex items-center">
            <button
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
            >
              <Bars3Icon v-if="!mobileMenuOpen" class="w-6 h-6" />
              <XMarkIcon v-else class="w-6 h-6" />
            </button>
          </div>
        </div>
      </div>
      
      <!-- Mobile menu -->
      <div v-show="mobileMenuOpen" class="sm:hidden">
        <div class="pt-2 pb-3 space-y-1">
          <router-link
            v-for="item in navigation"
            :key="item.name"
            :to="item.href"
            @click="mobileMenuOpen = false"
            class="flex items-center px-3 py-2 text-base font-medium transition-colors"
            :class="$route.path === item.href
              ? 'bg-primary-50 border-primary-500 text-primary-700 border-l-4'
              : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'"
          >
            <component :is="item.icon" class="w-5 h-5 mr-3" />
            {{ item.name }}
          </router-link>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Bars3Icon, 
  XMarkIcon,
  CubeIcon,
  RectangleGroupIcon,
  ArchiveBoxIcon,
  Squares2X2Icon,
  DocumentTextIcon
} from '@heroicons/vue/24/outline'

const mobileMenuOpen = ref(false)

const navigation = [
  { name: 'Catalog', href: '/', icon: Squares2X2Icon },
  { name: 'Products', href: '/products', icon: CubeIcon },
  { name: 'Bundles', href: '/bundles', icon: RectangleGroupIcon },
  { name: 'Inventory', href: '/inventory', icon: ArchiveBoxIcon },
  { name: 'API Reference', href: '/api-reference', icon: DocumentTextIcon },
]
</script>