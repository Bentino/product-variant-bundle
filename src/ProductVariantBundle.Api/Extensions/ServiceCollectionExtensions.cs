using Microsoft.AspNetCore.Mvc;
using ProductVariantBundle.Api.Filters;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Services;

namespace ProductVariantBundle.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // Add business services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBundleService, BundleService>();
        services.AddScoped<ISellableItemService, SellableItemService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IWarehouseService, WarehouseService>();

        // Add supporting services
        services.AddScoped<BundleAvailabilityCalculator>();
        services.AddScoped<BatchOperationService>();

        // Add validators
        services.AddScoped<ProductVariantBundle.Core.Validators.SkuValidator>();
        services.AddScoped<ProductVariantBundle.Core.Validators.ProductValidator>();
        services.AddScoped<ProductVariantBundle.Core.Validators.BundleValidator>();

        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        // Configure API behavior
        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Disable automatic model validation to use our custom filter
            options.SuppressModelStateInvalidFilter = true;
        });

        // Add validation filter
        services.AddScoped<ValidationFilter>();

        // Configure controllers with JSON options and validation filter
        services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        return services;
    }
}