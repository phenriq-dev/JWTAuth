#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["server/JWTAuth/JWTAuth.csproj", "server/JWTAuth/"]
COPY ["server/JWTAuth.Core/JWTAuth.Core.csproj", "server/JWTAuth.Core/"]
COPY ["server/JWTAuth.Db/JWTAuth.Db.csproj", "server/JWTAuth.Db/"]
COPY ["server/JWTAuth.Entities/JWTAuth.Entities.csproj", "server/JWTAuth.Entities/"]
RUN dotnet restore "./server/JWTAuth/JWTAuth.csproj"
COPY . .
WORKDIR "/src/server/JWTAuth"
RUN dotnet build "./JWTAuth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JWTAuth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JWTAuth.dll"]