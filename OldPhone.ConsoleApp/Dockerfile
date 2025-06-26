# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Multi-stage build for .NET 9.0 console application
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

# Set working directory
WORKDIR /src

# Copy project file first for better layer caching
COPY OldPhone.ConsoleApp/OldPhone.ConsoleApp.csproj OldPhone.ConsoleApp/
COPY OldPhone.sln ./

# Restore dependencies
RUN dotnet restore "OldPhone.ConsoleApp/OldPhone.ConsoleApp.csproj"

# Copy source code
COPY . .

# Build the application
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "OldPhone.ConsoleApp/OldPhone.ConsoleApp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OldPhone.ConsoleApp/OldPhone.ConsoleApp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS final

# Create non-root user for security
RUN addgroup -g 1001 -S appgroup && \
    adduser -u 1001 -S appuser -G appgroup

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Set entry point
ENTRYPOINT ["dotnet", "OldPhone.ConsoleApp.dll"]