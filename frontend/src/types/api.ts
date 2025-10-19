// API Response Types
export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
  errors?: string[]
}

export interface PagedResult<T> {
  data: T[]
  meta: PaginationMeta
}

export interface PaginationMeta {
  page: number
  pageSize: number
  total: number
  totalPages: number
  hasNext: boolean
  hasPrevious: boolean
}

// Product Types
export interface ProductMaster {
  id: string
  name: string
  slug: string
  description: string
  category: string
  attributes: Record<string, any>
  variants: ProductVariant[]
  status: EntityStatus
  createdAt: string
  updatedAt: string
}

export interface ProductVariant {
  id: string
  productMasterId: string
  sku: string
  price: number
  optionValues: VariantOptionValue[]
  attributes: Record<string, any>
  sellableItem: SellableItem
  status: EntityStatus
  createdAt: string
  updatedAt: string
}

export interface VariantOptionValue {
  optionName: string
  optionSlug: string
  value: string
}

// Bundle Types
export interface ProductBundle {
  id: string
  name: string
  description: string
  sku: string
  price: number
  items: BundleItem[]
  metadata: Record<string, any>
  sellableItem: SellableItem
  status: EntityStatus
  createdAt: string
  updatedAt: string
}

export interface BundleItem {
  sellableItemId: string
  quantity: number
  sellableItem: SellableItem
}

// Common Types
export interface SellableItem {
  id: string
  sku: string
  type: SellableItemType
}

export enum SellableItemType {
  Variant = 0,
  Bundle = 1
}

export enum EntityStatus {
  Inactive = 0,
  Active = 1,
  Archived = 2
}

// Filter Types
export interface ProductFilter {
  page?: number
  pageSize?: number
  search?: string
  category?: string
  status?: EntityStatus
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export interface BundleFilter {
  page?: number
  pageSize?: number
  search?: string
  status?: EntityStatus
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

// Inventory Types
export interface InventoryRecord {
  id: string
  sku: string
  sellableItemId: string
  sellableItem: SellableItem
  warehouseId: string
  warehouseCode: string
  onHand: number
  reserved: number
  available: number
  lastUpdated: string
  createdAt: string
  updatedAt: string
  status: EntityStatus
}

export interface InventoryFilter {
  page?: number
  pageSize?: number
  search?: string
  warehouse?: string
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export interface StockUpdateRequest {
  sku: string
  onHand: number
  reserved: number
  warehouseCode: string
}

export interface StockReservationRequest {
  sku: string
  quantity: number
  warehouseCode: string
}