# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SalesAI.API/SalesAI.API.csproj", "src/SalesAI.API/"]
COPY ["src/SalesAI.Application/SalesAI.Application.csproj", "src/SalesAI.Application/"]
COPY ["src/SalesAI.Domain/SalesAI.Domain.csproj", "src/SalesAI.Domain/"]
COPY ["src/SalesAI.Infrastructure/SalesAI.Infrastructure.csproj", "src/SalesAI.Infrastructure/"]

RUN dotnet restore "src/SalesAI.API/SalesAI.API.csproj"

COPY src/ src/

WORKDIR "/src/src/SalesAI.API"
RUN dotnet build "SalesAI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesAI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesAI.API.dll"]
