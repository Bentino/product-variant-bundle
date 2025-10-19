# Development stage - for hot reload
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /src

# Install dotnet tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy project files and restore
COPY ["src/ProductVariantBundle.Api/ProductVariantBundle.Api.csproj", "src/ProductVariantBundle.Api/"]
COPY ["src/ProductVariantBundle.Core/ProductVariantBundle.Core.csproj", "src/ProductVariantBundle.Core/"]
COPY ["src/ProductVariantBundle.Infrastructure/ProductVariantBundle.Infrastructure.csproj", "src/ProductVariantBundle.Infrastructure/"]

RUN dotnet restore "src/ProductVariantBundle.Api/ProductVariantBundle.Api.csproj"

# Set working directory
WORKDIR /src/src/ProductVariantBundle.Api

# Expose port
EXPOSE 8080

# Default command for development
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]

# Build stage - for production
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/ProductVariantBundle.Api/ProductVariantBundle.Api.csproj", "src/ProductVariantBundle.Api/"]
COPY ["src/ProductVariantBundle.Core/ProductVariantBundle.Core.csproj", "src/ProductVariantBundle.Core/"]
COPY ["src/ProductVariantBundle.Infrastructure/ProductVariantBundle.Infrastructure.csproj", "src/ProductVariantBundle.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/ProductVariantBundle.Api/ProductVariantBundle.Api.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/ProductVariantBundle.Api"
RUN dotnet build "ProductVariantBundle.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ProductVariantBundle.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProductVariantBundle.Api.dll"]