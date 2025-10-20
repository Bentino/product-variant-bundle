import axios from 'axios'
import type { 
  ApiResponse, 
  PagedResult, 
  ProductMaster, 
  ProductVariant,
  ProductBundle, 
  ProductFilter, 
  BundleFilter,
  InventoryRecord,
  InventoryFilter,
  StockUpdateRequest,
  StockReservationRequest
} from '@/types/api'

const api = axios.create({
  baseURL: '/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add request logging in development
if (import.meta.env.DEV) {
  api.interceptors.request.use(
    (config) => {
      console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)
      return config
    }
  )
  
  api.interceptors.response.use(
    (response) => {
      console.log(`✅ API Response: ${response.status} ${response.config.url}`)
      return response
    },
    (error) => {
      console.error(`❌ API Error: ${error.response?.status} ${error.config?.url}`, error.response?.data)
      return Promise.reject(error)
    }
  )
}

// Request interceptor
api.interceptors.request.use(
  (config) => {
    // Add auth token if available
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      localStorage.removeItem('auth_token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Product API
export const productApi = {
  async getProducts(filter: ProductFilter = {}): Promise<PagedResult<ProductMaster>> {
    const { data } = await api.get<ApiResponse<PagedResult<ProductMaster>>>('/products', {
      params: filter
    })
    return data.data
  },

  async getProduct(id: string): Promise<ProductMaster> {
    const { data } = await api.get<ApiResponse<ProductMaster>>(`/products/${id}`)
    return data.data
  },

  async createProduct(product: Partial<ProductMaster>): Promise<ProductMaster> {
    const { data } = await api.post<ApiResponse<ProductMaster>>('/products', product)
    return data.data
  },

  async updateProduct(id: string, product: Partial<ProductMaster>): Promise<ProductMaster> {
    const { data } = await api.put<ApiResponse<ProductMaster>>(`/products/${id}`, product)
    return data.data
  },

  async deleteProduct(id: string): Promise<void> {
    await api.delete(`/products/${id}`)
  },

  async getProductVariants(productId: string): Promise<ProductVariant[]> {
    const { data } = await api.get<ApiResponse<ProductVariant[]>>(`/products/${productId}/variants`)
    return data.data
  },

  async createVariant(productId: string, variant: any): Promise<ProductVariant> {
    const { data } = await api.post<ApiResponse<ProductVariant>>(`/products/${productId}/variants`, variant)
    return data.data
  },

  async updateVariant(productId: string, variantId: string, variant: any): Promise<ProductVariant> {
    const { data } = await api.put<ApiResponse<ProductVariant>>(`/products/${productId}/variants/${variantId}`, variant)
    return data.data
  },

  async deleteVariant(productId: string, variantId: string): Promise<void> {
    await api.delete(`/products/${productId}/variants/${variantId}`)
  }
}

// Bundle API
export const bundleApi = {
  async getBundles(filter: BundleFilter = {}): Promise<PagedResult<ProductBundle>> {
    const { data } = await api.get<ApiResponse<PagedResult<ProductBundle>>>('/bundles', {
      params: filter
    })
    return data.data
  },

  async getBundle(id: string): Promise<ProductBundle> {
    const { data } = await api.get<ApiResponse<ProductBundle>>(`/bundles/${id}`)
    return data.data
  },

  async createBundle(bundle: Partial<ProductBundle>): Promise<ProductBundle> {
    const { data } = await api.post<ApiResponse<ProductBundle>>('/bundles', bundle)
    return data.data
  },

  async updateBundle(id: string, bundle: Partial<ProductBundle>): Promise<ProductBundle> {
    const { data } = await api.put<ApiResponse<ProductBundle>>(`/bundles/${id}`, bundle)
    return data.data
  },

  async deleteBundle(id: string): Promise<void> {
    await api.delete(`/bundles/${id}`)
  }
}

// Inventory API
export const inventoryApi = {
  async getInventoryRecords(filter: InventoryFilter = {}): Promise<PagedResult<InventoryRecord>> {
    const { data } = await api.get<ApiResponse<PagedResult<InventoryRecord>>>('/inventory', {
      params: filter
    })
    return data.data
  },

  async getInventoryRecord(sku: string): Promise<InventoryRecord> {
    const { data } = await api.get<ApiResponse<InventoryRecord>>(`/inventory/${sku}`)
    return data.data
  },

  async updateStock(sku: string, stockData: StockUpdateRequest): Promise<InventoryRecord> {
    const { data } = await api.put<ApiResponse<InventoryRecord>>(`/inventory/${sku}/stock`, {
      SKU: stockData.sku,
      OnHand: stockData.onHand,
      Reserved: stockData.reserved,
      WarehouseCode: stockData.warehouseCode
    })
    return data.data
  },

  async reserveStock(sku: string, reserveData: StockReservationRequest): Promise<InventoryRecord> {
    const { data } = await api.post<ApiResponse<InventoryRecord>>(`/inventory/${sku}/reserve`, {
      SKU: reserveData.sku,
      Quantity: reserveData.quantity,
      WarehouseCode: reserveData.warehouseCode
    })
    return data.data
  },

  async releaseStock(sku: string, releaseData: StockReservationRequest): Promise<InventoryRecord> {
    const { data } = await api.post<ApiResponse<InventoryRecord>>(`/inventory/${sku}/release`, releaseData)
    return data.data
  }
}

export default api