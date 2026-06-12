# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StocksApp_Module/StocksApp2/StocksApp2.csproj", "StocksApp_Module/StocksApp2/"]
COPY ["Contact_Manager_Module/Entities/Entities.csproj", "Contact_Manager_Module/Entities/"]
COPY ["Contact_Manager_Module/ServiceContracts/ServiceContracts.csproj", "Contact_Manager_Module/ServiceContracts/"]
COPY ["Contact_Manager_Module/Servicess/Servicess.csproj", "Contact_Manager_Module/Servicess/"]
COPY ["StocksApp_Module/Repositories/Repositories.csproj", "StocksApp_Module/Repositories/"]
COPY ["StocksApp_Module/RepositryContracts/RepositryContracts.csproj", "StocksApp_Module/RepositryContracts/"]
COPY ["StocksApp_Module/ServiceContracts/ServiceContractsContacts.csproj", "StocksApp_Module/ServiceContracts/"]
COPY ["StocksApp_Module/Entities/EntitiesStocks.csproj", "StocksApp_Module/Entities/"]
COPY ["StocksApp_Module/Repositories_Stocks/Repositories_Stocks.csproj", "StocksApp_Module/Repositories_Stocks/"]
COPY ["StocksApp_Module/RipositoryContracts/RepositoryContracts.csproj", "StocksApp_Module/RipositoryContracts/"]
COPY ["StocksApp_Module/Services/Services.csproj", "StocksApp_Module/Services/"]
RUN dotnet restore "./StocksApp_Module/StocksApp2/StocksApp2.csproj"
COPY . .
WORKDIR "/src/StocksApp_Module/StocksApp2"
RUN dotnet build "./StocksApp2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StocksApp2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StocksApp2.dll"]