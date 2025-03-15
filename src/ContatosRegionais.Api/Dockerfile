# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/ContatosRegionais.Api/ContatosRegionais.Api.csproj", "src/ContatosRegionais.Api/"]
COPY ["src/ContatosRegionais.Application/ContatosRegionais.Application.csproj", "src/ContatosRegionais.Application/"]
COPY ["src/ContatosRegionais.Domain/ContatosRegionais.Domain.csproj", "src/ContatosRegionais.Domain/"]
COPY ["src/ContatosRegionais.Infra.Data/ContatosRegionais.Infra.Data.csproj", "src/ContatosRegionais.Infra.Data/"]
COPY ["src/ContatosRegionais.Service/ContatosRegionais.Service.csproj", "src/ContatosRegionais.Service/"]

RUN dotnet restore "./src/ContatosRegionais.Api/ContatosRegionais.Api.csproj"
COPY . .
WORKDIR "/src/src/ContatosRegionais.Api"
RUN dotnet build "./ContatosRegionais.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContatosRegionais.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ContatosRegionais.Api.dll"]