# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ContatosRegionais.ContatosFunction/ContatosRegionais.ContatosFunction.csproj", "src/ContatosRegionais.ContatosFunction/"]
COPY ["src/ContatosRegionais.Application/ContatosRegionais.Application.csproj", "src/ContatosRegionais.Application/"]
COPY ["src/ContatosRegionais.Domain/ContatosRegionais.Domain.csproj", "src/ContatosRegionais.Domain/"]
COPY ["src/ContatosRegionais.Infra.Data/ContatosRegionais.Infra.Data.csproj", "src/ContatosRegionais.Infra.Data/"]
COPY ["src/ContatosRegionais.Service/ContatosRegionais.Service.csproj", "src/ContatosRegionais.Service/"]
RUN dotnet restore "./src/ContatosRegionais.ContatosFunction/ContatosRegionais.ContatosFunction.csproj"
COPY . .
WORKDIR "/src/src/ContatosRegionais.ContatosFunction"
RUN dotnet build "./ContatosRegionais.ContatosFunction.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContatosRegionais.ContatosFunction.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true