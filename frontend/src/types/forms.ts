// Form request types
export interface CreateProductRequest {
  name: string
  slug: string
  description: string
  category: string
  status: number
  attributes?: Record<string, any>
}

export interface UpdateProductRequest extends Partial<CreateProductRequest> {}

export interface CreateBundleRequest {
  name: string
  description: string
  sku: string
  price: number
  status: number
  items: BundleItemRequest[]
  metadata?: Record<string, any>
}

export interface UpdateBundleRequest extends Partial<CreateBundleRequest> {}

export interface BundleItemRequest {
  sellableItemId: string
  quantity: number
}

// Form state types
export interface FormField<T = any> {
  value: T
  error: string
  touched: boolean
}

export interface ProductFormData {
  name: FormField<string>
  slug: FormField<string>
  description: FormField<string>
  category: FormField<string>
  attributes: FormField<Record<string, any>>
}

export interface BundleFormData {
  name: FormField<string>
  description: FormField<string>
  sku: FormField<string>
  price: FormField<number>
  items: FormField<BundleItemRequest[]>
  metadata: FormField<Record<string, any>>
}

// Validation types
export interface ValidationRule {
  required?: boolean
  minLength?: number
  maxLength?: number
  pattern?: RegExp
  custom?: (value: any) => string | null
}

export interface ValidationRules {
  [key: string]: ValidationRule
}