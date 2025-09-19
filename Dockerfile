# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY src/ ./src/
COPY tests/ ./tests/
COPY *.sln ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet build --configuration Release --no-restore

# Publish the API
RUN dotnet publish src/API/DotNetApiLambdaTemplate.API/DotNetApiLambdaTemplate.API.csproj \
    --configuration Release \
    --no-build \
    --output /app/publish

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:5001;http://+:5000

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "DotNetApiLambdaTemplate.API.dll"]
