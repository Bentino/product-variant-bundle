using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Infrastructure.Data;
using ProductVariantBundle.Infrastructure.Repositories;

namespace ProductVariantBundle.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbSection = configuration.GetSection("Database");
            var commandTimeout = dbSection["CommandTimeout"] != null ? int.Parse(dbSection["CommandTimeout"]) : 30;
            var maxRetryCount = dbSection["MaxRetryCount"] != null ? int.Parse(dbSection["MaxRetryCount"]) : 3;
            var maxRetryDelayString = dbSection["MaxRetryDelay"] ?? "00:00:30";
            var enableSensitiveDataLogging = dbSection["EnableSensitiveDataLogging"] != null ? bool.Parse(dbSection["EnableSensitiveDataLogging"]) : false;
            
            if (!TimeSpan.TryParse(maxRetryDelayString, out var maxRetryDelay))
            {
                maxRetryDelay = TimeSpan.FromSeconds(30);
            }

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: maxRetryCount,
                        maxRetryDelay: maxRetryDelay,
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(commandTimeout);
                });
            
            options.EnableSensitiveDataLogging(enableSensitiveDataLogging);
        });

        // Add repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBundleRepository, BundleRepository>();
        services.AddScoped<ISellableItemRepository, SellableItemRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IBatchOperationRepository, BatchOperationRepository>();

        // Add services
        services.AddScoped<ISampleDataService, ProductVariantBundle.Infrastructure.Services.SampleDataService>();

        return services;
    }
}