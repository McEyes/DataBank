# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["DataQualityAssessment.Job.Host/DataQualityAssessment.Job.Host.csproj", "DataQualityAssessment.Job.Host/"]
COPY ["DataQualityAssessment.Application/DataQualityAssessment.Application.csproj", "DataQualityAssessment.Application/"]
COPY ["DataQualityAssessment.Core/DataQualityAssessment.Core.csproj", "DataQualityAssessment.Core/"]
COPY ["DataQualityAssessment.Database.Migrations/DataQualityAssessment.Database.Migrations.csproj", "DataQualityAssessment.Database.Migrations/"]
COPY ["DataQualityAssessment.EntityFramework.Core/DataQualityAssessment.EntityFramework.Core.csproj", "DataQualityAssessment.EntityFramework.Core/"]
RUN dotnet restore "./DataQualityAssessment.Job.Host/DataQualityAssessment.Job.Host.csproj"
COPY . .
WORKDIR "/src/DataQualityAssessment.Job.Host"
RUN dotnet build "./DataQualityAssessment.Job.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataQualityAssessment.Job.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataQualityAssessment.Job.Host.dll"]