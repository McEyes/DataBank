# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["DataQualityAssessment/DataQualityAssessment.Web.Entry/DataQualityAssessment.Web.Entry.csproj", "DataQualityAssessment/DataQualityAssessment.Web.Entry/"]
COPY ["DataQualityAssessment/DataQualityAssessment.Web.Core/DataQualityAssessment.Web.Core.csproj", "DataQualityAssessment/DataQualityAssessment.Web.Core/"]
COPY ["DataQualityAssessment/DataQualityAssessment.Application/DataQualityAssessment.Application.csproj", "DataQualityAssessment/DataQualityAssessment.Application/"]
COPY ["DataQualityAssessment/DataQualityAssessment.Core/DataQualityAssessment.Core.csproj", "DataQualityAssessment/DataQualityAssessment.Core/"]
COPY ["DataQualityAssessment/DataQualityAssessment.Database.Migrations/DataQualityAssessment.Database.Migrations.csproj", "DataQualityAssessment/DataQualityAssessment.Database.Migrations/"]
COPY ["DataQualityAssessment/DataQualityAssessment.EntityFramework.Core/DataQualityAssessment.EntityFramework.Core.csproj", "DataQualityAssessment/DataQualityAssessment.EntityFramework.Core/"]
RUN dotnet restore "./DataQualityAssessment/DataQualityAssessment.Web.Entry/DataQualityAssessment.Web.Entry.csproj"
COPY . .
WORKDIR "/src/DataQualityAssessment/DataQualityAssessment.Web.Entry"
RUN dotnet build "./DataQualityAssessment.Web.Entry.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataQualityAssessment.Web.Entry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataQualityAssessment.Web.Entry.dll"]