# Dockerfile para backend .NET - CORREGIDO
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore REPS-backend.csproj
RUN dotnet build REPS-backend.csproj -c Release -o /app/build
RUN dotnet publish REPS-backend.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Ejecutar con configuración de producción
ENTRYPOINT ["dotnet", "REPS-backend.dll", "--urls", "http://0.0.0.0:5000"]

