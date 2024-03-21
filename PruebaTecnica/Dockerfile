# Utiliza una imagen base que contenga el runtime de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Utiliza una imagen base que contenga el SDK de .NET para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PruebaTecnica.csproj", "./"]
RUN dotnet restore "./PruebaTecnica.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PruebaTecnica.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "PruebaTecnica.csproj" -c Release -o /app/publish

# Usa la imagen base de ASP.NET Core y copia la aplicación publicada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PruebaTecnica.dll"]
