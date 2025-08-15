# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/OldPhone.UI/OldPhone.UI.Web/OldPhone.UI.Web.csproj", "src/OldPhone.UI/OldPhone.UI.Web/"]
COPY ["src/OldPhone.UI/OldPhone.UI.Web.Client/OldPhone.UI.Web.Client.csproj", "src/OldPhone.UI/OldPhone.UI.Web.Client/"]
COPY ["src/OldPhone.Core.Processor/OldPhone.Core.Processor.csproj", "src/OldPhone.Core.Processor/"]
COPY ["src/OldPhone.Infrastructure/OldPhone.Infrastructure.csproj", "src/OldPhone.Infrastructure/"]
COPY ["src/OldPhone.Core.Shared/OldPhone.Core.Shared.csproj", "src/OldPhone.Core.Shared/"]

# Restore dependencies
RUN dotnet restore "src/OldPhone.UI/OldPhone.UI.Web/OldPhone.UI.Web.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/OldPhone.UI/OldPhone.UI.Web"
RUN dotnet build "OldPhone.UI.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OldPhone.UI.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OldPhone.UI.Web.dll"]