# Imagen base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ./Statistics/*.csproj ./Statistics/

# Restaurar dependencias
RUN dotnet restore ./Statistics/Statistics.csproj

# Copiar el resto de los archivos
COPY . .

# Publicar la aplicación
WORKDIR /src/Statistics
RUN dotnet publish -c Release -o /app/publish

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5062
ENTRYPOINT ["dotnet", "Statistics.dll"]