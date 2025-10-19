<template>
  <div class="card">
    <!-- Table Header -->
    <div class="px-6 py-4 border-b border-gray-200">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-3 sm:space-y-0">
        <div>
          <h3 class="text-lg font-medium text-gray-900">{{ title }}</h3>
          <p v-if="description" class="mt-1 text-sm text-gray-500">{{ description }}</p>
        </div>
        <div class="flex items-center space-x-3">
          <!-- Search -->
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search..."
              class="input pl-10 w-64"
              @input="onSearch"
            />
          </div>
          <!-- Actions slot -->
          <slot name="actions" />
        </div>
      </div>
    </div>

    <!-- Table -->
    <div class="overflow-x-auto">
      <table class="table">
        <thead>
          <tr>
            <th
              v-for="column in columns"
              :key="column.key"
              :class="[
                column.sortable ? 'cursor-pointer hover:bg-gray-100' : '',
                column.align === 'center' ? 'text-center' : '',
                column.align === 'right' ? 'text-right' : ''
              ]"
              @click="column.sortable ? onSort(column.key) : null"
            >
              <div class="flex items-center space-x-1">
                <span>{{ column.label }}</span>
                <div v-if="column.sortable" class="flex flex-col">
                  <ChevronUpIcon 
                    class="w-3 h-3 -mb-1"
                    :class="sortBy === column.key && sortDirection === 'asc' 
                      ? 'text-primary-500' 
                      : 'text-gray-300'"
                  />
                  <ChevronDownIcon 
                    class="w-3 h-3"
                    :class="sortBy === column.key && sortDirection === 'desc' 
                      ? 'text-primary-500' 
                      : 'text-gray-300'"
                  />
                </div>
              </div>
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-if="loading">
            <td :colspan="columns.length" class="text-center py-12">
              <div class="flex items-center justify-center">
                <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-primary-500"></div>
                <span class="ml-2 text-gray-500">Loading...</span>
              </div>
            </td>
          </tr>
          <tr v-else-if="!data.length">
            <td :colspan="columns.length" class="text-center py-12 text-gray-500">
              No data available
            </td>
          </tr>
          <template v-else>
            <tr v-for="(item, index) in data" :key="getRowKey(item, index)">
              <td
                v-for="column in columns"
                :key="column.key"
                :class="[
                  column.align === 'center' ? 'text-center' : '',
                  column.align === 'right' ? 'text-right' : ''
                ]"
              >
                <slot 
                  :name="`cell-${column.key}`" 
                  :item="item" 
                  :value="getNestedValue(item, column.key)"
                  :index="index"
                >
                  {{ formatCellValue(getNestedValue(item, column.key), column) }}
                </slot>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div v-if="pagination && pagination.totalPages > 1" class="px-6 py-4 border-t border-gray-200">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-3 sm:space-y-0">
        <div class="text-sm text-gray-700">
          Showing {{ ((pagination.page - 1) * pagination.pageSize) + 1 }} to 
          {{ Math.min(pagination.page * pagination.pageSize, pagination.totalItems) }} of 
          {{ pagination.totalItems }} results
        </div>
        <div class="flex items-center space-x-2">
          <button
            :disabled="!pagination.hasPreviousPage"
            @click="onPageChange(pagination.page - 1)"
            class="btn btn-secondary btn-sm"
            :class="{ 'opacity-50 cursor-not-allowed': !pagination.hasPreviousPage }"
          >
            <ChevronLeftIcon class="w-4 h-4" />
            Previous
          </button>
          
          <div class="flex items-center space-x-1">
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="onPageChange(page)"
              class="btn btn-sm"
              :class="page === pagination.page ? 'btn-primary' : 'btn-secondary'"
            >
              {{ page }}
            </button>
          </div>
          
          <button
            :disabled="!pagination.hasNextPage"
            @click="onPageChange(pagination.page + 1)"
            class="btn btn-secondary btn-sm"
            :class="{ 'opacity-50 cursor-not-allowed': !pagination.hasNextPage }"
          >
            Next
            <ChevronRightIcon class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import {
  MagnifyingGlassIcon,
  ChevronUpIcon,
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon
} from '@heroicons/vue/24/outline'
import type { PaginationMeta } from '@/types/api'

interface Column {
  key: string
  label: string
  sortable?: boolean
  align?: 'left' | 'center' | 'right'
  format?: 'currency' | 'date' | 'datetime' | 'number'
}

interface Props {
  title: string
  description?: string
  columns: Column[]
  data: any[]
  loading?: boolean
  pagination?: PaginationMeta
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

interface Emits {
  (e: 'search', query: string): void
  (e: 'sort', column: string, direction: 'asc' | 'desc'): void
  (e: 'page-change', page: number): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  sortDirection: 'asc'
})

const emit = defineEmits<Emits>()

const searchQuery = ref('')

const visiblePages = computed(() => {
  if (!props.pagination) return []
  
  const current = props.pagination.page
  const total = props.pagination.totalPages
  const delta = 2
  
  const range = []
  const rangeWithDots = []
  
  for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
    range.push(i)
  }
  
  if (current - delta > 2) {
    rangeWithDots.push(1, '...')
  } else {
    rangeWithDots.push(1)
  }
  
  rangeWithDots.push(...range)
  
  if (current + delta < total - 1) {
    rangeWithDots.push('...', total)
  } else {
    rangeWithDots.push(total)
  }
  
  return rangeWithDots.filter((item, index, arr) => arr.indexOf(item) === index)
})

const onSearch = () => {
  emit('search', searchQuery.value)
}

const onSort = (column: string) => {
  const newDirection = props.sortBy === column && props.sortDirection === 'asc' ? 'desc' : 'asc'
  emit('sort', column, newDirection)
}

const onPageChange = (page: number) => {
  emit('page-change', page)
}

const getRowKey = (item: any, index: number) => {
  return item.id || item.key || index
}

const getNestedValue = (obj: any, path: string) => {
  return path.split('.').reduce((current, key) => current?.[key], obj)
}

const formatCellValue = (value: any, column: Column) => {
  if (value == null) return '-'
  
  switch (column.format) {
    case 'currency':
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(value)
    case 'date':
      return new Date(value).toLocaleDateString()
    case 'datetime':
      return new Date(value).toLocaleString()
    case 'number':
      return new Intl.NumberFormat().format(value)
    default:
      return value
  }
}

// Debounced search
let searchTimeout: NodeJS.Timeout
watch(searchQuery, () => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    onSearch()
  }, 300)
})
</script>