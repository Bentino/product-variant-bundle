import { ref, reactive, computed } from 'vue'
import type { FormField, ValidationRule } from '@/types/forms'

export function useFormValidation<T extends Record<string, any>>(
  initialData: T,
  rules: Record<keyof T, ValidationRule> = {}
) {
  // Create reactive form data
  const formData = reactive<Record<keyof T, FormField>>(
    Object.keys(initialData).reduce((acc, key) => {
      acc[key as keyof T] = {
        value: initialData[key],
        error: '',
        touched: false
      }
      return acc
    }, {} as Record<keyof T, FormField>)
  )

  // Validation functions
  const validateField = (fieldName: keyof T, value: any): string => {
    const rule = rules[fieldName]
    if (!rule) return ''

    // Required validation
    if (rule.required) {
      if (value === null || value === undefined) {
        return `${String(fieldName)} is required`
      }
      if (typeof value === 'string' && !value.trim()) {
        return `${String(fieldName)} is required`
      }
    }

    // Skip other validations if value is empty and not required
    if ((value === null || value === undefined || (typeof value === 'string' && !value.trim())) && !rule.required) return ''

    // Min length validation
    if (rule.minLength && typeof value === 'string' && value.length < rule.minLength) {
      return `${String(fieldName)} must be at least ${rule.minLength} characters`
    }

    // Max length validation
    if (rule.maxLength && typeof value === 'string' && value.length > rule.maxLength) {
      return `${String(fieldName)} must not exceed ${rule.maxLength} characters`
    }

    // Pattern validation
    if (rule.pattern && typeof value === 'string' && !rule.pattern.test(value)) {
      return `${String(fieldName)} format is invalid`
    }

    // Custom validation
    if (rule.custom) {
      const customError = rule.custom(value)
      if (customError) return customError
    }

    return ''
  }

  const validateForm = (): boolean => {
    let isValid = true
    
    Object.keys(formData).forEach(key => {
      const fieldName = key as keyof T
      const field = formData[fieldName]
      const error = validateField(fieldName, field.value)
      
      field.error = error
      field.touched = true
      
      if (error) isValid = false
    })
    
    return isValid
  }

  const validateSingleField = (fieldName: keyof T) => {
    const field = formData[fieldName]
    field.error = validateField(fieldName, field.value)
    field.touched = true
  }

  const resetForm = () => {
    Object.keys(formData).forEach(key => {
      const fieldName = key as keyof T
      const field = formData[fieldName]
      field.value = initialData[fieldName]
      field.error = ''
      field.touched = false
    })
  }

  const setFieldValue = (fieldName: keyof T, value: any) => {
    formData[fieldName].value = value
    if (formData[fieldName].touched) {
      validateSingleField(fieldName)
    }
  }

  const setFieldError = (fieldName: keyof T, error: string) => {
    formData[fieldName].error = error
    formData[fieldName].touched = true
  }

  // Computed properties
  const isFormValid = computed(() => {
    return Object.values(formData).every(field => !field.error)
  })

  const hasErrors = computed(() => {
    return Object.values(formData).some(field => field.error && field.touched)
  })

  const formValues = computed(() => {
    return Object.keys(formData).reduce((acc, key) => {
      acc[key as keyof T] = formData[key as keyof T].value
      return acc
    }, {} as T)
  })

  return {
    formData,
    validateForm,
    validateSingleField,
    resetForm,
    setFieldValue,
    setFieldError,
    isFormValid,
    hasErrors,
    formValues
  }
}