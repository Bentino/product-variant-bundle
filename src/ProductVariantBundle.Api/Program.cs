using ProductVariantBundle.Infrastructure.Extensions;
using ProductVariantBundle.Api.Extensions;
using ProductVariantBundle.Api.Middleware;
using ProductVariantBundle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var apiConfig = builder.Configuration.GetSection("Api");
    var title = apiConfig["Title"] ?? "Product Variant Bundle API";
    var version = apiConfig["Version"] ?? "v1";
    var description = apiConfig["Description"] ?? "API for managing product variants and bundles with flexible schema design";
    var contactName = apiConfig["Contact:Name"] ?? "API Support";
    var contactEmail = apiConfig["Contact:Email"] ?? "support@example.com";

    c.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo
    { 
        Title = title, 
        Version = version,
        Description = $@"{description}

## Features
- **Product Management**: Create and manage product masters with multiple variants
- **Bundle Management**: Create virtual product bundles with automatic availability calculation
- **Inventory Management**: Track stock levels with warehouse support and reservation capabilities
- **Batch Operations**: Perform bulk operations with idempotency support
- **Flexible Schema**: JSONB-based extensible attributes for future enhancements

## Response Format
All API responses follow a consistent envelope format:
```json
{{
  ""data"": {{ /* actual response data */ }},
  ""meta"": {{ /* pagination and metadata */ }},
  ""errors"": []
}}
```

## Error Handling
Error responses follow RFC 7807 Problem Details format with detailed validation information.

## Authentication
Currently, no authentication is required. This is suitable for internal APIs or development environments.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = contactName,
            Email = contactEmail
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License"
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add examples and better documentation
    c.EnableAnnotations();
    c.DescribeAllParametersInCamelCase();
    
    // Add response examples
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
    {
        Url = "http://localhost:8080",
        Description = "Development Server"
    });

    // Group endpoints by tags
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Infrastructure services (Database, Repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Core services (Business logic)
builder.Services.AddCore();

// Add API configuration (controllers, validation, etc.)
builder.Services.AddApiConfiguration();

// Add CORS
builder.Services.AddCors(options =>
{
    var corsConfig = builder.Configuration.GetSection("Cors");
    var allowedOrigins = corsConfig.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
    var allowedMethods = corsConfig.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "*" };
    var allowedHeaders = corsConfig.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "*" };
    var allowCredentials = corsConfig.GetValue<bool>("AllowCredentials");

    options.AddPolicy("ApiCorsPolicy", policy =>
    {
        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        if (allowedMethods.Contains("*"))
        {
            policy.AllowAnyMethod();
        }
        else
        {
            policy.WithMethods(allowedMethods);
        }

        if (allowedHeaders.Contains("*"))
        {
            policy.AllowAnyHeader();
        }
        else
        {
            policy.WithHeaders(allowedHeaders);
        }

        if (allowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Add global exception handling middleware first
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Variant Bundle API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Skip HTTPS redirection in Docker environment
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("ApiCorsPolicy");

// Add response wrapper middleware before authorization
app.UseMiddleware<ResponseWrapperMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Apply database migrations automatically BEFORE starting the app
Console.WriteLine("=== STARTING DATABASE MIGRATION ===");
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Got ApplicationDbContext successfully");
        
        Console.WriteLine("Checking database connection...");
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"Can connect to database: {canConnect}");
        
        Console.WriteLine("Starting database migration...");
        await context.Database.MigrateAsync();
        Console.WriteLine("Database migration completed successfully.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"=== DATABASE MIGRATION FAILED ===");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    throw; // Stop the application if migration fails
}
Console.WriteLine("=== MIGRATION SECTION COMPLETED ===");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();