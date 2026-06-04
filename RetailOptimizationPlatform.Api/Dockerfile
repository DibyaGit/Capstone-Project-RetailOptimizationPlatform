# Multi-stage Dockerfile for RetailOptimizationPlatform.Api
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["RetailOptimizationPlatform.Api/RetailOptimizationPlatform.Api.csproj", "RetailOptimizationPlatform.Api/"]
COPY ["RetailOptimizationPlatform.Core/RetailOptimizationPlatform.Core.csproj", "RetailOptimizationPlatform.Core/"]
COPY ["RetailOptimizationPlatform.Data/RetailOptimizationPlatform.Data.csproj", "RetailOptimizationPlatform.Data/"]
RUN dotnet restore "RetailOptimizationPlatform.Api/RetailOptimizationPlatform.Api.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/RetailOptimizationPlatform.Api"
RUN dotnet build "RetailOptimizationPlatform.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "RetailOptimizationPlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetailOptimizationPlatform.Api.dll"]
